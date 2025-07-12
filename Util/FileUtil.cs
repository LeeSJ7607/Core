using System;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class FileUtil
{
    public static void SaveAsJson(string path, object obj)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var fullPath = $"{path}/{obj.GetType().Name}";
        var json = JsonConvert.SerializeObject(obj, CreateJsonSettings());
        File.WriteAllText(fullPath, json);
    }

    public static object LoadFromJson(string path, Type type)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"{nameof(FileUtil)} {nameof(LoadFromJson)} Failed. File does not exist: {path}");
            return null;
        }
        
        var text = File.ReadAllText(path, Encoding.UTF8);
        return JsonConvert.DeserializeObject(text, type, CreateJsonSettings());
    }

    public static void Delete(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"{nameof(FileUtil)} {nameof(Delete)} Failed. File does not exist: {path}");
            return;
        }

        File.Delete(path);
    }

    private static JsonSerializerSettings CreateJsonSettings()
    {
        var resolver = new DefaultContractResolver();
        
#pragma warning disable 618
        resolver.DefaultMembersSearchFlags |= BindingFlags.NonPublic;
#pragma warning restore 618
        
        return new JsonSerializerSettings()
        {
            ContractResolver = resolver
        };
    }
}