using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundObject : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private Image image;

    private SetLevelList setLevelList;
    private SoundData soundData;

    public void SetData(SoundData soundData, SetLevelList setLevelList)
    {
        this.soundData = soundData;
        this.setLevelList = setLevelList;

        text.SetText(soundData.Name);
        image.sprite = soundData.Image;
    }

    public void OnClick()
    {
        setLevelList.SetSelectedSound(soundData);
    }
}
