using System.IO;
using System.Diagnostics;
using UnityEditor;

public sealed class MenuGit
{
    private const string SSH_CORE_GIT = "git@github.com:LeeSJ7607/Core.git";
    private const string HTTPS_CORE_GIT = "https://github.com/LeeSJ7607/Core.git";
    private const string CORE_FOLDER_PATH = "Assets/Scripts/Cor1";
    
    [MenuItem("Custom/Menu/Git/Clone Core")]
    public static void CloneCore()
    {
        if (Directory.Exists(CORE_FOLDER_PATH))
        {
            var msg = $"Already Exists.\nPath: {CORE_FOLDER_PATH}";
            EditorUtility.DisplayDialog("Git Clone Failed", msg, "OK");
            return;
        }
        
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = $"clone {SSH_CORE_GIT} {CORE_FOLDER_PATH}",
                //Arguments = $"clone {HTTPS_CORE_GIT} {CORE_FOLDER_PATH}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        process.WaitForExit();
        
        var stdoutx = process.StandardOutput.ReadToEnd();
        var stderrx = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0)
        {
            var msg = $"The following error occurred during git clone.\n{stdoutx}\n{stderrx}";
            EditorUtility.DisplayDialog("Git Clone Failed", msg, "OK");
            process.Close();
            return;
        }
        
        EditorUtility.DisplayDialog("Git Clone Success", "Core Repository Clone Success", "OK");
        AssetDatabase.Refresh();
        process.Close();
    }
}