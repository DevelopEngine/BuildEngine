using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BuildEngine.Scripts {
    public interface IScriptService {
        public Task<string> GetScriptPath();
        public Task<BuildScript> GetScriptContext(string targetPath);
    }
    public abstract class ScriptDownloadBase {
        public string UserAgent { get; protected set; }
        protected virtual string ScriptFilePath {get;}
        private readonly ILogger<IScriptService> _logger;
        private (string Name, string Version) _appName;
        private IAppInfoProvider _infoProvider;

        protected virtual string ScriptFileName {get;}

        protected ScriptDownloadBase(string fileName, ILogger<IScriptService> logger) {
            ScriptFileName = fileName;
            ScriptFilePath = Path.Combine(GetTempPath(), ScriptFileName);
            _logger = logger;
        }

        protected ScriptDownloadBase(string fileName, ILogger<IScriptService> logger, IAppInfoProvider infoProvider) : this(fileName, logger) {
            _infoProvider = infoProvider;
            if (_infoProvider != null && _infoProvider.GetAppInfo() is var appInfo && appInfo.Name != null) {
                UserAgent = $"{appInfo.Name}/{appInfo.Version}";
            }
        }

        [Obsolete("This has been replaced by an IAppInfoProvider")]
        private void ParseAppName(string appName) {
            AssemblyName GetEntryAssembly() {
                var assemblyName = Assembly.GetEntryAssembly().GetName();
                return assemblyName;
            }
            if (string.IsNullOrWhiteSpace(appName)) {
                var entry = GetEntryAssembly();
                _appName = (entry.Name, entry.Version.ToString(3));
            } else if (!appName.Contains("/")) {
                var entry = GetEntryAssembly();
                _appName = (appName, entry.Version.ToString(3));
            } else {
                var parts = appName.Split('/');
                _appName = (parts.First(), parts.Last());
            }
        }

        private string GetTempPath() {
            var tempDir = Path.Combine(Path.GetTempPath(), _appName.Name ?? "build");
            var di = Directory.CreateDirectory(tempDir);
            return di.FullName;
        }

        protected async Task DownloadScript(string url) {
            _logger.LogDebug($"Downloading new build file: '{ScriptFileName}'");
            using var client = GetClient();
            // var file = await client.GetStringAsync(url);
            // await File.WriteAllTextAsync(_scriptFilePath, file);
            var response = await client.GetAsync(url);
            using var fs = new FileStream(
                ScriptFilePath, 
                FileMode.Create);
            await response.Content.CopyToAsync(fs);
        }

        private HttpClient GetClient() {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            return client;
        }
    }
}