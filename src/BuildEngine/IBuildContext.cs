using System;
using System.IO;

namespace BuildEngine
{
    public interface IBuildContext : IDisposable
    {
        bool AddFolder(string relPath, DirectoryInfo sourceDir, string fileFilter = "*");
        bool AddFile(string relPath, FileInfo sourceFile);
        // bool AddFolder(string relPath, DirectoryInfo sourceDir, Func<FileInfo, bool> fileFilter);
    }
}