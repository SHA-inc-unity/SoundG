using System.Collections.Generic;
using UnityEngine;

public class PublishMusic : MonoBehaviour
{
    [SerializeField]
    private SoundObject standartSoundPublishObject;
    [SerializeField]
    private SoundInfoPanel soundInfoPanel;

    private LoadSounds loadSounds = new LoadSounds();

    private void Start()
    {
        List<SoundData> soundList = loadSounds.LoadAllSounds(1);
        foreach (SoundData data in soundList)
        {
            //SoundObject soundObject = Instantiate(standartSoundPublishObject, contentSoundList);
            //soundObject.SetData(data);
        }
    }
}
