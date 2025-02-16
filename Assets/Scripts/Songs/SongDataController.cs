using System.Collections;
using UnityEngine;

public class SongDataController : MonoBehaviour
{
    public SoundData SelectedSong { get => selectedSong; set => selectedSong = value; }

    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    protected BitLine bitLineA, bitLineW, bitLineS, bitLineD;

    private SoundData selectedSong;

    protected void Start()
    {
        SelectedSong = GameData.SelectedSong;
        if (SelectedSong == null || SelectedSong.Sound == null)
        {
            Debug.LogError("Выбранная песня не задана!");
            return;
        }

        audioSource.clip = SelectedSong.Sound;
        audioSource.Play();

        StartCoroutine(PlayBits());
    }

    protected IEnumerator PlayBits(float startTimeOffset = 0f)
    {
        float distance = Vector3.Distance(bitLineA.GetStartPosition(), bitLineA.GetEndPosition());
        float travelTime = distance / bitLineA.GetSpeed();

        foreach (var bit in SelectedSong.Bits)
        {
            float startTime = bit.time - travelTime;
            if (startTime < startTimeOffset)
                continue;

            float delay = startTime - audioSource.time;
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            Debug.Log($"Бит {bit.value} создаётся в {startTime} сек, чтобы прибыть в {bit.time} сек");

            GetBitLine(bit.value)?.CreateBit(new DataBit(bit));
        }
    }

    private BitLine GetBitLine(int value)
    {
        return value switch
        {
            0 => bitLineA,
            1 => bitLineW,
            2 => bitLineS,
            3 => bitLineD,
            _ => null
        };
    }
}
