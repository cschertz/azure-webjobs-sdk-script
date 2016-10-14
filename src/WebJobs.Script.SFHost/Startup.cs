using System.Web.Http;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Owin;
using System;
using Microsoft.Azure.WebJobs.Script;
using System.IO;

namespace WebJobs.Script.SFHost
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config, GetDefaultSettings());

            appBuilder.UseWebApi(config);
        }


        private static WebHostSettings GetDefaultSettings()
        {
            WebHostSettings settings = new WebHostSettings();

            string functionsStorageAccount = ConfigurationReader.GetConfigValue<String>("Config", "Config", "FunctionsStorageAccount", "");

            Environment.SetEnvironmentVariable("AzureWebJobsDashboard", functionsStorageAccount);
            Environment.SetEnvironmentVariable("AzureWebJobsStorage", functionsStorageAccount);

            settings.ScriptPath = ConfigurationReader.GetConfigValue<String>("Config", "Config", EnvironmentSettingNames.AzureWebJobsScriptRoot, "");
            settings.LogPath = Path.Combine(Path.GetTempPath(), @"Functions");
            settings.SecretsPath = ConfigurationReader.GetConfigValue<String>("Config", "Config", "AzureWebSecretsPath", "");
            settings.IsSelfHost = true;

            if (string.IsNullOrEmpty(settings.ScriptPath))
            {
                throw new InvalidOperationException("Unable to determine function script root directory.");
            }

            return settings;
        }

    }
}
