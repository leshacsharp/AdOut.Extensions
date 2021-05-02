using System.Threading.Tasks;

namespace AdOut.Extensions.Authorization
{
    public interface IAuthenticationService
    {
        Task<AuthResponse> AuthenticateAsync(string clientId, string clientSecret);
    }
}
