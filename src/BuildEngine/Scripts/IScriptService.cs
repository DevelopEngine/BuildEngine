using System.IO;
using System.Threading.Tasks;

namespace BuildEngine.Scripts
{
    public interface IScriptService {
        public Task<string> GetScriptPath();
        public Task<IBuildRunner> GetRunContext(string targetPath);
    }

    //this is basically a replacement for the buildcontextfactory (hopefully)
    // although the build context factory could be injected here?
    // this would be where we inject script services (if there)
    // GetBuild should "init" a build env and return a service for that env
    //also means one build service provider could select what sort of service to return
    public interface IBuildServiceProvider
    {
        public Task<IBuildService> GetBuild(string? buildId);
    }

    //this is what the "consuming" components are going to interact with most
    // especially since the context is here
    // this tightly couples the context and runner but I think that makes sense anyway
    public interface IBuildService
    {
        IBuildContext BuildContext { get; }
        public (bool Success, FileSystemInfo Output) RunBuild(string targetFileName);
    }
}