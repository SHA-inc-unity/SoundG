using NUnit.Framework;
using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static JsonDataSaver;

public class PanelCreateMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Button redactor, playButton;
    [SerializeField] private Image previewImage; // UI-������� ��� ����������� ����������� ��������
    [SerializeField] private TMP_Text songTitleText; // UI-������� ��� ����������� �������� ������
    [SerializeField] private TMP_InputField BPMInputField;

    private Sprite image;
    private string enteredText;
    private LoadSounds loadSounds = new LoadSounds();
    private int BPM = 0;

    public void CheckBPMInputField()
    {
        BPM = int.TryParse(BPMInputField.text, out int bpm) ? bpm : 0;
        UnlockRedactor();
    }

    public void PickAudioFile()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("�������� ���������", "", new[] { new ExtensionFilter("Audio Files", "mp3", "wav", "ogg") }, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string path = paths[0];
            enteredText = Path.GetFileNameWithoutExtension(path); // ��������� �������� ����� ��� ����������
            songTitleText.text = enteredText; // ���������� �������� �����
            StartCoroutine(LoadAudio(path));
        }
    }

    public void PickImageFile()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("�������� �����������", "", new[] { new ExtensionFilter("Images", "png", "jpg", "jpeg") }, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string path = paths[0];
            StartCoroutine(LoadImage(path));
        }
    }

    public void PauseUnpause()
    {
        if (audioSource.isPlaying)
        {
            playButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "||";
            audioSource.Pause();
        }
        else
        {
            playButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = ">";
            audioSource.UnPause();
        }
    }

    public void OpenRedactorOfBits()
    {
        List<JsonDataSaver.TimeValuePair> bitList = new List<JsonDataSaver.TimeValuePair>();

        try
        {
            Dictionary<string, (int, List<TimeValuePair>)> bits = loadSounds.LoadBits();
            foreach (var sound in bits)
                if (sound.Key == enteredText)
                {
                    bitList = sound.Value.Item2;
                    break;
                }
        }
        catch (System.Exception)
        {
            throw;
        }

        // ������� �������� �� ��������� �� ������

        GameData.SetSelectedSong(new SoundData(enteredText, image, audioSource.clip, bitList, new OwnerData("null", OwnerType.owner), BPM));
        SceneManager.LoadScene("BitsRedactor2");
    }

    private IEnumerator LoadAudio(string path)
    {
        using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip("file:///" + path, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                audioSource.clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
                if (audioSource.clip != null)
                {
                    PickImageFile();
                }
            }
            else
            {
                Debug.LogError("������ �������� ����������: " + www.error);
            }
        }
    }

    private IEnumerator LoadImage(string path)
    {
        using (var www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture("file:///" + path))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;
                image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                if (previewImage != null)
                {
                    previewImage.sprite = image;
                    UnlockRedactor();
                    audioSource.Play();
                    PauseUnpause();
                }
            }
            else
            {
                Debug.LogError("������ �������� �����������: " + www.error);
            }
        }
    }

    private void UnlockRedactor()
    {
        if (audioSource.clip != null)
        {
            if (BPM != 0)
            {
                redactor.interactable = true;
            }
            playButton.interactable = true;
        }
    }
}
