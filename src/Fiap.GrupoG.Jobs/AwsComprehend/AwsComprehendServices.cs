using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Amazon.Runtime;
using System;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Jobs.AwsComprehend
{
    public class AwsComprehendServices : IAwsComprehendServices
    {
        private readonly AWSCredentials _awsCredentials;

        public AwsComprehendServices(AWSCredentials awsCredentials)
        {
            _awsCredentials = awsCredentials;
        }

        public async Task<DetectEntitiesResponse> DetectEntitiesAsync(string texto)
        {
            var comprehendClient = new AmazonComprehendClient(_awsCredentials, Amazon.RegionEndpoint.USWest2);

            Console.WriteLine("Calling DetectEntities\n");
            var detectEntitiesRequest = new DetectEntitiesRequest
            {
                Text = texto,
                LanguageCode = "pt"
            };

            var detectEntitiesResponse = await comprehendClient.DetectEntitiesAsync(detectEntitiesRequest);
            return detectEntitiesResponse;
        }

        public async Task<DetectKeyPhrasesResponse> DetectKeyPhrasesAsync(string texto)
        {
            var comprehendClient = new AmazonComprehendClient(_awsCredentials, Amazon.RegionEndpoint.USWest2);

            Console.WriteLine("Calling DetectKeyPhrasesAsync\n");
            var detectKeyPhrasesRequest = new DetectKeyPhrasesRequest
            {
                Text = texto,
                LanguageCode = "pt"
            };
            var detectKeyPhrasesResponse = await comprehendClient.DetectKeyPhrasesAsync(detectKeyPhrasesRequest);
            return detectKeyPhrasesResponse;
        }

        public async Task<DetectSentimentResponse> DetectSentimentAsync(string texto)
        {
            var comprehendClient = new AmazonComprehendClient(_awsCredentials, Amazon.RegionEndpoint.USWest2);

            Console.WriteLine("Calling DetectSentimentAsync\n");
            var dDetectSentimentRequest = new DetectSentimentRequest
            {
                Text = texto,
                LanguageCode = "pt"
            };
            var detectSentimentResponse = await comprehendClient.DetectSentimentAsync(dDetectSentimentRequest);
            return detectSentimentResponse;
        }
    }
}
