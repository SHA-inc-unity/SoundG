using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static JsonDataSaver;

[Serializable, Flags]
public enum OwnerType
{
    standart = 1,
    buyed = 2,
    owner = 4,
    load = 8,
}

[Serializable]
public class SoundData
{
    public string Name { get; private set; }
    public Sprite Image { get; private set; }
    public AudioClip Sound { get; private set; }
    public int BPM { get; private set; }
    public List<TimeValuePair> Bits { get; private set; }
    public OwnerData Owner { get; private set; }

    public SoundData(string name, Sprite image, AudioClip sound, List<TimeValuePair> bits, OwnerData ownerData, int bPM)
    {
        Name = name;
        Image = image;
        Sound = sound;
        Bits = bits;
        Owner = ownerData;
        BPM = bPM;
    }

    public void ResetOwner(OwnerData ownerData)
    {
        Owner = ownerData;
    }
}

[Serializable]
public class OwnerData
{
    [SerializeField]
    public string owner;
    [SerializeField]
    public OwnerType ownedType;

    public OwnerData(string owner, OwnerType ownedType)
    {
        this.owner = owner;
        this.ownedType = ownedType;
    }
}