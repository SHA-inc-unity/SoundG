using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PublishMusic : MonoBehaviour
{
    [SerializeField]
    private Transform contentSoundList;
    [SerializeField]
    private SoundObjectPublish standartSoundPublishObject;
    [SerializeField]
    private PublishInfoPanel soundInfoPanel;

    private LoadSounds loadSounds = new LoadSounds();

    public void SetSelectedSound(SoundData soundData)
    {
        soundInfoPanel.RefreshSoundInfoPanel(soundData);
    }

    private async void OnEnable()
    {
        List<SoundData> soundList = await loadSounds.LoadAllSounds(1, false);
        for (int i = contentSoundList.childCount - 1; i >= 0; i--)
        {
            Destroy(contentSoundList.GetChild(i).gameObject);
        }
        foreach (SoundData data in soundList)
        {
            if (data.Owner.ownedType == OwnerType.owner)
            {
                SoundObjectPublish soundObject = Instantiate(standartSoundPublishObject, contentSoundList);
                soundObject.SetData(data, this);
            }
        }
    }
}
