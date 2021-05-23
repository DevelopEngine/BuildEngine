using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using BuildEngine.Scripts;
using Microsoft.Extensions.Logging;

namespace BuildEngine {
    public class BuildContextFactory : BuildContextFactory<BuildContext>
    {
        [AllowNull]
        private readonly IScriptService? _scriptService;
        private readonly ILogger<BuildContext>? _logger;
        private IAppInfoProvider? InfoProvider { get; set; } = new EntryAppInfoProvider();

        public BuildContextFactory(ILogger<BuildContext> logger) {
            _logger = logger;
        }

        public BuildContextFactory(IScriptService scriptService, ILogger<BuildContext> logger) : this(logger)
        {
            _scriptService = scriptService;
        }

        public BuildContextFactory(ILogger<BuildContext> logger, IAppInfoProvider infoProvider) : this(logger) {
            InfoProvider = infoProvider;
        }

        public BuildContextFactory(IScriptService scriptService, ILogger<BuildContext> logger, IAppInfoProvider infoProvider) : this(logger, infoProvider) {
            _scriptService = scriptService;
        }

        public override async Task<BuildContext> CreateContext([AllowNull]string contextName) {
            var buildId = Guid.NewGuid();
            var sourceId = InfoProvider != null
                ? InfoProvider.GetAppInfo().Name
                : $"build-{buildId:N}";
            var targetPath = new DirectoryInfo(Path.Combine(Path.GetTempPath(), sourceId, contextName ?? buildId.ToString()));
            targetPath.Create();
            
            if (_scriptService != null) {
                var scriptCtx = await _scriptService.GetRunContext(targetPath.FullName);
                // var scriptContext = _scriptService == null ? null : await _scriptService.GetRunContext(targetPath.FullName);
                return new BuildContext(scriptCtx, targetPath, buildId.ToString(), _logger);
            }
            return new BuildContext(null, targetPath, buildId.ToString(), _logger);
            
        }

    }
}