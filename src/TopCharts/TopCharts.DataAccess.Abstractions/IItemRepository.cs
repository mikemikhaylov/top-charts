using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TopCharts.Domain.Model;
using TopCharts.Domain.Model.Api;

namespace TopCharts.DataAccess.Abstractions
{
    public interface IItemRepository
    {
        Task SaveAsync(Item item, CancellationToken cancellationToken);

        Task<List<Item>> GetAsync(Site site, DateTime from, DateTime to, CancellationToken cancellationToken);
    }
}