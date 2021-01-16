using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using BuildEngine.Scripts;
using Microsoft.Extensions.Logging;

namespace BuildEngine {
    public class BuildContextFactory {
        [AllowNull]
        private readonly IScriptService _scriptService;
        private readonly ILogger<BuildContext> _logger;
        private IAppInfoProvider _infoProvider { get; set; } = new EntryAppInfoProvider();

        public BuildContextFactory(ILogger<BuildContext> logger) {
            _logger = logger;
        }

        public BuildContextFactory(IScriptService scriptService, ILogger<BuildContext> logger) : this(logger)
        {
            _scriptService = scriptService;
        }

        public BuildContextFactory(ILogger<BuildContext> logger, IAppInfoProvider infoProvider) : this(logger) {
            _infoProvider = infoProvider;
        }

        public BuildContextFactory(IScriptService scriptService, ILogger<BuildContext> logger, IAppInfoProvider infoProvider) : this(logger, infoProvider) {
            _scriptService = scriptService;
        }
        
        public async Task<BuildContext> Create([AllowNull]string contextName) {
            var buildId = Guid.NewGuid();
            var sourceId = _infoProvider != null
                ? _infoProvider.GetAppInfo().Name
                : $"build-{buildId:N}";
            var targetPath = new DirectoryInfo(Path.Combine(Path.GetTempPath(), sourceId, contextName ?? buildId.ToString()));
            targetPath.Create();
            
            if (_scriptService != null) {
                var scriptContext = await _scriptService?.GetScriptContext(targetPath.FullName);
                return new BuildContext(scriptContext, targetPath, buildId.ToString(), _logger);
            }
            return new BuildContext(null, targetPath, buildId.ToString(), _logger);
            
        }
    }
}