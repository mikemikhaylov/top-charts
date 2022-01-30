using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kvyk.Telegraph.Models;
using TopCharts.DataAccess.Abstractions;
using TopCharts.DataAccess.Api;
using TopCharts.Domain.Model;
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
        private readonly TelegraphApi _telegraphApi;

        private readonly PostingOptions _config;

        public PostingService(PostingOptions config, IKeyValueRepository keyValueRepository,
            IItemRepository itemRepository, IApiRequester apiRequester, DigestBuilder digestBuilder,
            DataLoader dataLoader, TelegraphApi telegraphApi)
        {
            _config = config;
            _keyValueRepository = keyValueRepository;
            _itemRepository = itemRepository;
            _apiRequester = apiRequester;
            _digestBuilder = digestBuilder;
            _dataLoader = dataLoader;
            _telegraphApi = telegraphApi;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var url = await _telegraphApi.CreatePageAsync("For future use", cancellationToken);
            var url2 = await _telegraphApi.EditPageAsync(url, "Title2", new List<Node>(){
                Node.P("Hello, World!222"),
            }, cancellationToken);
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