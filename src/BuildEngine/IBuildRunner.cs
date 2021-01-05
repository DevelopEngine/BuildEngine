using System;
using System.IO;
using BuildEngine.Scripts;

namespace BuildEngine {
    public interface IBuildRunner {
        public (bool Success, FileInfo Output) RunBuild(BuildScript script, string targetFileName);
    }
}