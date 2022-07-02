using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using TopCharts.DataAccess.Abstractions;
using TopCharts.Domain.Model.Api;

namespace TopCharts.DataAccess.Api
{
    public class ApiRequester : IApiRequester
    {
        private readonly HttpClient _httpClient;

        public ApiRequester(ApiRequesterOptions options, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add(
                HeaderNames.Accept, "application/json");
        }
        public async Task<Response> GetTimelineAsync(TimelineRequest request, CancellationToken cancellationToken)
        {
            return await _httpClient.GetFromJsonAsync<Response>(GetTimelineUrl(request), cancellationToken);
        }

        public async Task<ExtraData> GetExtraData(int id, CancellationToken cancellationToken)
        {
            var resp = await _httpClient.GetStringAsync(
                $"https://vc.ru/vote/get_likers?id={id}&type=1&mode=raw", cancellationToken);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            try
            {
                var data = JsonSerializer.Deserialize<LikersResponse>(resp, options);
                return new ExtraData()
                {
                    Likes = data.Data.Likers.Values.Where(x => x.Sign > 0).Sum(x=> x.Sign),
                    Dislikes = data.Data.Likers.Values.Where(x => x.Sign < 0).Sum(x=> x.Sign),
                };
            }
            catch
            {
                var data = JsonSerializer.Deserialize<EmptyLikersResponse>(resp, options);
                if (data.Data.Likers.Length == 0)
                {
                    return new ExtraData();
                }
                throw;
            }
        }

        private static string GetTimelineUrl(TimelineRequest request)
        {
            return $"timeline?allSite=true&sorting=date&lastId={request.LastId}&lastSortingValue={request.LastSortingValue}";
        }
    }
}