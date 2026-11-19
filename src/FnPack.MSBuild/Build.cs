using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace FnPack.MSBuild;

public sealed class Build : Task
{
    [Required]
    public string AppDirectory { get; set; } = string.Empty;

    [Required]
    public string OutputDirectory { get; set; } = string.Empty;

    [Required]
    public string ToolsDirectory { get; set; } = string.Empty;

    public override bool Execute()
    {
        var bin = ResolveBinary();
        if (bin is null)
        {
            return false;
        }

        Directory.CreateDirectory(OutputDirectory);
        var psi = new ProcessStartInfo(bin, $"build -d \"{AppDirectory}\"")
        {
            WorkingDirectory = OutputDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using var process = Process.Start(psi);
        process.OutputDataReceived += (_, e) => LogLine(e.Data);
        process.ErrorDataReceived += (_, e) => LogLine(e.Data);
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            Log.LogError("fnpack 退出码 {0}", process.ExitCode);

            return false;
        }

        return true;
    }

    private string? ResolveBinary()
    {
        string? os = null;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            os = "win";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            os = "linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            os = "osx";
        }
        var arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            Architecture.X86 => "x86",
            _ => null,
        };
        if (string.IsNullOrEmpty(os) || string.IsNullOrEmpty(arch))
        {
            Log.LogError("无法识别当前宿主平台：{0} {1}", os ?? "?", arch ?? "?");

            return null;
        }

        var name = os == "win" ? "fnpack.exe" : "fnpack";
        var path = Path.Combine(ToolsDirectory, $"{os}-{arch}", name);
        if (!File.Exists(path))
        {
            Log.LogError("fnpack 缺少当前平台二进制：{0}", path);

            return null;
        }

        return path;
    }

    private void LogLine(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }

        Log.LogMessage(MessageImportance.High, line);
    }
}
