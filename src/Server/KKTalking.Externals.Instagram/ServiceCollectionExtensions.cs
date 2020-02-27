using System.Net.Http;
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
        public static IServiceCollection AddInstagram(this IServiceCollection services)
        {
            const string HttpClientName = "KKTalking.Externals.Instagram.Services.ScrapingService.HttpClient";
            services.AddHttpClient(HttpClientName);
            services.TryAddScoped(provider =>
            {
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                var client = factory.CreateClient(HttpClientName);
                return new ScrapingService(client);
            });
            return services;
        }
    }
}
