using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static JsonDataSaver;

public class SoundData
{
    public string Name { get; private set; }
    public Sprite Image { get; private set; }
    public AudioClip Sound { get; private set; }
    public List<TimeValuePair> Bits { get; private set; }

    public SoundData(string name, Sprite image, AudioClip sound, List<TimeValuePair> bits)
    {
        Name = name;
        Image = image;
        Sound = sound;
        Bits = bits;
    }
}
