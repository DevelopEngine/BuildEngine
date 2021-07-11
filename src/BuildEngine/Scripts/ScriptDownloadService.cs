using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BuildEngine.Scripts {
    public class ScriptDownloadService
    {
        public string? UserAgent { get; protected set; }
        protected string ScriptFilePath { get; }
        protected readonly ILogger<IScriptService>? Logger;
        private IAppInfoProvider? InfoProvider { get; init; } = new EntryAppInfoProvider();
        public RemoteScript Script { get; }

        public ScriptDownloadService(RemoteScript script, ILogger<IScriptService>? logger = null)
        {
            // ScriptFileName = script.FileName;
            Script = script;
            ScriptFilePath = Path.Combine(GetTempPath(), script.FileName);
            Logger = logger;
        }

        public ScriptDownloadService(RemoteScript script, ILogger<IScriptService> logger, IAppInfoProvider infoProvider) : this(script, logger)
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
            Logger?.LogDebug($"Downloading new build file: '{Script.FileName}'");
            using var client = GetClient();
            var response = await client.GetAsync(url);
            await using var fs = new FileStream(
                ScriptFilePath,
                FileMode.Create);
            await response.Content.CopyToAsync(fs);
        }
        
        public virtual async Task<string> GetScriptPath() {
            
            var fi = new FileInfo(ScriptFilePath);
            if (fi.Exists && fi.Length > 0 && (string.IsNullOrWhiteSpace(Script?.FileSourceHash) || fi.CalculateMD5() == Script.FileSourceHash)) {
                //check if it matches
                return fi.FullName;
            } else {
                if (Script.Confirmation != null) {
                    var result = Script.Confirmation();
                    if (!result) {
                        throw new Exception("Aborted download of script file!");
                    }
                }
                await DownloadScript(Script.FileSourceUri);
                return fi.FullName;
            }
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