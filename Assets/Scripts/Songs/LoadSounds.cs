using System.Collections.Generic;
using UnityEngine;

public class LoadSounds
{
    public List<SoundData> LoadAllSounds()
    {
        List<SoundData> soundList = new List<SoundData>();

        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Sound");

        foreach (var clip in audioClips)
        {
            string soundName = clip.name;
            Sprite image = LoadImageForSound(soundName);

            soundList.Add(new SoundData(soundName, image, clip));
        }

        return soundList;
    }

    private Sprite LoadImageForSound(string soundName)
    {
        Sprite image = Resources.Load<Sprite>("Sound/" + soundName);
        return image;
    }
}