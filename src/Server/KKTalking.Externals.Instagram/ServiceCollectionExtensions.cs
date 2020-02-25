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
            services.AddHttpClient();
            //services.TryAddScoped<ScrapingService>();
            return services;
        }
    }
}
