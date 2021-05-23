using System.IO;
using ExecEngine;

namespace BuildEngine.Scripts {
    public abstract class ScriptRunner : CommandRunner, IBuildRunner {
        protected readonly FileInfo TempPath;
        protected readonly string[] DefaultArgs;

        public string CurrentPath => TempPath.FullName;

        public ScriptRunner(string targetPath, params string[] defaultArgs) : base(targetPath, defaultArgs)
        {
            TempPath = new FileInfo(targetPath);
            DefaultArgs = defaultArgs;
        }

        public new void Dispose() {
            base.Dispose();
            File.Delete(TempPath.FullName);
        }
        
        // public abstract (bool Success, FileInfo Output) RunBuild(string targetFileName);
        public abstract (bool Success, FileSystemInfo Output) RunBuild(string targetFileName);
    }
}