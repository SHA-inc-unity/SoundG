using System.Collections;
using UnityEngine;

public class SongDataController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private BitLine bitLineA, bitLineW, bitLineS, bitLineD;

    private SoundData selectedSong;

    void Start()
    {
        selectedSong = GameData.SelectedSong;
        if (selectedSong == null || selectedSong.Sound == null)
        {
            Debug.LogError("Выбранная песня не задана!");
            return;
        }

        audioSource.clip = selectedSong.Sound;
        audioSource.Play();

        StartCoroutine(PlayBits());
    }

    IEnumerator PlayBits()
    {
        float distance = Vector3.Distance(bitLineA.GetStartPosition(), bitLineA.GetEndPosition());
        float travelTime = distance / bitLineA.GetSpeed(); // Время на перемещение

        foreach (var bit in selectedSong.Bits)
        {
            float startTime = bit.time - travelTime; // Когда нужно создать бит
            float delay = startTime - audioSource.time;

            if (delay > 0)
                yield return new WaitForSeconds(delay);

            Debug.Log($"Бит {bit.value} создаётся в {startTime} сек, чтобы прибыть в {bit.time} сек");

            switch (bit.value)
            {
                case 0:
                    bitLineA.CreateBit();
                    break;
                case 1:
                    bitLineW.CreateBit();
                    break;
                case 2:
                    bitLineS.CreateBit();
                    break;
                case 3:
                    bitLineD.CreateBit();
                    break;
                default:
                    break;
            }
        }
    }
}
