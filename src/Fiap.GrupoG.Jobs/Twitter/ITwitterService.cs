using System.Threading;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Jobs.Twitter
{
    public interface ITwitterService
    {
        Task<TweetSearchDto> BuscarTweetAsync(CancellationToken stoppingToken);
    }
}
