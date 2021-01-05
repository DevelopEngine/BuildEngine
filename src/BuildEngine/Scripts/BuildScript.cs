using System;
using System.IO;

namespace BuildEngine.Scripts {
    public abstract class BuildScript : IDisposable {
        private readonly FileInfo _tempPath;
        private readonly string[] _defaultArgs;

        public string CurrentPath => _tempPath.FullName;

        public string WorkingDirectory => _tempPath.DirectoryName;

        public BuildScript(string targetPath, params string[] defaultArgs)
        {
            _tempPath = new FileInfo(targetPath);
            _defaultArgs = defaultArgs;
        }

        public void Dispose() {
            File.Delete(_tempPath.FullName);
        }
        
        public abstract (bool Success, FileInfo Output) RunBuild(string targetFileName);
    }
}