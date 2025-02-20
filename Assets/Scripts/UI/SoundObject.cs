using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundObject : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text, isNonBuyedText;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Button button;

    private SetLevelList setLevelList;
    private SoundData soundData;
    private OwnerData owner;

    public void SetData(SoundData soundData, SetLevelList setLevelList)
    {
        this.soundData = soundData;
        this.setLevelList = setLevelList;
        this.owner = soundData.Owner;

        text.SetText(soundData.Name);
        image.sprite = soundData.Image;

        if ((owner.ownedType & (OwnerType.buyed | OwnerType.standart | OwnerType.owner)) != 0)
        {
            // Пользователь купил объект или он входит в стандартную библиотеку
            button.interactable = true;
            isNonBuyedText.gameObject.SetActive(false);
        }
        else
        {
            // Пользователь не купил объект и он не является стандартным
            button.interactable = false;
            isNonBuyedText.gameObject.SetActive(true);
        }
    }

    public void OnClick()
    {
        setLevelList.SetSelectedSound(soundData);
    }
}
