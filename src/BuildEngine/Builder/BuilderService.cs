using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BuildEngine.Builder {
    public class BuilderService {
        private readonly ILogger<BuilderService>? _logger;
        private readonly BuildContextFactory _contextFactory;

        public BuilderService(ILogger<BuilderService> logger, BuildContextFactory contextFactory) : this(contextFactory) {
            _logger = logger;
        }

        public BuilderService(BuildContextFactory contextFactory) {
            _contextFactory = contextFactory;
        }

        public async Task<FileInfo?> RunBuild(string objName, string targetFileName, params BuildInstruction[] contextTargets) {
            var targets = contextTargets.ToList();
            using var ctx = await _contextFactory.Create(objName);
            foreach (var target in targets)
            {
                var linked = ctx.AddFromInstruction(target);
                if (!linked) {
                    _logger.LogError("[bold red]Failed to add folders to context directory![/]");
                    return null;
                }
            }

            if (ctx.BuildRunner != null) {
                var (success, output) = ctx.BuildRunner.RunBuild(targetFileName);
                if (success) {
                    _logger?.LogInformation(
                        $"[bold green]Success![/] Files for {objName} successfully packed from {targets.Sum(t => t.SourceFiles.Count)} files (in {targets.Count} targets)");
                    var tempFile = Path.GetTempFileName();
                    switch (output) {
                        case FileInfo fileInfo:
                            fileInfo.CopyTo(tempFile, true);
                            break;
                        case DirectoryInfo dirInfo:
                            System.IO.Compression.ZipFile.CreateFromDirectory(dirInfo.FullName, tempFile);
                            break;
                    }

                    return new FileInfo(tempFile);
                }

                var sourceGroup = contextTargets.First().SourceGroup;
                _logger?.LogInformation(
                        $"[bold white on red]Failed![/] Files from {(sourceGroup == null ? "unknown group" : Directory.GetParent(sourceGroup))} not packed successfully. Continuing...");
                return null;
            }
            _logger?.LogInformation("No build action defined for context. Continuing.");
            return null;
        }
    }
}