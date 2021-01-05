using System.Collections.Generic;
using System.Linq;
using BuildEngine.Builder;

namespace BuildEngine {
    public static class BuildExtensions {
        public static bool AddFromInstruction(this BuildContext ctx, BuildInstruction instruction) {
            var results = new List<bool>();
            foreach (var file in instruction.SourceFiles)
            {
                results.Add(ctx.AddFile(instruction.TargetPath, file));
            }
            return results.All(r => r == true);
        }
    }
}