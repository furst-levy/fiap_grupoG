using Fiap.GrupoG.Mongo.Entities;
using System;
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

        public TwitterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TweetSearchDto> BuscarTweetAsync(CancellationToken stoppingToken, UserEntity user)
        {
            _httpClient.DefaultRequestHeaders.Remove("Bearer");
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer AAAAAAAAAAAAAAAAAAAAANYDOwEAAAAAKYN%2Bgy%2FP0CBhknbDJS1IMz%2BdLMo%3DoTATLs5ygEwQaV93iE3VPFnviB4wN5WMaWgy3LjiLnB5QEJqwH");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                    "AAAAAAAAAAAAAAAAAAAAANYDOwEAAAAAKYN % 2Bgy % 2FP0CBhknbDJS1IMz % 2BdLMo % 3DoTATLs5ygEwQaV93iE3VPFnviB4wN5WMaWgy3LjiLnB5QEJqwH");

            var data = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            try
            {
                var response = await _httpClient.GetAsync(
                    $"/2/users/{user.UserId}/tweets?max_results=100&start_time={data}T00:00:00.000Z&end_time={data}T23:59:59.999Z",
                    stoppingToken);

                Console.WriteLine($"response code: {response.StatusCode}");

                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine($"statusCode: {response.StatusCode}. response body: {responseJson}");

                var tweetSearchDto = JsonSerializer.Deserialize<TweetSearchDto>(responseJson);
                tweetSearchDto.User = user;

                return tweetSearchDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
