using System;
using System.Collections.Generic;
using UnityEngine;
using static JsonDataSaver;

[Serializable]
public class SoundDataPack
{
    public string Name;
    public int BPM;
    public List<TimeValuePair> Bits;
    public OwnerData Owner;

    public byte[] ImageData;  // PNG или JPG
    public byte[] SoundData;  // WAV или MP3

    public SoundDataPack(string name, int bpm, List<TimeValuePair> bits, OwnerData owner, byte[] imageData, byte[] soundData)
    {
        Name = name;
        BPM = bpm;
        Bits = bits;
        Owner = owner;
        ImageData = imageData;
        SoundData = soundData;
    }
}
