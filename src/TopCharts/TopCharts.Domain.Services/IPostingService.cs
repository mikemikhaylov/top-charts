using System.Threading;
using System.Threading.Tasks;

namespace TopCharts.Domain.Services
{
    public interface IPostingService
    {
        Task ProcessAsync(CancellationToken cancellationToken);
    }
}