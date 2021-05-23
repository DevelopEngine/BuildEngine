using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuildEngine {
    public static class CoreExtensions {
        internal static string ToArgument(this string path) {
            return path.Contains(' ')
                ? $"\"{path}\""
                : path;
        }

        internal static IEnumerable<string> ToArguments(this IEnumerable<string> paths) {
            return paths.Select(p => p.ToArgument());
        }
        
        public static void CopyTo(this DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory, string fileFilter) {

            CopyAll(sourceDirectory, targetDirectory, fileFilter);
        }

        public static void CopyTo(this DirectoryInfo sourceDirectory, string targetDirectory, string fileFilter) {

            CopyAll(sourceDirectory, new DirectoryInfo(targetDirectory), fileFilter);
        }

        public static void CopyTo(this DirectoryInfo sourceDirectory, string targetDirectory, Func<FileInfo, bool> fileFilter) {

            CopyAllFiltered(sourceDirectory, new DirectoryInfo(targetDirectory), fileFilter);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target, string? filter) {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles(filter ?? "*")) {
                // System.Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories()) {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, filter);
            }
        }

        private static void CopyAllFiltered(DirectoryInfo source, DirectoryInfo target, Func<FileInfo, bool> filterFunc) {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.EnumerateFiles().Where(filterFunc)) {
                // System.Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories()) {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAllFiltered(diSourceSubDir, nextTargetSubDir, filterFunc);
            }
        }

        public static string MakeSafe(this string input, bool removeChars = false) {
            return removeChars
                ? Path.GetInvalidFileNameChars().Aggregate(input, (current, c) => current.IndexOf(c) == -1 ? current : current.Remove(current.IndexOf(c), 1))
                : Path.GetInvalidFileNameChars().Aggregate(input, (current, c) => current.Replace(c, '-'));
        }
    }
}