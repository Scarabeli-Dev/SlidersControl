using SlidersControl.Data;
using SlidersControl.Entities;
using SlidersControl.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SlidersControl.ApiEndpoints;

public static class SlidersEndpoints
{
    public static void MapSlidersEndpoints(this WebApplication app)
    {
        app.MapGet("/sliders", async (SlidersControlDbContext db) =>
            await db.sliders.ToListAsync()).WithTags("SlidersControl").RequireAuthorization();

        app.MapGet("/sliders/{id:Guid}", async (Guid id, SlidersControlDbContext db) =>
        {
            return await db.sliders.FindAsync(id)
                         is Slider slider
                         ? Results.Ok(slider)
                         : Results.NotFound();
        }).RequireAuthorization();

        app.MapGet("/sliders/active", async (SlidersControlDbContext db) =>
        {
            return await db.sliders
                           .Where(act => act.IsActive.Equals(true))
                           .ToListAsync();

        }).RequireAuthorization();

        app.MapPost("/sliders", async ([FromForm] Slider slider, IFormFile largeImage, IFormFile smalImage, SlidersControlDbContext db, IStorageService storageService) =>
        {

            AwsCredentials awsCredentials = new AwsCredentials()
            {
                AwsKey = app.Configuration["AWS:AccessKeyId"],
                AwsSecretKey = app.Configuration["AWS:SecretAccessKey"]
            };

            var largeResult = await storageService.UploadFileAsync(largeImage, app.Configuration["AWS:BucketName"], awsCredentials);
            var smalResult = await storageService.UploadFileAsync(smalImage, app.Configuration["AWS:BucketName"], awsCredentials);

            slider.LargeImage = largeResult.Key;
            slider.SmalImage = smalResult.Key;
            slider.CreatedAt = DateTime.Now;
            db.sliders.Add(slider);
            await db.SaveChangesAsync();

            return Results.Created($"/sliders/{slider.Id}", slider);
        }).DisableAntiforgery().RequireAuthorization();

        app.MapPut("/sliders/{id:Guid}", async (Guid id, [FromForm] Slider slider, IFormFile? largeImage, IFormFile? smalImage, SlidersControlDbContext db, IStorageService storageService) =>
        {

            if (slider.Id != id)
            {
                return Results.BadRequest();
            }
            var sliderDB = await db.sliders.FindAsync(id);
            if (sliderDB is null) return Results.NotFound();

            if (largeImage != null)
            {
                AwsCredentials awsCredentials = new AwsCredentials()
                {
                    AwsKey = app.Configuration["AWS:AccessKeyId"],
                    AwsSecretKey = app.Configuration["AWS:SecretAccessKey"]
                };
                var largeResult = await storageService.UploadFileAsync(largeImage, app.Configuration["AWS:BucketName"], awsCredentials);

                sliderDB.LargeImage = largeResult.Key;
            }

            if (smalImage != null)
            {
                AwsCredentials awsCredentials = new AwsCredentials()
                {
                    AwsKey = app.Configuration["AWS:AccessKeyId"],
                    AwsSecretKey = app.Configuration["AWS:SecretAccessKey"]
                };
                var smalResult = await storageService.UploadFileAsync(smalImage, app.Configuration["AWS:BucketName"], awsCredentials);
                sliderDB.SmalImage = smalResult.Key;
            }



            sliderDB.AccessLink = slider.AccessLink;
            sliderDB.Order = slider.Order;

            if (slider.Order != Entities.Enum.Order.Not)
            {
                slider.IsActive = true;
            }
            else
            {
                sliderDB.IsActive = false;
            }

            await db.SaveChangesAsync();

            return Results.Ok(sliderDB);
        }).DisableAntiforgery().RequireAuthorization();

        app.MapDelete("/sliders/{id:Guid}", async (Guid id, SlidersControlDbContext db) =>
        {
            var slider = await db.sliders.FindAsync(id);

            if (slider is null)
            {
                return Results.NotFound();
            }

            db.sliders.Remove(slider);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}
