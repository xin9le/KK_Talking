using System.Net.Http;
using KKTalking.Configurations;
using KKTalking.Domain.Instagram.Search;
using KKTalking.Externals.Instagram;
using Microsoft.Azure.Search;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;



namespace KKTalking.Domain.Instagram
{
    /// <summary>
    /// <see cref="IServiceCollection"/> の拡張機能を提供します。
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Instagram の投稿検索サービスを DI に登録します。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInstagramSearch(this IServiceCollection services, CloudStorageAccount storageAccount, CognitiveSearchConfiguration search, WebProxyConfiguration? webProxy = null)
        {
            services.AddInstagram(webProxy);

            const string SearchServiceHttpClient = "KKTalking.Domain.Instagram.Search.SearchService.HttpClient";
            services.AddHttpClient(SearchServiceHttpClient);
            services.TryAddSingleton(provider =>
            {
                //--- Blob Client
                var blobClient = storageAccount.CreateCloudBlobClient();

                //--- Azure Cognitive Search
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = factory.CreateClient(SearchServiceHttpClient);
                var credentials = new SearchCredentials(search.ApiKey);
                var serviceClient = new SearchServiceClient(credentials, httpClient, false) { SearchServiceName = search.ServiceName };

                //--- サービス生成
                return ActivatorUtilities.CreateInstance<SearchService>(provider, blobClient, serviceClient);
            });
            return services;
        }
    }
}
