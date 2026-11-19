using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace FnPack.MSBuild;

public sealed class SetManifestVersion : Task
{
    [Required]
    public string ManifestPath { get; set; } = string.Empty;

    [Required]
    public string Version { get; set; } = string.Empty;

    public override bool Execute()
    {
        var text = File.ReadAllText(ManifestPath);
        var replaced = Regex.Replace(text, @"(?m)^version([ \t]*)=[^\r\n]*", "version$1= " + Version, RegexOptions.None, TimeSpan.FromSeconds(1));
        File.WriteAllText(ManifestPath, replaced);

        return true;
    }
}
