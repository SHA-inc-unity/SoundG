using System.Collections.Generic;
using UnityEngine;

public class PublishMusic : MonoBehaviour
{
    [SerializeField]
    private SoundObject standartSoundPublishObject;
    [SerializeField]
    private SoundInfoPanel soundInfoPanel;

    private LoadSounds loadSounds = new LoadSounds();

    private async void Start()
    {
        List<SoundData> soundList = await loadSounds.LoadAllSounds(1);
        foreach (SoundData data in soundList)
        {
            //SoundObject soundObject = Instantiate(standartSoundPublishObject, contentSoundList);
            //soundObject.SetData(data);
        }
    }
}
