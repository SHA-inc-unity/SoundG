using System.Collections.Generic;
using UnityEngine;

public class SetLevelList : MonoBehaviour
{
    [SerializeField]
    private Transform contentSoundList;
    [SerializeField]
    private SoundObject standartSoundObject;
    [SerializeField]
    private SoundInfoPanel soundInfoPanel;

    private LoadSounds loadSounds = new LoadSounds();

    public void ReloadAllSounds()
    {
        loadSounds.LoadAllSounds(2, true);
    }

    public void SetSelectedSound(SoundData sound)
    {
        soundInfoPanel.RefreshSoundInfoPanel(sound);
    }

    public async void OnEnable()
    {
        List<SoundData> soundList = await loadSounds.LoadAllSounds(1, true);
        for (int i = contentSoundList.childCount - 1; i >= 0; i--)
        {
            Destroy(contentSoundList.GetChild(i).gameObject);
        }
        foreach (SoundData data in soundList)
        {
            SoundObject soundObject = Instantiate(standartSoundObject, contentSoundList);
            soundObject.SetData(data, this);
        }
    }
}
