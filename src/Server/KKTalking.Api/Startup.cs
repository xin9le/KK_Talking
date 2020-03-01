using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(KKTalking.Api.Startup))]



namespace KKTalking.Api
{
    /// <summary>
    /// アプリケーションの起動に関する初期化処理を提供します。
    /// </summary>
    internal class Startup : FunctionsStartup
    {
        /// <summary>
        /// サービスを追加/構成します。
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            var config = services.AddConfiguration();
            var appSettings = services.AddAppSettings(config);
            services.AddDomainServices(appSettings);
        }
    }
}
