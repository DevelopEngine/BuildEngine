using System.IO;
using System.Threading.Tasks;

namespace BuildEngine
{
    public interface IBuildService
    {
        IBuildContext BuildContext { get; }
        public Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(string targetFileName);
    }
}