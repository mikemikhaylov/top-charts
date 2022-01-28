using System;
using System.Net.Http;
using System.Net.Http.Json;
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

        private static string GetTimelineUrl(TimelineRequest request)
        {
            return $"timeline?allSite=true&sorting=date&lastId={request.LastId}&lastSortingValue={request.LastSortingValue}";
        }
    }
}