using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static JsonDataSaver;

[Serializable]
[Flags]
public enum OwnerType
{
    standart = 1,
    buyed = 2,
    top100 = 4,
    downloaded = 8,
    owner = 16,
}

public class SoundData
{
    public string Name { get; private set; }
    public Sprite Image { get; private set; }
    public AudioClip Sound { get; private set; }
    public List<TimeValuePair> Bits { get; private set; }
    public OwnerData Owner { get; private set; }

    public SoundData(string name, Sprite image, AudioClip sound, List<TimeValuePair> bits, OwnerData ownerData)
    {
        Name = name;
        Image = image;
        Sound = sound;
        Bits = bits;
        Owner = ownerData;
    }
}

[Serializable]
public class OwnerData
{
    public string owner { get; private set; }
    public OwnerType ownedType { get; private set; }

    public OwnerData(string owner, OwnerType ownedType)
    {
        this.owner = owner;
        this.ownedType = ownedType;
    }
}