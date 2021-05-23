using System;
using System.IO;

namespace BuildEngine {
    
    public interface IBuildRunner : IDisposable {
        public (bool Success, FileSystemInfo Output) RunBuild(string targetFileName);
        //public string? WorkingDirectory { get; }
    }
}