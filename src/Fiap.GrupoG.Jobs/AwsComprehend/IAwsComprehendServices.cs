using Amazon.Comprehend.Model;
using System.Threading.Tasks;

namespace Fiap.GrupoG.Jobs.AwsComprehend
{
    public interface IAwsComprehendServices
    {
        Task<DetectEntitiesResponse> DetectEntitiesAsync(string texto);
        Task<DetectKeyPhrasesResponse> DetectKeyPhrasesAsync(string texto);
        Task<DetectSentimentResponse> DetectSentimentAsync(string texto);
    }
}
