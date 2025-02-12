using UnityEngine;

public class SoundData
{
    public string Name { get; private set; }
    public Sprite Image { get; private set; }
    public AudioClip Sound { get; private set; }

    public SoundData(string name, Sprite image, AudioClip sound)
    {
        Name = name;
        Image = image;
        Sound = sound;
    }
}
