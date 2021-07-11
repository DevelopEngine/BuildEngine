using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace BuildEngine
{
    public abstract class BuildService<TContext> : IBuildService where TContext : IBuildContext
    {
        protected BuildService(TContext context) {
            Context = context;
        }

        protected BuildService(Func<TContext> contextFactory) {
            Context = contextFactory.Invoke();
        }

        protected BuildService<TContext> UseContext(TContext context) {
            Context = context;
            return this;
        }
        
        public IBuildContext BuildContext => Context;
        public abstract Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(string targetFileName);

        protected TContext Context { get; private set; }
        
    }
}