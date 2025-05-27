using System.IO;
using System.Threading.Tasks;
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
            isNonBuyedText.gameObject.SetActive(false);
        }
        else
        {
            // Пользователь не купил объект и он не является стандартным
            isNonBuyedText.gameObject.SetActive(true);
        }
    }

    public void OnClick()
    {
        OnClickAsync();
    }

    private async Task OnClickAsync()
    {
        if (isNonBuyedText.gameObject.activeSelf)
        {
            if(await NetServerController.Instance.BuySong(soundData))
                setLevelList.OnEnable();
        }
        else
            setLevelList.SetSelectedSound(soundData);
    }
}
