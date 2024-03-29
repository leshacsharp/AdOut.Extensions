﻿using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;

namespace AdOut.Extensions.Authorization
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, string authServerUrl)
        {
            services.AddHttpClient();
            var baseUrl = authServerUrl.LastOrDefault() != '/' ? $"{authServerUrl}/" : authServerUrl;
            services.AddScoped<IAuthenticationService>(p => new AuthenticationService(p.GetRequiredService<IHttpClientFactory>(), baseUrl));
            return services;
        }
    }
}
