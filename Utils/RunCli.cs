namespace payto;
public class RunCli
{
    public static string default_workDir = Environment.CurrentDirectory;

    /// <summary>
    /// This will execute lightning-cli (must be in path) with the "command" 
    /// </summary>
    /// <param name="command">Everything after "lightning-cli" including arguments..</param>
    /// <returns>Returns everything that lightning-cli writes to output (both std & err)</returns>
    public static string ExecuteLightnigCli(string command, string? workDir = null)
    {
        return ExecuteLightnigCliCommand(command, workDir ?? default_workDir);
    }

    private static string ExecuteLightnigCliCommand(string arguments, string workDir)
    {
        string result = "";
        using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
        {
            proc.StartInfo.FileName = "lightning-cli";
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.WorkingDirectory = workDir ?? default_workDir;
            proc.Start();

            result += proc.StandardOutput.ReadToEnd();

            result += proc.StandardError.ReadToEnd();

            proc.WaitForExit();
        }
        return result;
    }

    /// <summary>
    /// Execute Linux commands - used for DNS resolution using dig
    /// </summary>
    public static string ExecuteCommand(string process, string arguments, string? workDir = null)
    {
        string result = "";
        using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
        {
            proc.StartInfo.FileName = process;
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.WorkingDirectory = workDir ?? default_workDir;
            proc.Start();

            result += proc.StandardOutput.ReadToEnd();

            result += proc.StandardError.ReadToEnd();

            proc.WaitForExit();
        }
        return result;
    }
}