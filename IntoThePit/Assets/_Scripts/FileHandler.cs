using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

public static class FileHandler
{
    public static string ReadFile(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            if (fs != null)
            {
                StreamReader reader = new StreamReader(fs);
                return reader.ReadToEnd();
            }
            else
            {
                return "";
            }
        }
    }
    public static void WriteFile(string fileName, string content)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        FileStream fs = new FileStream(path, FileMode.Create);
        using(StreamWriter writer = new StreamWriter(fs))
        {
            writer.Write(content);
        }
    }

    public static void WriteJson<T>(string fileName, List<T> list)
    {
        string json = JsonHelper.ToJson(list.ToArray());
        WriteFile(fileName, json);
    }
    public static List<T> ReadJson<T>(string fileName)
    {
        string content = ReadFile(fileName);
        if(string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }
        else
        {
            List<T> res = JsonHelper.FromJson<T>(content).ToList();
            return res;
        }
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
