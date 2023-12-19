using SlidersControl.Entities;

namespace SlidersControl.Services.Interfaces;

public interface IStorageService
{
    public Task<S3ResponseDto> UploadFileAsync(IFormFile file, string bucketName, AwsCredentials awsCredentials);
}
