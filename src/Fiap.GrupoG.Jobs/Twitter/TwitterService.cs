using Confluent.Kafka;
using Fiap.GrupoG.Mongo.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Jobs.Twitter
{
    public class TwitterService : ITwitterService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly ILogger<TwitterService> _logger;

        public TwitterService(HttpClient httpClient, IConfigurationRoot configurationRoot, ILogger<TwitterService> logger)
        {
            _httpClient = httpClient;
            _configurationRoot = configurationRoot;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Remove("Bearer");
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer AAAAAAAAAAAAAAAAAAAAANYDOwEAAAAAKYN%2Bgy%2FP0CBhknbDJS1IMz%2BdLMo%3DoTATLs5ygEwQaV93iE3VPFnviB4wN5WMaWgy3LjiLnB5QEJqwH");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                    "AAAAAAAAAAAAAAAAAAAAANYDOwEAAAAAKYN%2Bgy%2FP0CBhknbDJS1IMz%2BdLMo%3DoTATLs5ygEwQaV93iE3VPFnviB4wN5WMaWgy3LjiLnB5QEJqwH");

        }

        public async Task<TweetSearchDto> BuscarTweetAsync(CancellationToken stoppingToken, UserEntity user)
        {
            
            var data = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            try
            {
                var response = await _httpClient.GetAsync(
                    $"/2/users/{user.UserId}/tweets?max_results=100&start_time={data}T00:00:00.000Z&end_time={data}T23:59:59.999Z&tweet.fields=created_at",
                    stoppingToken);

                _logger.LogInformation($"response code: {response.StatusCode}");
                Console.WriteLine($"response code: {response.StatusCode}");

                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    _logger.LogError($"statusCode: {response.StatusCode}. response body: {responseJson}");

                var tweetSearchDto = JsonSerializer.Deserialize<TweetSearchDto>(responseJson);
                tweetSearchDto.User = user;

                return tweetSearchDto;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
                return null;
            }
        }

        public async Task BuscarTweetStreamAsync()
        {
            _logger.LogInformation("Lendo tweets");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                    "AAAAAAAAAAAAAAAAAAAAANYDOwEAAAAAKYN%2Bgy%2FP0CBhknbDJS1IMz%2BdLMo%3DoTATLs5ygEwQaV93iE3VPFnviB4wN5WMaWgy3LjiLnB5QEJqwH");
            httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
            var requestUri = "https://api.twitter.com/2/tweets/search/stream?tweet.fields=created_at&expansions=author_id&user.fields=created_at";
            var stream = await httpClient.GetStreamAsync(requestUri);

            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var currentLine = reader.ReadLine();
                if (string.IsNullOrEmpty(currentLine)) continue;

                _logger.LogInformation("novo tweet:");
                _logger.LogInformation(currentLine);

                var config = new ProducerConfig { BootstrapServers = _configurationRoot.GetSection("KafkaServer").Value };

                using var producer = new ProducerBuilder<Null, string>(config).Build();
                var result = await producer.ProduceAsync("twitter-topic", new Message<Null, string> { Value = currentLine });

                producer.Flush(TimeSpan.FromSeconds(10));

                _logger.LogInformation($"tweet enviado para tópico kafka");
            }
        }
    }
}
