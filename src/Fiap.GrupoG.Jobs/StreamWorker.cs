using Fiap.GrupoG.Jobs.Twitter;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Jobs
{
    public class StreamWorker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITwitterService _twitterService;

        public StreamWorker(ILogger<Worker> logger, ITwitterService twitterService)
        {
            _logger = logger;
            _twitterService = twitterService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _twitterService.BuscarTweetStreamAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                }
                finally
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}
