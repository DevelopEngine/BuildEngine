using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using BuildEngine.Scripts;
using Microsoft.Extensions.Logging;

namespace BuildEngine {
    public class DirectoryBuildContextFactory
    {
        private readonly ILogger<DirectoryBuildContext>? _logger;
        private IAppInfoProvider? InfoProvider { get; set; } = new EntryAppInfoProvider();

        public DirectoryBuildContextFactory(ILogger<DirectoryBuildContext> logger) {
            _logger = logger;
        }

        public DirectoryBuildContextFactory(ILogger<DirectoryBuildContext> logger, IAppInfoProvider infoProvider) : this(logger) {
            InfoProvider = infoProvider;
        }

        public DirectoryBuildContext CreateContext([AllowNull]string? contextName) {
            var buildId = Guid.NewGuid();
            var sourceId = InfoProvider != null
                ? InfoProvider.GetAppInfo().Name
                : $"build-{buildId:N}";
            var targetPath = new DirectoryInfo(Path.Combine(Path.GetTempPath(), sourceId, contextName ?? buildId.ToString()));
            targetPath.Create();
            
            return new DirectoryBuildContext(targetPath, buildId.ToString(), _logger);
        }

    }
}