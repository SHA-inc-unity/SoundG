using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    private const int HEADER_SIZE = 44;

    public static byte[] ToWav(AudioClip clip)
    {
        if (clip == null) return null;

        using (MemoryStream stream = new MemoryStream())
        {
            int sampleCount = clip.samples * clip.channels;
            int byteCount = sampleCount * sizeof(short);
            int fileSize = byteCount + HEADER_SIZE - 8;

            // WAV Header
            WriteString(stream, "RIFF");
            WriteInt(stream, fileSize);
            WriteString(stream, "WAVE");

            // fmt chunk
            WriteString(stream, "fmt ");
            WriteInt(stream, 16);
            WriteShort(stream, 1);
            WriteShort(stream, (short)clip.channels);
            WriteInt(stream, clip.frequency);
            WriteInt(stream, clip.frequency * clip.channels * sizeof(short));
            WriteShort(stream, (short)(clip.channels * sizeof(short)));
            WriteShort(stream, 16);

            // data chunk
            WriteString(stream, "data");
            WriteInt(stream, byteCount);

            // Запись аудиоданных
            float[] samples = new float[sampleCount];
            clip.GetData(samples, 0);
            foreach (var sample in samples)
            {
                short intSample = (short)(sample * short.MaxValue);
                WriteShort(stream, intSample);
            }

            return stream.ToArray();
        }
    }

    public static AudioClip ToAudioClip(byte[] wavData, string clipName)
    {
        if (wavData == null || wavData.Length < HEADER_SIZE)
        {
            Debug.LogError("Invalid WAV data");
            return null;
        }

        using (MemoryStream stream = new MemoryStream(wavData))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            reader.BaseStream.Seek(22, SeekOrigin.Begin);
            short channels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            reader.BaseStream.Seek(40, SeekOrigin.Begin);
            int dataSize = reader.ReadInt32();

            float[] samples = new float[dataSize / sizeof(short)];
            for (int i = 0; i < samples.Length; i++)
            {
                short sample = reader.ReadInt16();
                samples[i] = sample / (float)short.MaxValue;
            }

            AudioClip clip = AudioClip.Create(clipName, samples.Length, channels, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }

    private static void WriteString(Stream stream, string value)
    {
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static void WriteInt(Stream stream, int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static void WriteShort(Stream stream, short value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }
}


public static class SavWav
{
    public static void Save(AudioClip clip, Stream stream)
    {
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            byte[] wav = WavUtility.ToWav(clip);
            writer.Write(wav);
        }
    }
}