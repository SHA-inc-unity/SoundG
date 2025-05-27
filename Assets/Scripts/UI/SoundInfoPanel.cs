using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundInfoPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text soundName, buttonText;
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

        if (!File.Exists(MuzPackSaver.LoadMuzPackPath(soundData.Name)) && GameData.SelectedSong.Owner.ownedType != OwnerType.standart)
            buttonText.text = "Load Song";
        else
            buttonText.text = "Start Song";
    }

    public void StartSong()
    {
       StartSongAsync();
    }

    public async Task StartSongAsync()
    {
        if (GameData.SelectedSong.Owner.ownedType != OwnerType.standart)
        {
            GameData.SetSelectedSong(await MuzPackSaver.LoadMuzPack(GameData.SelectedSong.Name));
        }

        SceneManager.LoadScene("Game");
    }
}
