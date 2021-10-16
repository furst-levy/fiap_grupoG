using Fiap.GrupoG.Jobs.AwsComprehend;
using Fiap.GrupoG.Jobs.Twitter;
using Fiap.GrupoG.Mongo.Entities;
using Fiap.GrupoG.Mongo.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Fiap.GrupoG.Jobs
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITwitterService _twitterService;
        private readonly IAwsComprehendServices _awsComprehendServices;
        private readonly ITweetRepository _tweetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfigurationRoot _configurationRoot;

        public Worker(ILogger<Worker> logger, IAwsComprehendServices awsComprehendServices,
            ITwitterService twitterService, ITweetRepository tweetRepository, IUserRepository userRepository, IConfigurationRoot configurationRoot)
        {
            _logger = logger;
            _awsComprehendServices = awsComprehendServices;
            _twitterService = twitterService;
            _tweetRepository = tweetRepository;
            _userRepository = userRepository;
            _configurationRoot = configurationRoot;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var users = ListUsers();
                    if (users == null || !users.Any())
                        continue;

                    foreach (var user in users)
                    {
                        var tweetSearch = await _twitterService.BuscarTweetAsync(stoppingToken, user);

                        foreach (var item in tweetSearch.Data)
                        {
                            try
                            {
                                var detectEntities = await _awsComprehendServices.DetectEntitiesAsync(item.Text);
                                var detectKeyPhrases = await _awsComprehendServices.DetectKeyPhrasesAsync(item.Text);
                                var detectSentiment = await _awsComprehendServices.DetectSentimentAsync(item.Text);

                                var tweetEnriry = new TweetEnriry
                                {
                                    TweetId = item.Id,
                                    Text = item.Text,
                                    CreatedAt = item.CreatedAt,
                                    User = new TweetEnriry.UserEntity
                                    {
                                        UserId = tweetSearch.User.UserId,
                                        Name = tweetSearch.User.Name,
                                        UserName = tweetSearch.User.UserName
                                    },
                                    Comprehend = new TweetEnriry.ComprehendEntity()
                                };

                                detectEntities.Entities.ForEach(x =>
                                {
                                    tweetEnriry.Comprehend.DetectEntities.Add(
                                        new TweetEnriry.ComprehendEntity.DetectEntitiesDb
                                        {
                                            Text = x.Text,
                                            Score = x.Score,
                                            Type = x.Type
                                        });
                                });

                                detectKeyPhrases.KeyPhrases.ForEach(x =>
                                {
                                    tweetEnriry.Comprehend.DetectKeyPhrases.Add(
                                        new TweetEnriry.ComprehendEntity.DetectKeyPhrasesDb
                                        {
                                            Text = x.Text,
                                            Score = x.Score
                                        });
                                });

                                tweetEnriry.Comprehend.DetectSentiment.ScoreMixed = detectSentiment.SentimentScore.Mixed;
                                tweetEnriry.Comprehend.DetectSentiment.ScoreNegative =
                                    detectSentiment.SentimentScore.Negative;
                                tweetEnriry.Comprehend.DetectSentiment.ScoreNeutral =
                                    detectSentiment.SentimentScore.Neutral;
                                tweetEnriry.Comprehend.DetectSentiment.ScorePositive =
                                    detectSentiment.SentimentScore.Positive;
                                tweetEnriry.Comprehend.DetectSentiment.Sentiment = detectSentiment.Sentiment.Value;

                                await _tweetRepository.SaveTweetAsync(tweetEnriry);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                }
                finally
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
            }
        }

        public IEnumerable<UserEntity> ListUsers()
        {
            return _configurationRoot.GetSection("twitterUsers").Get<IEnumerable<UserEntity>>();
        }
    }
}
