using System.Threading.Tasks;

namespace AdOut.Extensions.Infrastructure
{
    public interface IInitialization
    {
        Task InitAsync();
    }
}
