using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public class AwsImageStore : IImageStore
    {
        private readonly IAmazonS3 _client;
        private readonly IConfigurationService _configService;
        private readonly IReporter _reporter;
        private const string BucketName = "furventure-games";

        public AwsImageStore(IConfigurationService configService, IReporter reporter)
        {
            _reporter = reporter;
            _configService = configService;
            _client = new AmazonS3Client(new BasicAWSCredentials(_configService.GetConfig("AwsAccessKey"), _configService.GetConfig("AwsSecretKey")),
                RegionEndpoint.EUWest2);
        }

        public async Task<StoredImage> SaveImageAsync(Stream imageStream)
        {
            var key = Guid.NewGuid().ToString() + ".jpg";
            var response = await _client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = BucketName,
                InputStream = imageStream,
                Key = key,
            });

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                _reporter.ReportError($"Failed to save image to AWS. Error code '{response.HttpStatusCode}', image key '{key}'");
                return null;
            }

            var url = $"https://{BucketName}.s3.eu-west-2.amazonaws.com/{key}";
            return new StoredImage { id = url };
        }

        public async Task<int> CleanupImagesOlderThan(DateTime time)
        {
            var bucketList = await _client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = BucketName });

            var toDelete = bucketList.S3Objects.Where(a => a.LastModified < time);

            if (toDelete.Count() == 0) return 0; 

            var deleteResponse = await _client.DeleteObjectsAsync(new DeleteObjectsRequest
            {
                BucketName = BucketName,
                Objects = toDelete.Select(a => new KeyVersion
                {
                    Key = a.Key,
                    VersionId = null
                }).ToList()
            });

            var objectsDelete = deleteResponse.DeletedObjects.Count();
            return objectsDelete;
        }
    }
}
