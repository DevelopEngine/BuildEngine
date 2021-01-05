using System.Runtime.InteropServices;

namespace BuildEngine {
    public static class FileHelpers {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);
        
    }
    internal enum SymbolicLink {
        File = 0,
        Directory = 1
    }
}