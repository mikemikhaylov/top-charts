using System.Threading;
using System.Threading.Tasks;
using TopCharts.Domain.Model;

namespace TopCharts.DataAccess.Abstractions
{
    public interface IKeyValueRepository
    {
        Task<string> GetAsync(Site site, string key, CancellationToken cancellationToken);
        Task SetAsync(Site site, string key, string value, CancellationToken cancellationToken);
    }
}