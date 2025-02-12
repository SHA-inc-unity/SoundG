using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static JsonDataSaver;

public static class BitGenerator
{
    private static int SampleSize = 1024;
    private static float[] spectrum = new float[SampleSize];
    private static List<TimeValuePair> rhythmEvents;
    private static float globalMedian, upperMedian, lowerMedian;
    private static float bpm = 0f;

    public static List<TimeValuePair> AnalyzeMusic(AudioClip audioClip)
    {
        rhythmEvents = new List<TimeValuePair>();
        List<float> spectrumMedians = new List<float>();
        List<float[]> allSpectrums = new List<float[]>();

        float step = 0.5f; // Интервал анализа
        float[] audioData = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(audioData, 0); // Получаем весь аудиофайл

        int samplesPerStep = Mathf.RoundToInt(audioClip.frequency * step);

        for (int i = 0; i < audioData.Length; i += samplesPerStep)
        {
            // Берем фрагмент аудио и делаем FFT
            float[] sampleSegment = new float[SampleSize];
            for (int j = 0; j < SampleSize && i + j < audioData.Length; j++)
            {
                sampleSegment[j] = audioData[i + j];
            }

            AudioFFT(sampleSegment, spectrum);

            float spectrumMedian = GetMedian(spectrum);
            spectrumMedians.Add(spectrumMedian);
        }

        // Вычисляем медианы спектра
        globalMedian = GetMedian(spectrumMedians.ToArray());
        Debug.Log($"Global Spectrum Median: {globalMedian}");

        var aboveMedian = spectrumMedians.Where(x => x > globalMedian).ToArray();
        var belowMedian = spectrumMedians.Where(x => x < globalMedian).ToArray();

        upperMedian = aboveMedian.Length > 0 ? GetMedian(aboveMedian) : globalMedian;
        lowerMedian = belowMedian.Length > 0 ? GetMedian(belowMedian) : globalMedian;

        Debug.Log($"Upper Spectrum Median: {upperMedian}");
        Debug.Log($"Lower Spectrum Median: {lowerMedian}");

        // Анализируем ритм и собираем события
        float time = 0f;
        List<float> beatTimes = new List<float>();

        foreach (var spec in allSpectrums)
        {
            List<int> arrowMask = DetermineArrows(spec);
            if (arrowMask.Count > 0)
            {
                rhythmEvents.Add(new TimeValuePair(time, arrowMask.First()));
                beatTimes.Add(time);
            }
            time += step;
        }

        // Вычисление BPM
        bpm = CalculateBPM(beatTimes);
        Debug.Log($"Estimated BPM: {bpm}");

        return rhythmEvents;
    }

    private static void AudioFFT(float[] samples, float[] output)
    {
        for (int i = 0; i < samples.Length; i++)
            output[i] = samples[i];

        AudioListener.GetSpectrumData(output, 0, FFTWindow.Hamming);
    }

    private static List<int> DetermineArrows(float[] spectrum)
    {
        List<int> res = new List<int>();

        float spectrumMedian = GetMedian(spectrum);

        if (spectrumMedian < lowerMedian)
            res.Add(0);
        else if (spectrumMedian < globalMedian)
            res.Add(1);
        else if (spectrumMedian < upperMedian)
            res.Add(2);
        else
            res.Add(3);

        return res;
    }

    private static float GetMedian(float[] values)
    {
        if (values.Length == 0) return 0f;
        var sortedValues = values.OrderBy(x => x).ToArray();
        int middle = sortedValues.Length / 2;
        return sortedValues.Length % 2 == 0
            ? (sortedValues[middle - 1] + sortedValues[middle]) / 2f
            : sortedValues[middle];
    }

    private static float CalculateBPM(List<float> beatTimes)
    {
        if (beatTimes.Count < 2) return 0f;

        List<float> intervals = new List<float>();
        for (int i = 1; i < beatTimes.Count; i++)
        {
            intervals.Add(beatTimes[i] - beatTimes[i - 1]);
        }

        float avgInterval = intervals.Average();
        return avgInterval > 0 ? 60f / avgInterval : 0f;
    }
}
