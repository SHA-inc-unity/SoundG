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
        loadSounds.LoadAllSounds(true);
    }

    public void SetSelectedSound(SoundData sound)
    {
        soundInfoPanel.RefreshSoundInfoPanel(sound);
    }

    private void Start()
    {
        List<SoundData> soundList = loadSounds.LoadAllSounds(false);
        foreach (SoundData data in soundList)
        {
            SoundObject soundObject = Instantiate(standartSoundObject, contentSoundList);
            soundObject.SetData(data, this);
        }
    }
}
