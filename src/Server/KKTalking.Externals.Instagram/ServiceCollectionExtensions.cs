using System.Net;
using System.Net.Http;
using KKTalking.Configurations;
using KKTalking.Externals.Instagram.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;



namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> の拡張機能を提供します。
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Instagram 関連サービスを DI に登録します。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInstagram(this IServiceCollection services, WebProxyConfiguration? config = null)
        {
            const string ScrapingServiceHttpClient = "KKTalking.Externals.Instagram.Services.ScrapingService.HttpClient";
            var builder = services.AddHttpClient(ScrapingServiceHttpClient);
            if (config != null)
            {
                var proxy = new WebProxy(config.Host, config.Port);
                if (config.Credentials != null)
                    proxy.Credentials = new NetworkCredential(config.Credentials.UserName, config.Credentials.Password);
                builder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { Proxy = proxy });
            }
            services.TryAddSingleton(provider =>
            {
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                var client = factory.CreateClient(ScrapingServiceHttpClient);
                return new ScrapingService(client);
            });
            return services;
        }
    }
}
