using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundInfoPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text soundName;
    [SerializeField]
    private Image soundImage;
    [SerializeField]
    private Button startSoundButton;

    public void RefreshSoundInfoPanel(SoundData soundData)
    {
        if(soundData == null)
            return;

        startSoundButton.GetComponent<Button>().interactable = true;

        soundName.SetText(soundData.Name);
        soundImage.sprite = soundData.Image;

        GameData.SetSelectedSong(soundData);
    }

    public void StartSong()
    {

        SceneManager.LoadScene("Game");
    }
}
