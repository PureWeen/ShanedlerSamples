using System.Diagnostics;

namespace Maui.FixesAndWorkarounds;

public static partial class DeviceKeyboard
{
    static Task<bool> _getKeyboardNavigationEnabledTask;

    public static Task<bool> GetKeyboardNavigationEnabledAsync(CancellationToken token = default)
    {
        if (_getKeyboardNavigationEnabledTask == null || _getKeyboardNavigationEnabledTask.IsCompleted)
            _getKeyboardNavigationEnabledTask = GetKeyboardNavigationEnabledTask();

        return _getKeyboardNavigationEnabledTask;
    }

    static async Task<bool> GetKeyboardNavigationEnabledTask(CancellationToken token = default)
    {
        var command = "defaults read -g AppleKeyboardUIMode";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = new() { StartInfo = startInfo };

        string processOutput = null;
        string processError = null;

        void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
                return;

            processOutput = e.Data;
        }

        void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
                return;

            processError = e.Data.ToString();
        }

        process.OutputDataReceived += ProcessOutputDataReceived;
        process.ErrorDataReceived += ProcessErrorDataReceived;

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync(token);
        }
        finally
        {
            process.OutputDataReceived -= ProcessOutputDataReceived;
            process.ErrorDataReceived -= ProcessErrorDataReceived;
        }

        if (token.IsCancellationRequested)
            return true; // Default to true

        if (!string.IsNullOrWhiteSpace(processOutput) && int.TryParse(processOutput, out var result))
            return result == 2; // Keyboard navigation enabled

        // TODO: Review whether to return an actual enum with unknown type?

        return true; // Default to true
    }

    public static async Task SetKeyboardNavigationEnabledAsync(bool enabled, CancellationToken token = default)
    {
        var command = $"defaults write -g AppleKeyboardUIMode -int {(enabled ? 2 : 0)}";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = new() { StartInfo = startInfo };

        string processError = null;

        void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
                return;

            processError = e.Data.ToString();
        }

        process.ErrorDataReceived += ProcessErrorDataReceived;

        try
        {
            process.Start();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync(token);
        }
        finally
        {
            process.ErrorDataReceived -= ProcessErrorDataReceived;
        }

        // TODO: Review whether to throw exception on failure or return updated state
        if (!string.IsNullOrWhiteSpace(processError))
            throw new Exception("Error setting Keyboard navigation", new Exception(processError));
    }
}