using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BuildEngine.Builder {
    public class BuilderService {
        private readonly ILogger<BuilderService> _logger;
        private readonly BuildContextFactory _contextFactory;

        public BuilderService(ILogger<BuilderService> logger, BuildContextFactory contextFactory) {
            _logger = logger;
            _contextFactory = contextFactory;
        }


        public async Task<FileInfo> RunBuild(string objName, string targetFileName, params BuildInstruction[] contextTargets) {
            var targets = contextTargets.ToList();
            using (var ctx = await _contextFactory.Create(objName))
            {
                foreach (var target in targets)
                {
                    var linked = ctx.AddFromInstruction(target);
                    if (!linked) {
                        _logger.LogError("[bold red]Failed to add folders to context directory![/]");
                        return null;
                    }
                }
                var buildResult = ctx.BuildScript.RunBuild(targetFileName);
                if (buildResult.Success) {
                    _logger.LogInformation($"[bold green]Success![/] Files for {objName} successfully packed from {targets.Sum(t => t.SourceFiles.Count)} files (in {targets.Count} targets)");
                    var tempFile = Path.GetTempFileName();
                    switch (buildResult.Output)
                    {
                        case FileInfo fileInfo:
                            fileInfo.CopyTo(tempFile, true);
                            break;
                        case DirectoryInfo dirInfo:
                            System.IO.Compression.ZipFile.CreateFromDirectory(dirInfo.FullName, tempFile);
                            break;
                    }
                    return new FileInfo(tempFile);
                } else {
                    _logger.LogInformation($"[bold white on red]Failed![/] Files from {Directory.GetParent(contextTargets.First().SourceGroup)} not packed successfully. Continuing...");
                    return null;
                }
            }
        }
    }
}