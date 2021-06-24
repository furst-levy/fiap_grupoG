using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.GrupoG
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await BuscarTweet(stoppingToken);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                

                await Task.Delay(1000000000, stoppingToken);
            }
        }

        private async Task BuscarTweet(CancellationToken stoppingToken)
        {
            var connectionString = "Server=fiap-grupo-g.cluster-cq4ov4lzopxk.us-east-1.rds.amazonaws.com;Database=twitter;Uid=grupog;Pwd=grupog;";
            IEnumerable<UserDb> users;

            using (var connection = new MySqlConnection(connectionString))
            {
                users = await connection.QueryAsync<UserDb>("SELECT * FROM User");
            }

            if (users == null || !users.Any())
                return;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer AAAAAAAAAAAAAAAAAAAAANYDOwEAAAAAKYN%2Bgy%2FP0CBhknbDJS1IMz%2BdLMo%3DoTATLs5ygEwQaV93iE3VPFnviB4wN5WMaWgy3LjiLnB5QEJqwH");
            client.DefaultRequestHeaders.Add("IdUsuarioLogado", "svc_dict");

            var data = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            foreach (var user in users)
            {
                try
                {
                    var response = await client.GetAsync($"https://api.twitter.com/2/users/{user.UserId}/tweets?max_results=100&start_time={data}T00:00:00.000Z&end_time={data}T23:59:59.999Z", stoppingToken);

                    Console.WriteLine($"response code: {response.StatusCode}");

                    var responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                        Console.WriteLine($"statusCode: {response.StatusCode}. response body: {responseJson}");

                    var tweetSearch = JsonSerializer.Deserialize<TweetSearch>(responseJson);

                    if (!tweetSearch.Data.Any())
                        return;

                    foreach (var item in tweetSearch.Data)
                    {
                        try
                        {
                            var detectEntities = await DetectEntitiesAsync(item.Text);
                            var detectKeyPhrases = await DetectKeyPhrasesAsync(item.Text);
                            var detectSentiment = await DetectSentimentAsync(item.Text);

                            using var connection = new MySqlConnection(connectionString);
                            var tweetId = await connection.InsertAsync(item.ConverterParaTabela(1));

                            foreach (var detectEntity in detectEntities.Entities)
                            {
                                await connection.InsertAsync(new DetectEntitiesDb
                                {
                                    Score = detectEntity.Score,
                                    Text = detectEntity.Text,
                                    Type = detectEntity.Type,
                                    TweetId = tweetId
                                });
                            }

                            foreach (var detectKeyPhrase in detectKeyPhrases.KeyPhrases)
                            {
                                await connection.InsertAsync(new DetectKeyPhrasesDb
                                {
                                    Score = detectKeyPhrase.Score,
                                    Text = detectKeyPhrase.Text,
                                    TweetId = tweetId
                                });
                            }

                            await connection.InsertAsync(new DetectSentimentDb
                            {
                                Sentiment = detectSentiment.Sentiment.Value,
                                ScoreMixed = detectSentiment.SentimentScore.Mixed,
                                ScoreNeutral = detectSentiment.SentimentScore.Neutral,
                                ScorePositive = detectSentiment.SentimentScore.Positive,
                                ScoreNegative = detectSentiment.SentimentScore.Negative,
                                TweetId = tweetId
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private async Task<DetectEntitiesResponse> DetectEntitiesAsync(string texto)
        {
            var comprehendClient = new AmazonComprehendClient(Amazon.RegionEndpoint.USWest2);

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

        private async Task<DetectKeyPhrasesResponse> DetectKeyPhrasesAsync(string texto)
        {
            var comprehendClient = new AmazonComprehendClient(Amazon.RegionEndpoint.USWest2);

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

        private async Task<DetectSentimentResponse> DetectSentimentAsync(string texto)
        {
            var comprehendClient = new AmazonComprehendClient(Amazon.RegionEndpoint.USWest2);

            Console.WriteLine("Calling DetectSentimentAsync\n");
            var dDetectSentimentRequest = new DetectSentimentRequest
            {
                Text = texto,
                LanguageCode = "pt"
            };
            var detectSentimentResponse = await comprehendClient.DetectSentimentAsync(dDetectSentimentRequest);
            Console.WriteLine("Sentiment: {0}, Score Mixed: {1}, Score Neutral: {2}, Score Positive: {3}, Score Negative: {4}"
                , detectSentimentResponse.Sentiment.Value, detectSentimentResponse.SentimentScore.Mixed
                , detectSentimentResponse.SentimentScore.Neutral, detectSentimentResponse.SentimentScore.Positive, detectSentimentResponse.SentimentScore.Negative);

            return detectSentimentResponse;
        }
    }

    public class TweetSearch
    {
        public TweetSearch()
        {
            Data = new List<TweetSearchItem>();
        }

        [JsonPropertyName("data")]
        public IEnumerable<TweetSearchItem> Data { get; set; }
    }

    public class TweetSearchItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        public TweetDb ConverterParaTabela(int userId)
        {
            return new TweetDb(this, userId);
        }
    }

    [Table("User")]
    public class UserDb
    {
        [Key]
        public int Id { get; set; }
        public Int64 UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }

    [Table("Tweet")]
    public class TweetDb
    {
        public TweetDb() { }

        public TweetDb(TweetSearchItem tweetSearchItem, int userId)
        {
            UserId = userId;
            TweetId = tweetSearchItem.Id;
            Text = tweetSearchItem.Text;
            Date = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TweetId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        [Write(false)]
        [Computed]
        public virtual UserDb User { get; set; }
    }

    [Table("DetectEntities")]
    public class DetectEntitiesDb
    {
        [Key]
        public int Id { get; set; }
        public int TweetId { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public float Score { get; set; }
    }

    [Table("DetectKeyPhrases")]
    public class DetectKeyPhrasesDb
    {
        [Key]
        public int Id { get; set; }
        public int TweetId { get; set; }
        public string Text { get; set; }
        public float Score { get; set; }
    }

    [Table("DetectSentiment")]
    public class DetectSentimentDb
    {
        [Key]
        public int Id { get; set; }
        public int TweetId { get; set; }
        public string Sentiment { get; set; }
        public float ScoreMixed { get; set; }
        public float ScoreNeutral { get; set; }
        public float ScorePositive { get; set; }
        public float ScoreNegative { get; set; }
    }
}
