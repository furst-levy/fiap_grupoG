using Fiap.GrupoG.Mongo.Entities;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Mongo.Interfaces
{
    public interface ITweetRepository
    {
        Task SaveTweetAsync(TweetEnriry tweetEnriry);
    }
}
