using System.Linq;

namespace BuildEngine.Builder {
    public static class BuildExtensions {
        public static bool AddFromInstruction(this IBuildContext ctx, BuildInstruction instruction) {
            if (instruction.TargetPath != null) {
                var results = instruction.SourceFiles.Select(file => ctx.AddFile(instruction.TargetPath, file))
                    .ToList();
                return results.All(r => r);
            }
            return false;
        }
    }
}