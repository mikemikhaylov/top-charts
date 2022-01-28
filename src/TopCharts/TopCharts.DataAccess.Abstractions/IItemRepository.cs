using System.Threading;
using System.Threading.Tasks;
using TopCharts.Domain.Model.Api;

namespace TopCharts.DataAccess.Abstractions
{
    public interface IItemRepository
    {
        Task SaveAsync(Item item, CancellationToken cancellationToken);
    }
}