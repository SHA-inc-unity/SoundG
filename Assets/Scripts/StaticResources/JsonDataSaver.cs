using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonDataSaver
{
    private static readonly string FilePathBitList = "Assets/Resources/data.json";
    private static readonly string FilePathOwnerData = "owned.json";

    public static void SaveBitList(Dictionary<string, List<TimeValuePair>> data)
    {
        var wrapper = new WrapperDataEntry { items = new List<DataEntry>() };
        foreach (var kv in data)
        {
            wrapper.items.Add(new DataEntry { key = kv.Key, values = kv.Value });
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(FilePathBitList, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh(); // Обновляет файлы в Unity Editor
#endif
    }

    public static Dictionary<string, List<TimeValuePair>> LoadBitList()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("data");
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found in Resources!");
            return new Dictionary<string, List<TimeValuePair>>();
        }

        WrapperDataEntry wrapper = JsonUtility.FromJson<WrapperDataEntry>(jsonFile.text);
        var dict = new Dictionary<string, List<TimeValuePair>>();
        foreach (var entry in wrapper.items)
        {
            dict[entry.key] = entry.values;
        }
        return dict;
    }


    public static void SaveOwnerData(List<(OwnerData, string)> ownerData)
    {
        string path = Path.Combine(Application.persistentDataPath, FilePathOwnerData);

        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fileStream))
            {
                List<OwnerDataWrapper> wrapperList = new List<OwnerDataWrapper>();
                foreach (var (owner, song) in ownerData)
                {
                    wrapperList.Add(new OwnerDataWrapper { ownerData = owner, songName = song });
                }

                string json = JsonUtility.ToJson(new WrapperOwnerData { list = wrapperList });
                writer.Write(json);
            }
            Debug.Log($"Данные успешно сохранены в: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при сохранении данных: {ex.Message}");
        }
    }

    public static List<(OwnerData, string)> LoadOwnerData()
    {
        string path = Path.Combine(Application.persistentDataPath, FilePathOwnerData);

        if (!File.Exists(path))
        {
            Debug.LogWarning("Файл данных не найден.");
            return new List<(OwnerData, string)>();
        }

        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                string json = reader.ReadString();
                var wrapper = JsonUtility.FromJson<WrapperOwnerData>(json);

                List<(OwnerData, string)> result = new List<(OwnerData, string)>();
                foreach (var item in wrapper.list)
                {
                    result.Add((item.ownerData, item.songName));
                }

                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при загрузке данных: {ex.Message}");
            return new List<(OwnerData, string)>();
        }
    }



    [Serializable]
    private class WrapperOwnerData
    {
        public List<OwnerDataWrapper> list;
    }

    [Serializable]
    private class OwnerDataWrapper
    {
        public OwnerData ownerData;
        public string songName;
    }

    [System.Serializable]
    private class WrapperDataEntry
    {
        public List<DataEntry> items;
    }

    [System.Serializable]
    private class DataEntry
    {
        public string key;
        public int speed;
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
