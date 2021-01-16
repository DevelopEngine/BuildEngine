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
    public abstract class ScriptDownloadBase
    {
        public string UserAgent { get; protected set; }
        protected virtual string ScriptFilePath { get; }
        private readonly ILogger<IScriptService> _logger;
        private (string Name, string Version) _appName;
        private IAppInfoProvider _infoProvider { get; set; } = new EntryAppInfoProvider();

        protected virtual string ScriptFileName { get; }

        protected ScriptDownloadBase(string fileName, ILogger<IScriptService> logger)
        {
            ScriptFileName = fileName;
            ScriptFilePath = Path.Combine(GetTempPath(), ScriptFileName);
            _logger = logger;
        }

        protected ScriptDownloadBase(string fileName, ILogger<IScriptService> logger, IAppInfoProvider infoProvider) : this(fileName, logger)
        {
            _infoProvider = infoProvider;
        }

        private string GetTempPath()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), _appName.Name ?? "build");
            var di = Directory.CreateDirectory(tempDir);
            return di.FullName;
        }

        private string GetUserAgent() {
            if (_infoProvider != null && _infoProvider.GetAppInfo() is var appInfo && appInfo.Name != null)
            {
                return $"{appInfo.Name}/{appInfo.Version}";
            }
            throw new Exception("User agent could not be determined!");
        }

        protected async Task DownloadScript(string url)
        {
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

        private HttpClient GetClient()
        {
            UserAgent ??= GetUserAgent();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            return client;
        }
    }
}