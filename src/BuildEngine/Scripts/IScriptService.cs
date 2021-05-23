using System.Threading.Tasks;

namespace BuildEngine.Scripts
{
    public interface IScriptService {
        public Task<string> GetScriptPath();
        public Task<IBuildRunner> GetRunContext(string targetPath);
    }
}