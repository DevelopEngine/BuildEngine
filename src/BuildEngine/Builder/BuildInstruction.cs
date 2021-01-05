using System.Collections.Generic;
using System.IO;

namespace BuildEngine.Builder {
    public abstract class BuildInstruction {
        public string TargetPath {get;set;}
        // public string PackGroup {get;set;}
        public SourceGroup SourceGroup {get;set;}
        public List<FileInfo> SourceFiles {get;set;} = new List<FileInfo>();
        public virtual string GetOutputName(string separator = "_") => null;
    }
}