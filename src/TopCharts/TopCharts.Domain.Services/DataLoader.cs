using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TopCharts.DataAccess.Abstractions;
using TopCharts.Domain.Model;
using TopCharts.Domain.Model.Api;

namespace TopCharts.Domain.Services
{
    public class DataLoader
    {
        private readonly IKeyValueRepository _keyValueRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IApiRequester _apiRequester;

        public DataLoader(IKeyValueRepository keyValueRepository, IItemRepository itemRepository, IApiRequester apiRequester)
        {
            _keyValueRepository = keyValueRepository;
            _itemRepository = itemRepository;
            _apiRequester = apiRequester;
        }
        public async Task LoadToDateAsync(Site site, string cacheKey, DateTime toDate, CancellationToken cancellationToken)
        {
            var initialDownloadValue =
                string.IsNullOrWhiteSpace(cacheKey) ? null :
                await _keyValueRepository.GetAsync(site, cacheKey, cancellationToken);
            var timelineRequest = initialDownloadValue == null ? new TimelineRequest() : JsonSerializer.Deserialize<TimelineRequest>(initialDownloadValue);
            var i = 0;
            while (true)
            {
                var result = await _apiRequester.GetTimelineAsync(timelineRequest, cancellationToken);
                foreach (var item in result.Result.Items)
                {
                    Console.WriteLine($"Saving article {item.Data.GetCreatedAt()} {item.Data.Title}");
                    CleanItem(site, item);
                    await _itemRepository.SaveAsync(item, cancellationToken);
                }
                timelineRequest = new TimelineRequest
                {
                    LastId = result.Result.LastId,
                    LastSortingValue = result.Result.LastSortingValue,
                };
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    await _keyValueRepository.SetAsync(site, cacheKey,
                        JsonSerializer.Serialize(timelineRequest), cancellationToken);
                }
                
                if (result.Result.Items.Any(x => x.Data.GetCreatedAt() < toDate))
                {
                    return;
                }

                if (i++ > 50000)
                {
                    Console.WriteLine("Operations overflow");
                    return;
                }
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
        private void CleanItem(Site site, Item item)
        {
            item.Site = site;
            var blockToGo = item.Data.Blocks?.FirstOrDefault(x => !x.Hidden && x.Type == "text");
            item.Data.Blocks = blockToGo == null ? Array.Empty<Block>() : new [] {blockToGo};
        }
    }
}