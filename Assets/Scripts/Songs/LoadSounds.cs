using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static JsonDataSaver;

public class LoadSounds
{
    public List<SoundData> LoadAllSounds(bool reanalize)
    {
        List<SoundData> soundList = new List<SoundData>();

        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Sound");

        Dictionary<string, List<TimeValuePair>> bits = JsonDataSaver.Load();

        foreach (var clip in audioClips)
        {
            string soundName = clip.name;
            Sprite image = LoadImageForSound(soundName);

            if (!bits.ContainsKey(soundName))
            {
                List<TimeValuePair> x = BitGenerator.AnalyzeMusic(clip);
                bits.Add(soundName, x);
            }

            if (reanalize)
            {
                List<TimeValuePair> x = BitGenerator.AnalyzeMusic(clip);
                bits[soundName] = x;
            }

            soundList.Add(new SoundData(soundName, image, clip, bits[soundName]));
        }

        JsonDataSaver.Save(bits);

        return soundList;
    }

    private Sprite LoadImageForSound(string soundName)
    {
        Sprite image = Resources.Load<Sprite>("Sound/" + soundName);
        return image;
    }
}