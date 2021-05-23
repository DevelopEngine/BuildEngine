using System.Collections.Generic;
using System.IO;

namespace BuildEngine.Builder {
    public abstract class BuildInstruction {
        public virtual string? TargetPath {get; init;}

        // public string PackGroup {get;set;}
        public SourceGroup? SourceGroup {get;set;}
        public virtual List<FileInfo> SourceFiles {get;set;} = new List<FileInfo>();
        public virtual string? GetOutputName(string separator = "_") => null;
    }
}