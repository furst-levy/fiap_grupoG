using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace Fiap.GrupoG.Api
{
    public class AwsUtils
    {
        public static async Task UploadFileToS3(IFormFile file)
        {
            //https://stackoverflow.com/questions/70333681/for-an-amazon-s3-bucket-deplolyent-from-guithub-how-do-i-fix-the-error-accesscon
            //deixar como s3 publico
            using var client = new AmazonS3Client("AKIA4E42PRG7I46BQDUZ", "w801BluptxmykvCw9VspQ+hPoZgowI6vAXq8egdH", RegionEndpoint.USEast1);
            await using var newMemoryStream = new MemoryStream();
            file.CopyTo(newMemoryStream);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = file.FileName,
                BucketName = Environment.GetEnvironmentVariable("BUCKETNAME"),
                CannedACL = S3CannedACL.PublicRead
            };

            var fileTransferUtility = new TransferUtility(client);

            try
            {
                await fileTransferUtility.UploadAsync(uploadRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
