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
            //--- 必要ならプロキシを生成
            WebProxy? proxy = null;
            if (config?.IsEnabled == true)
            {
                proxy = new WebProxy(config.Host, config.Port);
                if (config.Credentials != null)
                    proxy.Credentials = new NetworkCredential(config.Credentials.UserName, config.Credentials.Password);
            }

            //--- HttpClient を構成
            const string ScrapingServiceHttpClient = "KKTalking.Externals.Instagram.Services.ScrapingService.HttpClient";
            services
                .AddHttpClient(ScrapingServiceHttpClient)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    Proxy = proxy,
                    AllowAutoRedirect = false,  // リダイレクトが発生したら検知する
                });

            //--- ScrapingService を構成
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
