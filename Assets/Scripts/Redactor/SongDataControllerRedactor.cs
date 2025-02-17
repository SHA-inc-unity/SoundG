using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongDataControllerRedactor : SongDataController
{
    private bool isAddingOrRemoving = true;

    [SerializeField]
    private Slider redactorTime;
    [SerializeField]
    private Toggle redactorToggle;

    public bool IsAddingOrRemoving { get => isAddingOrRemoving; }

    public void ChangeRedactorTime()
    {
        float time = redactorTime.value;

        if (Mathf.Abs(audioSource.time - redactorTime.value) > 1f)
        {
            bitLineA.Clear(); bitLineW.Clear(); bitLineS.Clear(); bitLineD.Clear();
            audioSource.time = time;
            StopAllCoroutines();
            StartCoroutine(PlayBits(time));
        }
    }

    public void SaveAndExitBits()
    {
        SaveBits();
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveBits()
    {
        var json = JsonDataSaver.Load();
        if (json.ContainsKey(SelectedSong.Name))
            json[SelectedSong.Name] = SelectedSong.Bits;
        else
            json.Add(SelectedSong.Name, SelectedSong.Bits);
        JsonDataSaver.Save(json);
    }

    public void ChangeAddingOrRemoving()
    {
        isAddingOrRemoving = redactorToggle.isOn;
    }

    private new void Start()
    {
        base.Start();

        redactorTime.maxValue = SelectedSong.Sound.length;
    }

    private void Update()
    {
        redactorTime.value = audioSource.time;

        
    }
}
