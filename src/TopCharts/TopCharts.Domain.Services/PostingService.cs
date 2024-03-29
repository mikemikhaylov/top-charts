using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly TelegraphApi _telegraphApi;

        private readonly PostingOptions _config;

        public PostingService(PostingOptions config, IKeyValueRepository keyValueRepository,
            IItemRepository itemRepository, 
            DataLoader dataLoader, DigestPoster digestPoster, TelegraphApi telegraphApi)
        {
            _config = config;
            _keyValueRepository = keyValueRepository;
            _itemRepository = itemRepository;
            _dataLoader = dataLoader;
            _digestPoster = digestPoster;
            _telegraphApi = telegraphApi;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            if (_config.Type == PostingType.InitialDownload)
            {
                await InitialDownload(cancellationToken);
                return;
            }
            if (_config.Type == PostingType.PrevWeek)
            {
                await PrevWeekDownload(cancellationToken);
                return;
            }
            if (_config.Type == PostingType.PrevMonth)
            {
                await PrevMonthDownload(cancellationToken);
                return;
            }
            if (_config.Type == PostingType.PrevSeason)
            {
                await PrevSeasonDownload(cancellationToken);
                return;
            }
            throw new System.NotImplementedException();
        }

        private static string InitialDownloadKey = "InitialDownload";

        private async Task PrevWeekDownload(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow.AddHours(3);
            var loadFrom = _digestPoster.GetPrevWeekBeginning(now);
            var weekKey = $"week={loadFrom.ToString("d", new CultureInfo("ru-RU"))}";
            var weekValue = await _keyValueRepository.GetAsync(_config.Site, weekKey, cancellationToken);
            if (weekValue != null)
            {
                Console.WriteLine("Week already posted");
                return;
            }
            await _dataLoader.LoadToDateAsync(_config.Site, null, loadFrom, false, cancellationToken);
            await _digestPoster.PostWeek(now, cancellationToken);
            await _keyValueRepository.SetAsync(_config.Site, weekKey, "done", cancellationToken);
        }
        private async Task PrevMonthDownload(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow.AddHours(3);
            var loadFrom = _digestPoster.GetPrevMonthBeginning(now);
            var monthKey = $"month={loadFrom.ToString("d", new CultureInfo("ru-RU"))}";
            var monthValue = await _keyValueRepository.GetAsync(_config.Site, monthKey, cancellationToken);
            if (monthValue != null)
            {
                Console.WriteLine("Month already posted");
                return;
            }
            await _dataLoader.LoadToDateAsync(_config.Site, null, loadFrom, true, cancellationToken);
            await _digestPoster.PostMonth(now, cancellationToken);
            await _keyValueRepository.SetAsync(_config.Site, monthKey, "done", cancellationToken);
        }
        
        private async Task PrevSeasonDownload(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow.AddHours(3);
            var loadFrom = _digestPoster.GetPrevSeasonBeginning(now);
            var seasonKey = $"season={loadFrom.ToString("d", new CultureInfo("ru-RU"))}";
            var seasonValue = await _keyValueRepository.GetAsync(_config.Site, seasonKey, cancellationToken);
            if (seasonValue != null)
            {
                Console.WriteLine("Season already posted");
                return;
            }
            await _dataLoader.LoadToDateAsync(_config.Site, null, loadFrom, false, cancellationToken);
            await _digestPoster.PostSeason(now, cancellationToken);
            await _keyValueRepository.SetAsync(_config.Site, seasonKey, "done", cancellationToken);
        }
        
        private async Task InitialDownload(CancellationToken cancellationToken)
        {
            var nowNextWeekBeginning = _digestPoster.GetPrevWeekBeginning(DateTime.UtcNow.AddHours(3)).AddDays(14);
            var currentNow = _config.InitialDate;
            var loadFrom = _digestPoster.GetPrevMonthBeginning(currentNow);
            await _dataLoader.LoadToDateAsync(_config.Site, InitialDownloadKey, loadFrom, false, cancellationToken);
            while (true)
            {
                await _digestPoster.PostWeek(currentNow, cancellationToken);
                var prevWeekBeginning = _digestPoster.GetPrevWeekBeginning(currentNow);
                if (prevWeekBeginning.Month != currentNow.Month)
                {
                    await _digestPoster.PostMonth(currentNow, cancellationToken);
                }

                currentNow = currentNow.AddDays(7);
                if (currentNow >= nowNextWeekBeginning)
                {
                    break;
                }
            }
        }
    }
}