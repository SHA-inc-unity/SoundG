using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundInfoPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text soundName;
    [SerializeField]
    private Image soundImage;
    [SerializeField]
    private Button startSoundButton;

    private SoundData nowSelectSoung;

    public void RefreshSoundInfoPanel(SoundData soundData)
    {
        if(soundData == null)
            return;

        startSoundButton.gameObject.SetActive(true);

        soundName.SetText(soundData.Name);
        soundImage.sprite = soundData.Image;

        nowSelectSoung = soundData;
    }

    public void StartSong()
    {
        // Тут начало песни
    }
}
