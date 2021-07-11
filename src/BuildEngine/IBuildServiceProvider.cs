using System.Threading.Tasks;

namespace BuildEngine
{
    //this is basically a replacement for the buildcontextfactory (hopefully)
    // although the build context factory could be injected here?
    // this would be where we inject script services (if there)
    // GetBuild should "init" a build env and return a service for that env
    //also means one build service provider could select what sort of service to return
    public interface IBuildServiceProvider
    {
        public Task<IBuildService> GetBuild(string? buildId);
    }
}