using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TopCharts.DataAccess.Abstractions;
using TopCharts.Domain.Model.Api;

namespace TopCharts.Domain.Services
{
    public class PostingService : IPostingService
    {
        private readonly IKeyValueRepository _keyValueRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IApiRequester _apiRequester;
        private readonly DigestBuilder _digestBuilder;

        private readonly PostingOptions _config;

        public PostingService(PostingOptions config, IKeyValueRepository keyValueRepository, IItemRepository itemRepository, IApiRequester apiRequester, DigestBuilder digestBuilder)
        {
            _config = config;
            _keyValueRepository = keyValueRepository;
            _itemRepository = itemRepository;
            _apiRequester = apiRequester;
            _digestBuilder = digestBuilder;
        }
        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var digests = await _digestBuilder.BuildAsync(_config.Site, DateTime.Now.AddDays(-7),DateTime.Now,
                cancellationToken);
            return;
            
            if (_config.Type == PostingType.InitialDownload)
            {
                await InitialDownload(cancellationToken);
                return;
            }
            throw new System.NotImplementedException();
        }

        private static string InitialDownloadKey = "InitialDownload";
        private async Task InitialDownload(CancellationToken cancellationToken)
        {
            var initialDownloadValue =
                await _keyValueRepository.GetAsync(_config.Site, InitialDownloadKey, cancellationToken);
            var timelineRequest = initialDownloadValue == null ? new TimelineRequest() : JsonSerializer.Deserialize<TimelineRequest>(initialDownloadValue);
            var i = 0;
            while (true)
            {
                var result = await _apiRequester.GetTimelineAsync(timelineRequest, cancellationToken);
                foreach (var item in result.Result.Items)
                {
                    Console.WriteLine($"Saving article {item.Data.GetCreatedAt()} {item.Data.Title}");
                    CleanItem(item);
                    await _itemRepository.SaveAsync(item, cancellationToken);
                }
                timelineRequest = new TimelineRequest
                {
                    LastId = result.Result.LastId,
                    LastSortingValue = result.Result.LastSortingValue,
                };
                await _keyValueRepository.SetAsync(_config.Site, InitialDownloadKey,
                    JsonSerializer.Serialize(timelineRequest), cancellationToken);
                if (result.Result.Items.Any(x => x.Data.GetCreatedAt() < _config.InitialDate))
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

        private void CleanItem(Item item)
        {
            item.Site = _config.Site;
            var blockToGo = item.Data.Blocks?.FirstOrDefault(x => !x.Hidden && x.Type == "text");
            item.Data.Blocks = blockToGo == null ? Array.Empty<Block>() : new [] {blockToGo};
        }
    }
}