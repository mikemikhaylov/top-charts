using System.Threading;
using System.Threading.Tasks;
using TopCharts.Domain.Model.Api;

namespace TopCharts.DataAccess.Abstractions
{
    public interface IApiRequester
    {
        Task<Response> GetTimelineAsync(TimelineRequest request, CancellationToken cancellationToken);
    }
}