using System;

namespace BuildEngine.Scripts
{
    public record RemoteScript
    {
        public RemoteScript(string fileName, string fileSourceUri) {
            FileName = fileName;
            FileSourceUri = fileSourceUri;
        }

        public string FileName { get; }
        public string FileSourceUri { get; }
        public string? FileSourceHash { get; init; }
        public Func<bool>? Confirmation { get; init; }
    }
}