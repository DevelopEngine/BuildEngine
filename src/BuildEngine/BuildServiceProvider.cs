using System.Threading.Tasks;

namespace BuildEngine
{
    public abstract class BuildServiceProvider<TService> : IBuildServiceProvider where TService : IBuildService
    {
        public abstract Task<TService> CreateService(string? contextName);

        public async Task<IBuildService> GetBuild(string? buildId) =>
            await ((CreateService(buildId) as Task<IBuildService>)!);
    }
}