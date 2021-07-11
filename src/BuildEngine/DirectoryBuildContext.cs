using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ExecEngine;
using Microsoft.Extensions.Logging;

namespace BuildEngine {
    public class DirectoryBuildContext : IBuildContext
    {
        public DirectoryInfo WorkingDirectory { get; }
        private readonly ILogger? _logger;
        public string Id { get; }

        internal DirectoryBuildContext(DirectoryInfo targetPath, string buildId, ILogger? logger) {
            Id = buildId;
            WorkingDirectory = targetPath;
            _logger = logger;
        }

        public bool AddFolder(string relPath, DirectoryInfo sourceDir, string fileFilter = "*") {
            var targetPath = Path.Combine(WorkingDirectory.FullName, relPath);
            sourceDir.CopyTo(targetPath, fileFilter);
            return Directory.Exists(targetPath);
        }

        public bool AddFile(string relPath, FileInfo sourceFile) {
            var targetPath = Path.Combine(WorkingDirectory.FullName, relPath);
            var targetFilePath = Path.Combine(targetPath, sourceFile.Name);
            if (!Directory.Exists(targetPath)) {
                Directory.CreateDirectory(targetPath);
            }
            sourceFile.CopyTo(targetFilePath, true);
            return File.Exists(targetFilePath);
        }

        public bool AddFolder(string relPath, DirectoryInfo sourceDir, Func<FileInfo, bool> fileFilter) {
            var targetPath = Path.Combine(WorkingDirectory.FullName, relPath);
            sourceDir.CopyTo(targetPath, fileFilter);
            return Directory.Exists(targetPath);
        }

        public bool LinkFolder(string relPath, DirectoryInfo sourceDir) {
            var linkPath = Path.Combine(WorkingDirectory.FullName, relPath);
            Directory.CreateDirectory(Path.GetDirectoryName(linkPath) ?? linkPath);
            // var result = CreateSymbolicLink(linkPath, sourceDir.FullName, SymbolicLink.Directory);
            var result = CreateLink(new DirectoryInfo(linkPath), sourceDir.FullName);
            if (!result) {
                _logger.LogWarning("Couldn't link folders, most likely due to missing permissions! Falling back to copying files...");
                AddFolder(relPath, sourceDir);
            }
            // FileHelpers.CreateSymbolicLink(linkPath, sourceDir.FullName, SymbolicLink.Directory);
            return Directory.Exists(linkPath);
        }

        private static bool CreateLink(FileSystemInfo linkPath, string targetName) {
            var cmdRunner = new CommandRunner("cmd.exe");
            cmdRunner.SetWorkingDirectory(Path.GetDirectoryName(linkPath.FullName));
            var result = cmdRunner.RunCommand(new[] { "/C mklink /D", linkPath.Name.ToArgument(), targetName.ToArgument()});
            return result.ExitCode == 0 && Directory.Exists(linkPath.FullName);
        }

        public void Dispose() {
            WorkingDirectory.Delete(true);
        }
    }
}