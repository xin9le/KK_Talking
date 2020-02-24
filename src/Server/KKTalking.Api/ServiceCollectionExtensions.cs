using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;



namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> の拡張機能を提供します。
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// アプリケーションの構成情報を DI サービスに登録します。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IConfigurationRoot AddConfiguration(this IServiceCollection services)
        {
            //--- 既定の構成を読み込む
            var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            var @default = builder.Build();

            //--- App Configuration を読み込む
            IConfigurationRoot configuration;
            var connectionString = @default.GetConnectionString("AppConfig");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                configuration = @default;
            }
            else
            {
                configuration = @default;
                /*
                var host = services.BuildServiceProvider().GetRequiredService<IHostingEnvironment>();
                configuration
                    = builder
                    .AddAzureAppConfiguration(o =>
                    {
                        o.Connect(connectionString);
                        o.Select("*");
                        o.Select("*", host.EnvironmentName);
                    })
                    .Build();
                */
            }

            //--- DI に登録
            services.AddSingleton(configuration);
            return configuration;
        }
    }
}
