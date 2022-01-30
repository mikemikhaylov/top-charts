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
        private readonly DataLoader _dataLoader;
        private readonly DigestPoster _digestPoster;

        private readonly PostingOptions _config;

        public PostingService(PostingOptions config, IKeyValueRepository keyValueRepository,
            IItemRepository itemRepository, 
            DataLoader dataLoader, DigestPoster digestPoster)
        {
            _config = config;
            _keyValueRepository = keyValueRepository;
            _itemRepository = itemRepository;
            _dataLoader = dataLoader;
            _digestPoster = digestPoster;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await _digestPoster.PostWeek(DateTime.Now.AddDays(1), cancellationToken);
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