using System.IO;
using System.Diagnostics;
using UnityEditor;

internal sealed class MenuGit
{
    private const string SSH_CORE_GIT = "git@github.com:LeeSJ7607/Core.git";
    private const string HTTPS_CORE_GIT = "https://github.com/LeeSJ7607/Core.git";
    private const string CORE_FOLDER_PATH = "Assets/Scripts/Core";
    
    [MenuItem("Custom/Menu/Git/Clone Core")]
    private static void CloneCore()
    {
        if (!TryCloneCore())
        {
            return;
        }

        var process = new Process();
        if (!TryExecuteCloneCore(process))
        {
            return;
        }
        
        EditorUtility.DisplayDialog("Git Clone Success", "Core Repository Clone Success", "OK");
        AssetDatabase.Refresh();
        process.Close();
    }

    private static bool TryCloneCore()
    {
        if (Directory.Exists(CORE_FOLDER_PATH))
        {
            EditorUtility.DisplayDialog("Git Clone Failed", $"Already Exists.\nPath: {CORE_FOLDER_PATH}", "OK");
            return false;
        }

        Directory.CreateDirectory("Assets/Scripts/Core");
        return true;
    }

    private static bool TryExecuteCloneCore(Process process)
    {
        process.StartInfo = new ProcessStartInfo()
        {
            FileName = "git",
            //Arguments = $"clone {SSH_CORE_GIT} {CORE_FOLDER_PATH}",
            Arguments = $"clone {HTTPS_CORE_GIT} {CORE_FOLDER_PATH}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        process.Start();
        process.WaitForExit();
        
        var stdoutx = process.StandardOutput.ReadToEnd();
        var stderrx = process.StandardError.ReadToEnd();

        if (process.ExitCode == 0)
        {
            return true;
        }

        var msg = $"The following error occurred during git clone.\n{stdoutx}\n{stderrx}";
        EditorUtility.DisplayDialog("Git Clone Failed", msg, "OK");
        process.Close();
        return false;
    }
}