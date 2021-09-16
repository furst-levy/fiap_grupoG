using Fiap.GrupoG.Mongo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fiap.GrupoG.Mongo.Entities;

namespace Fiap.GrupoG.Jobs.Twitter
{
    public class TwitterService : ITwitterService
    {
        private readonly IUserRepository _userRepository;
        private readonly HttpClient _httpClient;

        public TwitterService(IUserRepository userRepository, HttpClient httpClient)
        {
            _userRepository = userRepository;
            _httpClient = httpClient;
        }

        public async Task<TweetSearchDto> BuscarTweetAsync(CancellationToken stoppingToken)
        {
            var users = await _userRepository.ListUsersAsync();
            if (users == null || !users.Any())
                return null;

            _httpClient.DefaultRequestHeaders.Remove("Bearer");
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer AAAAAAAAAAAAAAAAAAAAANYDOwEAAAAAKYN%2Bgy%2FP0CBhknbDJS1IMz%2BdLMo%3DoTATLs5ygEwQaV93iE3VPFnviB4wN5WMaWgy3LjiLnB5QEJqwH");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "AAAAAAAAAAAAAAAAAAAAANYDOwEAAAAAKYN % 2Bgy % 2FP0CBhknbDJS1IMz % 2BdLMo % 3DoTATLs5ygEwQaV93iE3VPFnviB4wN5WMaWgy3LjiLnB5QEJqwH");

            var data = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            foreach (var user in users)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"/2/users/{user.UserId}/tweets?max_results=100&start_time={data}T00:00:00.000Z&end_time={data}T23:59:59.999Z", stoppingToken);

                    Console.WriteLine($"response code: {response.StatusCode}");

                    var responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                        Console.WriteLine($"statusCode: {response.StatusCode}. response body: {responseJson}");

                    return JsonSerializer.Deserialize<TweetSearchDto>(responseJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
