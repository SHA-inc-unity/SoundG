using UnityEngine;

public class GameData : MonoBehaviour
{
    private static SoundData selectedSong;
    public static SoundData SelectedSong { get => selectedSong; }


    public static void SetSelectedSong(SoundData soundData)
    {
        selectedSong = soundData;
    }
}
