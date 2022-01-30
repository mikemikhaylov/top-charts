using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kvyk.Telegraph.Models;
using TopCharts.DataAccess.Api;
using TopCharts.Domain.Model;
using TopCharts.Domain.Model.Api;

namespace TopCharts.Domain.Services
{
    public class DigestPoster
    {
        private readonly PostingOptions _postingOptions;
        private readonly DigestBuilder _digestBuilder;
        private readonly TelegraphApi _telegraphApi;

        public DigestPoster(PostingOptions postingOptions, DigestBuilder digestBuilder, TelegraphApi telegraphApi)
        {
            _postingOptions = postingOptions;
            if (_postingOptions.Site != Site.Vc)
            {
                throw new NotImplementedException();
            }

            _digestBuilder = digestBuilder;
            _telegraphApi = telegraphApi;
        }

        public async Task PostWeek(DateTime dateTime, CancellationToken cancellationToken)
        {
            var from = StartOfWeek(dateTime.AddDays(-7), DayOfWeek.Monday);
            var to = from.AddDays(7);
            await PostPeriod("Лучшие статьи недели", "17 – 23 января", from, to, cancellationToken);
        }

        public async Task PostMonth(DateTime dateTime, CancellationToken cancellationToken)
        {
            var firstDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            await PostPeriod("Лучшие статьи за", "январь 2022", firstDayOfMonth.AddMonths(-1), firstDayOfMonth,
                cancellationToken);
        }

        private async Task PostPeriod(string title, string period, DateTime from, DateTime to,
            CancellationToken cancellationToken)
        {
            var digests = await _digestBuilder.BuildAsync(_postingOptions.Site, from, to, cancellationToken);
            var linksByDigest = new Dictionary<SubSiteType, DigestLinks>();
            foreach (var digest in digests)
            {
                var links = new DigestLinks
                {
                    ByBookmarks = await _telegraphApi.CreatePageAsync(
                        $"ТОП-{digest.TopSize} {digest.Name} — {period} — по закладкам", cancellationToken),
                    ByLikes = await _telegraphApi.CreatePageAsync(
                        $"ТОП-{digest.TopSize} {digest.Name} — {period} — по лайкам", cancellationToken),
                    ByComments = await _telegraphApi.CreatePageAsync(
                        $"ТОП-{digest.TopSize} {digest.Name} — {period} — по комментариям", cancellationToken),
                    ByViews = await _telegraphApi.CreatePageAsync(
                        $"ТОП-{digest.TopSize} {digest.Name} — {period} — по просмотрам", cancellationToken),
                };
                linksByDigest[digest.SubSiteType] = links;
            }

            var mailLink =
                await _telegraphApi.CreatePageAsync($"{title} {period}",
                    cancellationToken);
            await EditMainLink(mailLink, digests, linksByDigest, cancellationToken);
            foreach (var digest in digests)
            {
                var links = linksByDigest[digest.SubSiteType];
                await EditDigestLink(links.ByLikes, mailLink, links, digest.ByLikes, cancellationToken);
                await EditDigestLink(links.ByBookmarks, mailLink, links, digest.ByBookmarks, cancellationToken);
                await EditDigestLink(links.ByComments, mailLink, links, digest.ByComments, cancellationToken);
                await EditDigestLink(links.ByViews, mailLink, links, digest.ByViews, cancellationToken);
            }
        }

        private async Task EditDigestLink(string url, string mainLink, DigestLinks digestLinks, Item[] items,
            CancellationToken cancellationToken)
        {
            var nodes = new List<Node>();
            nodes.Add(Node.);
            await _telegraphApi.EditPageAsync(url, nodes, cancellationToken);
        }

        private async Task EditMainLink(string url, Digest[] digests,
            Dictionary<SubSiteType, DigestLinks> linksByDigest, CancellationToken cancellationToken)
        {
            var nodes = new List<Node>();
            var allSite = digests.First(x => x.SubSiteType == SubSiteType.All);
            nodes.Add(Node.A(linksByDigest[allSite.SubSiteType].ByLikes, allSite.Name));
            nodes.Add(Node.Br());
            nodes.Add(Node.Br());
            foreach (var digest in digests.OrderBy(x => x.Name))
            {
                if (digest.SubSiteType is SubSiteType.All or SubSiteType.Other)
                {
                    continue;
                }

                nodes.Add(Node.A(linksByDigest[digest.SubSiteType].ByLikes, digest.Name));
                nodes.Add(Node.Br());
            }

            nodes.Add(Node.Br());
            var otherSite = digests.First(x => x.SubSiteType == SubSiteType.Other);
            nodes.Add(Node.A(linksByDigest[otherSite.SubSiteType].ByLikes, otherSite.Name));
            await _telegraphApi.EditPageAsync(url, nodes, cancellationToken);
        }

        private static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        private class DigestLinks
        {
            public string ByLikes { get; set; }
            public string ByComments { get; set; }
            public string ByBookmarks { get; set; }
            public string ByViews { get; set; }
            public string ByReposts { get; set; }
        }
    }
}