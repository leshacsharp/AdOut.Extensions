using System.Threading;
using System.Threading.Tasks;

namespace AdOut.Extensions.Context
{
    public interface ICommitProvider
    {
        Task<int> SaveChangesAsync(bool generateEvents = true, CancellationToken cancellationToken = default);
    }
}
