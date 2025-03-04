using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PublishInfoPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text soundName, uploadText;
    [SerializeField]
    private Image soundImage;
    [SerializeField]
    private Button startSoundButton;
    [SerializeField]
    private MuzPackPreview muzPackPreview;

    private void Update()
    {
        if (UploadDownloadStatus.mustLoadedData != 0 && UploadDownloadStatus.loadedData < UploadDownloadStatus.mustLoadedData)
        {
            uploadText.text = $"{UploadDownloadStatus.loadedData / 1024}kb / {UploadDownloadStatus.mustLoadedData / 1024}kb";
        }
        else
        {
            uploadText.text = "";
        }
    }

    public void RefreshSoundInfoPanel(SoundData soundData)
    {
        if (soundData == null)
            return;

        startSoundButton.GetComponent<Button>().interactable = true;

        soundName.SetText(soundData.Name);
        soundImage.sprite = soundData.Image;
        this.muzPackPreview = MuzPackSaver.GetMuzPackPreviewByName(soundData.Name);

        GameData.SetSelectedSong(soundData);
    }

    public void PublishSong()
    {
        PublishSongAsync();
    }

    private async void PublishSongAsync()
    {
        bool task = await NetServerController.Instance.PublishSong(LoginData.UserData.name, LoginData.UserData.password, muzPackPreview, soundName.text);
        Debug.Log(task);
    }
}
