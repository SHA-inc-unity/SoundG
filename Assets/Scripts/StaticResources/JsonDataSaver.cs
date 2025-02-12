using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonDataSaver
{
    private static readonly string FilePath = "Assets/Resources/data.json";

    public static void Save(Dictionary<string, List<TimeValuePair>> data)
    {
        var wrapper = new Wrapper { items = new List<DataEntry>() };
        foreach (var kv in data)
        {
            wrapper.items.Add(new DataEntry { key = kv.Key, values = kv.Value });
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(FilePath, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh(); // Обновляет файлы в Unity Editor
#endif
    }

    public static Dictionary<string, List<TimeValuePair>> Load()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("data");
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found in Resources!");
            return new Dictionary<string, List<TimeValuePair>>();
        }

        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(jsonFile.text);
        var dict = new Dictionary<string, List<TimeValuePair>>();
        foreach (var entry in wrapper.items)
        {
            dict[entry.key] = entry.values;
        }
        return dict;
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<DataEntry> items;
    }

    [System.Serializable]
    private class DataEntry
    {
        public string key;
        public List<TimeValuePair> values;
    }

    [System.Serializable]
    public class TimeValuePair
    {
        public float time;
        public int value;

        public TimeValuePair(float time, int value)
        {
            this.time = time;
            this.value = value;
        }
    }
}
