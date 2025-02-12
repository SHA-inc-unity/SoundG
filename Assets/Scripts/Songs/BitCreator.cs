using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BitCreator : MonoBehaviour
{
    public int sampleSize = 1024; // Размер окна FFT
    public int targetRate = 4;    // Количество уровней в секунду
    private float[] samples;
    private float[] spectrum;

    public List<int> AnalyzeMusic(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogError("AudioClip не задан!");
            return new List<int>();
        }

        int sampleRate = audioClip.frequency;
        int totalSamples = audioClip.samples;
        int step = sampleRate / targetRate; // Шаг сэмплов для анализа
        List<int> levels = new List<int>();

        samples = new float[sampleSize];
        spectrum = new float[sampleSize];

        AudioClip clip = audioClip;
        float[] fullData = new float[totalSamples * clip.channels];
        clip.GetData(fullData, 0);

        for (int i = 0; i < totalSamples; i += step)
        {
            float volume = GetVolume(fullData, i, step);
            int insertCount = Mathf.RoundToInt(volume * 4);
            float[] sampleSlice = GetSlice(fullData, i, sampleSize);
            float[] spectrumData = FFT(sampleSlice);

            double[] energies = AnalyzeFrequencies(spectrumData, sampleRate);

            for (int j = 0; j < insertCount; j++)
            {
                levels.Add(GetArrowLevel(energies));
            }
        }

        return levels;
    }

    private float GetVolume(float[] data, int start, int length)
    {
        float sum = 0;
        int end = Mathf.Min(start + length, data.Length);
        for (int i = start; i < end; i++)
        {
            sum += Mathf.Abs(data[i]);
        }
        return Mathf.Clamp(sum / length, 0f, 1f);
    }

    private float[] GetSlice(float[] data, int start, int length)
    {
        float[] slice = new float[length];
        int end = Mathf.Min(start + length, data.Length);
        for (int i = start; i < end; i++)
        {
            slice[i - start] = data[i];
        }
        return slice;
    }

    private float[] FFT(float[] data)
    {
        float[] spectrum = new float[data.Length];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        return spectrum;
    }

    private double[] AnalyzeFrequencies(float[] spectrum, int sampleRate)
    {
        double lowBass = 0, highBass = 0, midFreq = 0, highFreq = 0;

        for (int i = 0; i < spectrum.Length / 2; i++)
        {
            float freq = i * sampleRate / spectrum.Length;
            float magnitude = spectrum[i];

            if (freq < 150) lowBass += magnitude;
            else if (freq < 400) highBass += magnitude;
            else if (freq < 2000) midFreq += magnitude;
            else highFreq += magnitude;
        }

        return new double[] { lowBass, highBass, midFreq, highFreq };
    }

    private int GetArrowLevel(double[] energies)
    {
        double maxEnergy = energies.Max();
        if (maxEnergy < 0.25) return 0;
        if (maxEnergy < 0.5) return 1;
        if (maxEnergy < 0.75) return 2;
        return 3;
    }
}
