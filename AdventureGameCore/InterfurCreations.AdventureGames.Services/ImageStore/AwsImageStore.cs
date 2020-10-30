using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.ImageStore
{
    public class AwsImageStore : IImageStore
    {
        private readonly IAmazonS3 _client;
        private const string BucketName = "furventure-games";

        public AwsImageStore()
        {
            _client = new AmazonS3Client(RegionEndpoint.EUWest2);
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
                return null;

            var url = $"https://{BucketName}.s3.eu-west-2.amazonaws.com/{key}";
            return new StoredImage { id = url };
        }
    }
}
