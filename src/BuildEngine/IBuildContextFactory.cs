using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BuildEngine
{
    public interface IBuildContextFactory
    {
        Task<IBuildContext> Create([AllowNull]string contextName);
    }

    public abstract class BuildContextFactory<TContext> : IBuildContextFactory where TContext : IBuildContext
    {
        public abstract Task<TContext> CreateContext(string? contextName);


        public Task<IBuildContext> Create(string? contextName) =>
            (CreateContext(contextName) as Task<IBuildContext>)!;
    }
}