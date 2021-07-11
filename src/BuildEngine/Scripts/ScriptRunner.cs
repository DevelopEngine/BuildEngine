using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ExecEngine;

namespace BuildEngine.Scripts
{
    public abstract class ScriptRunner : CommandRunner, IScriptService, IDisposable
    {
        protected FileInfo? SourceScriptPath { get; private set; }

        public ScriptRunner(FileInfo scriptPath, params string[] defaultArgs) : base(scriptPath.Name, defaultArgs) {
            SourceScriptPath = scriptPath;
        }

        public ScriptRunner(RemoteScript remoteScript, string targetPath, params string[] defaultArgs) : base(
            targetPath, defaultArgs) {
            DownloadService = new ScriptDownloadService(remoteScript);
        }

        private ScriptDownloadService? DownloadService { get; }

        public new void Dispose() {
            base.Dispose();
            SourceScriptPath?.Delete();
        }

        // public abstract (bool Success, FileInfo Output) RunBuild(string targetFileName);
        protected abstract IEnumerable<string> GetBuildArgs(FileInfo scriptPath, string targetFileName);

        public virtual async Task<string?> GetScriptPath() {
            if (DownloadService != null && SourceScriptPath is not {Exists: true}) {
                var scriptPath = await DownloadService.GetScriptPath();
                SourceScriptPath = new FileInfo(scriptPath);
                return scriptPath;
            }

            return SourceScriptPath?.FullName;
        }

        public async Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(string workingPath,
            string targetFileName) {
            var sourceFile = await GetScriptPath();
            if (sourceFile != null && SourceScriptPath == null) {
                SourceScriptPath = new FileInfo(sourceFile);
            }

            if (SourceScriptPath is not {Exists: true}) {
                throw new InvalidOperationException("Script file not found!");
            }

            var targetFile = Path.Combine(workingPath, SourceScriptPath.Name);
            File.Copy(SourceScriptPath.FullName, targetFile, true);
            var args = GetBuildArgs(new FileInfo(targetFile), targetFileName);
            var output = SetWorkingDirectory(workingPath).RunCommand(args);
            return (output.ExitCode == 0,
                new FileInfo(Path.IsPathRooted(targetFileName)
                    ? targetFileName
                    : Path.Combine(workingPath, targetFileName)));
        }
    }
}