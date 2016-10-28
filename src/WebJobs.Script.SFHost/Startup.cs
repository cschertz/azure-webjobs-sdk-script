using System.Web.Http;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Owin;
using System;
using System.Fabric;
using Microsoft.Azure.WebJobs.Script;
using System.IO;

namespace WebJobs.Script.SFHost
{
    using Microsoft.Azure.WebJobs.Script.Config;

    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder, ICodePackageActivationContext context)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config, ScriptSettingsManager.Instance, GetDefaultSettings(context));

            appBuilder.UseWebApi(config);
        }


        private static WebHostSettings GetDefaultSettings(ICodePackageActivationContext context)
        {
            WebHostSettings settings = new WebHostSettings();

            DataPackage dataPkg = context.GetDataPackageObject("Extensions");

            string functionsStorageAccount = ConfigurationReader.GetConfigValue<String>("Config", "Config", "FunctionsStorageAccount", "");

            //Environment.SetEnvironmentVariable("AzureWebJobsDashboard", functionsStorageAccount);
            ScriptSettingsManager.Instance.SetSetting("AzureWebJobsDashboard", functionsStorageAccount);
            //Environment.SetEnvironmentVariable("AzureWebJobsStorage", functionsStorageAccount);
            ScriptSettingsManager.Instance.SetSetting("AzureWebJobsStorage", functionsStorageAccount);

            //settings.ScriptPath = ConfigurationReader.GetConfigValue<String>("Config", "Config", EnvironmentSettingNames.AzureWebJobsScriptRoot, "");
            settings.ScriptPath = Path.Combine(dataPkg.Path, ".\\code\\site\\wwwroot");
            settings.LogPath = Path.Combine(dataPkg.Path, @"LogFiles\Application\Functions");
            settings.SecretsPath = Path.Combine(dataPkg.Path, ".\\code\\site\\security");
            settings.IsSelfHost = true;



            if (string.IsNullOrEmpty(settings.ScriptPath))
            {
                throw new InvalidOperationException("Unable to determine function script root directory.");
            }

            return settings;
        }

    }
}
