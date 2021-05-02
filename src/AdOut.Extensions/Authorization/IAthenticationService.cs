using System.Threading.Tasks;

namespace AdOut.Extensions.Authorization
{
    public interface IAthenticationService
    {
        Task<AuthResponse> AuthenticateAsync(string clientId, string clientSecret);
    }
}
