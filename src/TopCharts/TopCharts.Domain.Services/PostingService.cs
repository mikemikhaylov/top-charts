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
        private readonly DataLoader _dataLoader;

        private readonly PostingOptions _config;

        public PostingService(PostingOptions config, IKeyValueRepository keyValueRepository, IItemRepository itemRepository, IApiRequester apiRequester, DigestBuilder digestBuilder, DataLoader dataLoader)
        {
            _config = config;
            _keyValueRepository = keyValueRepository;
            _itemRepository = itemRepository;
            _apiRequester = apiRequester;
            _digestBuilder = digestBuilder;
            _dataLoader = dataLoader;
        }
        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var digests = await _digestBuilder.BuildAsync(_config.Site, DateTime.Now.AddDays(-700),DateTime.Now,
                cancellationToken);
            var top = digests.OrderByDescending(x => x.TotalViews).ToList();
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
            await _dataLoader.LoadToDateAsync(_config.Site, InitialDownloadKey, _config.InitialDate, cancellationToken);
        }
    }
}