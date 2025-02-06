using System.IO;
using System.Linq;

public static class DirectoryUtil
{
    public static bool HasFile(string folderPath, string searchPattern)
    {
        return Directory.EnumerateFiles(folderPath, searchPattern).Any();
    }
}