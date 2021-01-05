using System.Reflection;

namespace BuildEngine {
    public interface IAppInfoProvider {
        public (string Name, string Version) GetAppInfo();
    }
    internal class EntryAppInfoProvider : IAppInfoProvider {
        private static AssemblyName GetEntryAssembly() {
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            return assemblyName;
        }

        public (string Name, string Version) GetAppInfo() {
            var entry = GetEntryAssembly();
            return (entry.Name, entry.Version.ToString(3));
        }
    }
}