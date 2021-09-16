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
            foreach (var entity in detectEntitiesResponse.Entities)
                Console.WriteLine("Text: {0}, Type: {1}, Score: {2}", entity.Text, entity.Type, entity.Score);

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
            foreach (var entity in detectKeyPhrasesResponse.KeyPhrases)
                Console.WriteLine("Text: {0}, Score: {1}", entity.Text, entity.Score);

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
            Console.WriteLine(
                "Sentiment: {0}, Score Mixed: {1}, Score Neutral: {2}, Score Positive: {3}, Score Negative: {4}"
                , detectSentimentResponse.Sentiment.Value, detectSentimentResponse.SentimentScore.Mixed
                , detectSentimentResponse.SentimentScore.Neutral, detectSentimentResponse.SentimentScore.Positive,
                detectSentimentResponse.SentimentScore.Negative);

            return detectSentimentResponse;
        }
    }
}
