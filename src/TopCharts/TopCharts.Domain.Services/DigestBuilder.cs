using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopCharts.DataAccess.Abstractions;
using TopCharts.Domain.Model;
using TopCharts.Domain.Model.Api;

namespace TopCharts.Domain.Services
{
    public class DigestBuilder
    {
        private static readonly int TopAllCount = 30;
        private static readonly int TopSubSiteCount = 30;
        private static readonly HashSet<SubSiteType> SubSiteTypes = Enum.GetValues<SubSiteType>().Where(x=> x != SubSiteType.All && x != SubSiteType.Other).ToHashSet();

        private readonly IItemRepository _itemRepository;

        public DigestBuilder(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<Digest[]> BuildAsync(Site site, DateTime from, DateTime to,
            CancellationToken cancellationToken)
        {
            var items = await _itemRepository.GetAsync(site, from, to, cancellationToken);
            if (!items.Any())
            {
                return Array.Empty<Digest>();
            }
            var digests = new List<Digest> {GetDigest(SubSiteType.All, items, TopAllCount)};
            foreach (var subSiteType in SubSiteTypes)
            {
                var digestItems = items.Where(x => x.Data.SubSite.Id == (int) subSiteType).ToList();
                if (!digestItems.Any())
                {
                    continue;
                }
                digests.Add(GetDigest(subSiteType, digestItems, TopSubSiteCount));
            }
            var otherItems = items.Where(x => !SubSiteTypes.Contains((SubSiteType)x.Data.SubSite.Id)).ToList();
            if (otherItems.Any())
            {
                digests.Add(GetDigest(SubSiteType.Other, otherItems, TopAllCount));
            }
            return digests.ToArray();
        }

        private Digest GetDigest(SubSiteType subSiteType, List<Item> items, int topCount)
        {
            var digest = new Digest
            {
                SubSiteType = subSiteType,
                TopSize = topCount,
                ByBookmarks = items.OrderByDescending(x => x.Data.Counters.Favorites)
                    .ThenByDescending(x => x.Data.Likes.Summ)
                    .ThenByDescending(x => x.Data.HitsCount)
                    .ThenByDescending(x => x.Data.Counters.Comments).ThenBy(x => x.Data.Counters.Reposts)
                    .ThenByDescending(x => x.Data.GetCreatedAt()).Take(topCount).ToArray(),
                ByLikes = items.OrderByDescending(x => x.Data.Likes.Summ)
                    .ThenByDescending(x => x.Data.HitsCount)
                    .ThenByDescending(x => x.Data.Counters.Comments).ThenByDescending(x => x.Data.Counters.Favorites)
                    .ThenByDescending(x => x.Data.Counters.Reposts)
                    .ThenByDescending(x => x.Data.GetCreatedAt()).Take(topCount).ToArray(),
                ByComments = items.OrderByDescending(x => x.Data.Counters.Comments)
                    .ThenByDescending(x => x.Data.Likes.Summ).ThenByDescending(x => x.Data.HitsCount)
                    .ThenByDescending(x => x.Data.Counters.Favorites).ThenByDescending(x => x.Data.Counters.Reposts)
                    .ThenByDescending(x => x.Data.GetCreatedAt()).Take(topCount).ToArray(),
                ByReposts = items.OrderByDescending(x => x.Data.Counters.Reposts)
                    .ThenByDescending(x => x.Data.Likes.Summ).ThenByDescending(x => x.Data.HitsCount)
                    .ThenByDescending(x => x.Data.Counters.Comments).ThenByDescending(x => x.Data.Counters.Favorites)
                    .ThenByDescending(x => x.Data.GetCreatedAt()).Take(topCount).ToArray(),
                ByViews = items.OrderByDescending(x => x.Data.HitsCount).ThenByDescending(x => x.Data.Likes.Summ)
                    .ThenByDescending(x => x.Data.Counters.Reposts)
                    .ThenByDescending(x => x.Data.Counters.Comments).ThenByDescending(x => x.Data.Counters.Favorites)
                    .ThenByDescending(x => x.Data.GetCreatedAt()).Take(topCount).ToArray(),
            };
            digest.TotalComments = items.Select(x=> x.Data.Counters.Comments).Sum();
            digest.TotalLikes = items.Select(x=> x.Data.Likes.Summ).Sum();
            digest.TotalReposts = items.Select(x=> x.Data.Counters.Reposts).Sum();
            digest.TotalViews = items.Select(x=> x.Data.HitsCount).Sum();
            digest.TotalBookmarks = items.Select(x=> x.Data.Counters.Favorites).Sum();
            digest.Name = GetSubSiteName(digest);
            return digest;
        }
        
        private static string GetSubSiteName(Digest digest)
        {
            if (digest.SubSiteType == SubSiteType.All)
            {
                return "Весь сайт";
            }
            if (digest.SubSiteType == SubSiteType.Other)
            {
                return "Остальные подсайты";
            }
            return digest.ByLikes.First().Data.SubSite.Name;
        }
    }
}