using Fiap.GrupoG.Jobs.AwsComprehend;
using Fiap.GrupoG.Jobs.Twitter;
using Fiap.GrupoG.Mongo.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Fiap.GrupoG.Mongo.Interfaces;

namespace Fiap.GrupoG.Jobs
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITwitterService _twitterService;
        private readonly IAwsComprehendServices _awsComprehendServices;
        private readonly ITweetRepository _tweetRepository;

        public Worker(ILogger<Worker> logger, IAwsComprehendServices awsComprehendServices,
            ITwitterService twitterService, ITweetRepository tweetRepository)
        {
            _logger = logger;
            _awsComprehendServices = awsComprehendServices;
            _twitterService = twitterService;
            _tweetRepository = tweetRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var tweetSearch = await _twitterService.BuscarTweetAsync(stoppingToken);

                foreach (var item in tweetSearch.Data)
                {
                    try
                    {
                        var detectEntities = await _awsComprehendServices.DetectEntitiesAsync(item.Text);
                        var detectKeyPhrases = await _awsComprehendServices.DetectKeyPhrasesAsync(item.Text);
                        var detectSentiment = await _awsComprehendServices.DetectSentimentAsync(item.Text);

                        await _tweetRepository.SaveTweetAsync(new TweetEnriry
                        {
                            
                        })


                        foreach (var detectEntity in detectEntities.Entities)
                        {
                            await connection.InsertAsync(new TweetEnriry.ComprehendEntity.DetectEntitiesDb
                            {
                                Score = detectEntity.Score,
                                Text = detectEntity.Text,
                                Type = detectEntity.Type
                            });
                        }

                        foreach (var detectKeyPhrase in detectKeyPhrases.KeyPhrases)
                        {
                            await connection.InsertAsync(new TweetEnriry.ComprehendEntity.DetectKeyPhrasesDb
                            {
                                Score = detectKeyPhrase.Score,
                                Text = detectKeyPhrase.Text
                            });
                        }

                        await connection.InsertAsync(new TweetEnriry.ComprehendEntity.DetectSentimentDb
                        {
                            Sentiment = detectSentiment.Sentiment.Value,
                            ScoreMixed = detectSentiment.SentimentScore.Mixed,
                            ScoreNeutral = detectSentiment.SentimentScore.Neutral,
                            ScorePositive = detectSentiment.SentimentScore.Positive,
                            ScoreNegative = detectSentiment.SentimentScore.Negative
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
