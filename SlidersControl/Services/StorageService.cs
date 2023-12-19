using Amazon.Runtime;
using Amazon.S3.Transfer;
using Amazon.S3;
using SlidersControl.Entities;
using SlidersControl.Services.Interfaces;

namespace SlidersControl.Services;

public class StorageService : IStorageService
{
    public async Task<S3ResponseDto> UploadFileAsync(IFormFile file, string bucketName, AwsCredentials awsCredentials)
    {
        await using var stream = new MemoryStream();
        file.CopyTo(stream);

        var fileExt = Path.GetExtension(file.FileName);
        var largeImageName = $"{Guid.NewGuid()}{fileExt}";


        S3Object s3Object = new S3Object()
        {
            Name = largeImageName,
            InputStream = stream,
            BucketName = bucketName
        };

        var credentials = new BasicAWSCredentials(awsCredentials.AwsKey, awsCredentials.AwsSecretKey);

        var config = new AmazonS3Config()
        {
            RegionEndpoint = Amazon.RegionEndpoint.SAEast1
        };

        var response = new S3ResponseDto();

        try
        {
            var uploadRequest = new TransferUtilityUploadRequest()
            {
                InputStream = s3Object.InputStream,
                Key = s3Object.Name,
                BucketName = s3Object.BucketName,
                CannedACL = S3CannedACL.NoACL,
            };

            using var client = new AmazonS3Client(credentials, config);

            var transferUtitlity = new TransferUtility(client);

            await transferUtitlity.UploadAsync(uploadRequest);

            response.StatusCode = 200;
            response.Message = $"{s3Object.Name} has been uploaded successfully";
        }
        catch (AmazonS3Exception ex)
        {
            response.StatusCode = (int)ex.StatusCode;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
        }
        response.Key = s3Object.Name;
        return response;
    }
}
