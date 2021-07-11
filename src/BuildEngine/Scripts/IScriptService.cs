using System.IO;
using System.Threading.Tasks;

namespace BuildEngine.Scripts
{
    public interface IScriptService {
        public Task<string?> GetScriptPath();
        public Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(string targetPath, string targetFileName);
    }
}