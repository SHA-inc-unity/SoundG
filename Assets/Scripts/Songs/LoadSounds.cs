using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static JsonDataSaver;

public class LoadSounds
{
    public List<SoundData> LoadAllSounds(int reanalize)
    {
        List<SoundData> soundList = new List<SoundData>();

        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Sound");

        Dictionary<string, List<TimeValuePair>> bits = JsonDataSaver.LoadBitList();

        List<(OwnerData, string)> owners = null;

        try
        {
            
        }
        catch (System.Exception)
        {
            owners = JsonDataSaver.LoadOwnerData();
            throw;
        }

        foreach (var clip in audioClips)
        {
            string soundName = clip.name;
            Sprite image = LoadImageForSound(soundName);

            // Проверка наличия анализа битов
            if (reanalize == 1 && !bits.ContainsKey(soundName))
            {
                bits[soundName] = BitGenerator.AnalyzeMusic(clip);
            }
            else if (reanalize == 2)
            {
                bits[soundName] = BitGenerator.AnalyzeMusic(clip);
            }

            OwnerData owner = owners.FirstOrDefault(o => o.Item2 == soundName).Item1 ?? new OwnerData(null, 0);

            soundList.Add(new SoundData(soundName, image, clip, bits[soundName], owner));
        }

        JsonDataSaver.SaveBitList(bits);

        return soundList;
    }

    private Sprite LoadImageForSound(string soundName)
    {
        Sprite image = Resources.Load<Sprite>("Sound/" + soundName);
        return image;
    }
}