using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using System.Collections.Generic;
using static JsonDataSaver;

public static class MuzPackSaver
{
    private static string localPath = Application.persistentDataPath;

    public static void SaveMuzPack(SoundData soundData)
    {
        string fileName = soundData.Name + ".muzpack";
        string savePath = Path.Combine(Application.persistentDataPath, fileName);

        byte[] imageBytes = soundData.Image.texture.EncodeToPNG();
        byte[] audioBytes = ConvertAudioClipToBytes(soundData.Sound);
        string bitsJson = JsonUtility.ToJson(new BitsWrapper(soundData.Bits));
        string ownerJson = JsonUtility.ToJson(soundData.Owner);

        using (FileStream fs = new FileStream(savePath, FileMode.Create))
        using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Create))
        {
            AddTextToArchive(archive, "Name.txt", soundData.Name);
            AddTextToArchive(archive, "BPM.txt", soundData.BPM.ToString());
            AddTextToArchive(archive, "Bits.json", bitsJson);
            AddTextToArchive(archive, "Owner.json", ownerJson);

            AddBytesToArchive(archive, "Image.png", imageBytes);
            AddBytesToArchive(archive, "Sound.wav", audioBytes);
        }

        Debug.Log("Saved: " + savePath);
    }

    private static void AddTextToArchive(ZipArchive archive, string entryName, string text)
    {
        ZipArchiveEntry entry = archive.CreateEntry(entryName);
        using (StreamWriter writer = new StreamWriter(entry.Open()))
        {
            writer.Write(text);
        }
    }

    private static void AddBytesToArchive(ZipArchive archive, string entryName, byte[] data)
    {
        ZipArchiveEntry entry = archive.CreateEntry(entryName);
        using (Stream entryStream = entry.Open())
        {
            entryStream.Write(data, 0, data.Length);
        }
    }

    private static byte[] ConvertAudioClipToBytes(AudioClip clip)
    {
        if (clip == null) return new byte[0];
        MemoryStream stream = new MemoryStream();
        SavWav.Save(clip, stream);
        return stream.ToArray();
    }

    public static string LoadMuzPackPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + ".muzpack");
    }

    public static SoundData LoadMuzPack(string fileName)
    {
        string loadPath = Path.Combine(Application.persistentDataPath, fileName + ".muzpack");

        string name = "";
        int bpm = 0;
        List<TimeValuePair> bits = new List<TimeValuePair>();
        OwnerData owner = null;
        byte[] imageBytes = null;
        byte[] audioBytes = null;

        using (FileStream fs = new FileStream(loadPath, FileMode.Open))
        using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
        {
            foreach (var entry in archive.Entries)
            {
                switch (entry.Name)
                {
                    case "Name.txt":
                        name = ReadTextFromStream(entry.Open());
                        break;
                    case "BPM.txt":
                        int.TryParse(ReadTextFromStream(entry.Open()), out bpm);
                        break;
                    case "Bits.json":
                        bits = JsonUtility.FromJson<BitsWrapper>(ReadTextFromStream(entry.Open())).bits;
                        break;
                    case "Owner.json":
                        owner = JsonUtility.FromJson<OwnerData>(ReadTextFromStream(entry.Open()));
                        break;
                    case "Image.png":
                        imageBytes = ReadBytesFromStream(entry.Open());
                        break;
                    case "Sound.wav":
                        audioBytes = ReadBytesFromStream(entry.Open());
                        break;
                }
            }
        }

        if (string.IsNullOrEmpty(name) || imageBytes == null || audioBytes == null || owner == null)
        {
            Debug.LogError("Ошибка загрузки .muzpack");
            return null;
        }

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);
        Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        AudioClip sound = ConvertBytesToAudioClip(audioBytes);

        return new SoundData(name, image, sound, bits, owner, bpm);
    }

    private static string ReadTextFromStream(Stream stream)
    {
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    private static byte[] ReadBytesFromStream(Stream stream)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    private static AudioClip ConvertBytesToAudioClip(byte[] bytes)
    {
        return WavUtility.ToAudioClip(bytes, "LoadedSound");
    }


    public static List<MuzPackPreview> GetMuzPackPreviews()
    {
        List<MuzPackPreview> previews = new List<MuzPackPreview>();

        if (!Directory.Exists(localPath))
            return previews;

        string[] files = Directory.GetFiles(localPath, "*.muzpack");

        foreach (string filePath in files)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);
            Sprite image = ExtractImageFromMuzPack(filePath);
            OwnerData ownerType = ExtractOwnerTypeFromMuzPack(filePath);

            if (image != null)
            {
                previews.Add(new MuzPackPreview(name, image, ownerType));
            }
        }

        return previews;
    }

    public static MuzPackPreview GetMuzPackPreviewByName(string nameMP)
    {
        MuzPackPreview previews;

        if (!Directory.Exists(localPath))
            return null;

        string[] files = Directory.GetFiles(localPath, "*.muzpack");

        foreach (string filePath in files)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);
            Sprite image = ExtractImageFromMuzPack(filePath);
            OwnerData ownerType = ExtractOwnerTypeFromMuzPack(filePath);

            if (image != null && nameMP == name)
            {
                previews = new MuzPackPreview(name, image, ownerType);
                return previews;
            }
        }

        return null;
    }

    private static OwnerData ExtractOwnerTypeFromMuzPack(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
        {
            ZipArchiveEntry jsonEntry = archive.GetEntry("Owner.json");
            if (jsonEntry != null)
            {
                using (StreamReader reader = new StreamReader(jsonEntry.Open()))
                {
                    string jsonContent = reader.ReadToEnd();
                    OwnerData pack = JsonUtility.FromJson<OwnerData>(jsonContent);
                    return pack;
                }
            }
        }

        return null; // Или другой дефолтный вариант
    }

    public static bool DeleteMuzpackByName(string muzpackName)
    {
        string filePath = Path.Combine(localPath, muzpackName + ".muzpack");

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log($"MuzPack {muzpackName} успешно удалён.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при удалении {muzpackName}: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"Файл {muzpackName}.muzpack не найден.");
            return false;
        }
    }

    private static Sprite ExtractImageFromMuzPack(string muzPackPath)
    {
        using (FileStream fs = new FileStream(muzPackPath, FileMode.Open))
        using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.Name == "Image.png")
                {
                    byte[] imageBytes = ReadBytesFromStream(entry.Open());

                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageBytes);
                    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                }
            }
        }

        return null;
    }
}

[Serializable]
public class BitsWrapper
{
    public List<TimeValuePair> bits;

    public BitsWrapper(List<TimeValuePair> bits)
    {
        this.bits = bits;
    }
}

[Serializable]
public class MuzPackPreview
{
    public string Name { get; private set; }
    public Sprite Image { get; private set; }
    public OwnerData OwnerType { get; private set; }

    public MuzPackPreview(string name, Sprite image, OwnerData ownerType)
    {
        Name = name;
        Image = image;
        OwnerType = ownerType;
    }

    public override string ToString()
    {
        string imageData = Image != null ? Convert.ToBase64String(Image.texture.EncodeToPNG()) : "null";
        return $"{Name} {imageData} {OwnerType}";
    }
}