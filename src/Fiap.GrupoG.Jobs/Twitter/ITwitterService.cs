using Fiap.GrupoG.Mongo.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Jobs.Twitter
{
    public interface ITwitterService
    {
        Task<TweetSearchDto> BuscarTweetAsync(CancellationToken stoppingToken, UserEntity user);
        Task BuscarTweetStreamAsync();
    }
}
