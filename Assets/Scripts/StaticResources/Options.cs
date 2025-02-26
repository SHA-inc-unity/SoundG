using UnityEngine;

public static class Options
{
    private static int soundLvl = 100, bufferSize = 131072, maxParallelUploads = 16;

    public static int SoundLvl { get => soundLvl; }
    public static int BufferSize { get => bufferSize; }
    public static int MaxParallelUploads { get => maxParallelUploads; }
}
