namespace BuildEngine {
    public class SourceGroup {
        public SourceGroup() { }

        public SourceGroup(string raw) {
            RawValue = raw;
        }

        public SourceGroup(string raw, string name) : this(raw) {
            Name = name;
        }
        
        public string RawValue { get; private set; }
        public string Name { get; set; }
        
        public static implicit operator string(SourceGroup sg) {
            return sg.RawValue;
        }

        public static implicit operator SourceGroup(string s) {
            return new() {
                RawValue = s
            };
        }

        public override string ToString() {
            return RawValue;
        }

        public string GetName() {
            return string.IsNullOrWhiteSpace(Name)
                ? RawValue
                : Name;
        }
    }
}