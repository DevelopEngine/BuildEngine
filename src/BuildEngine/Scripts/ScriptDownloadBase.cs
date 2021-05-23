using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BuildEngine.Scripts {
    public abstract class ScriptDownloadBase
    {
        public string? UserAgent { get; protected set; }
        protected string ScriptFilePath { get; }
        protected readonly ILogger<IScriptService> Logger;
        private IAppInfoProvider? InfoProvider { get; set; } = new EntryAppInfoProvider();

        protected string ScriptFileName { get; }

        protected ScriptDownloadBase(string fileName, ILogger<IScriptService> logger)
        {
            ScriptFileName = fileName;
            ScriptFilePath = Path.Combine(GetTempPath(), ScriptFileName);
            Logger = logger;
        }

        protected ScriptDownloadBase(string fileName, ILogger<IScriptService> logger, IAppInfoProvider infoProvider) : this(fileName, logger)
        {
            InfoProvider = infoProvider;
        }

        private string GetTempPath()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), GetTempDirectoryName());
            var di = Directory.CreateDirectory(tempDir);
            return di.FullName;
        }

        private string GetUserAgent() {
            if (InfoProvider != null && InfoProvider.GetAppInfo() is var appInfo && appInfo.Name != null)
            {
                return $"{appInfo.Name}/{appInfo.Version}";
            }
            throw new Exception("User agent could not be determined!");
        }

        private string GetTempDirectoryName() {
            if (InfoProvider != null && InfoProvider.GetAppInfo() is var appInfo && appInfo.Name != null) {
                return appInfo.Name;
            }
            return "build";
        }

        protected async Task DownloadScript(string url)
        {
            Logger.LogDebug($"Downloading new build file: '{ScriptFileName}'");
            using var client = GetClient();
            var response = await client.GetAsync(url);
            await using var fs = new FileStream(
                ScriptFilePath,
                FileMode.Create);
            await response.Content.CopyToAsync(fs);
        }

        private HttpClient GetClient()
        {
            UserAgent ??= GetUserAgent();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            return client;
        }
    }
}