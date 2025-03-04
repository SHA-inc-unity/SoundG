using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundObjectPublish : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Button button;

    private PublishMusic setLevelList;
    private SoundData soundData;
    private OwnerData owner;


    public void SetData(SoundData soundData, PublishMusic setLevelList)
    {
        this.soundData = soundData;
        this.setLevelList = setLevelList;
        this.owner = soundData.Owner;

        text.SetText(soundData.Name);
        image.sprite = soundData.Image;

        if ((owner.ownedType & (OwnerType.owner)) != 0)
        {
            // ������������ ����� ������ ��� �� ������ � ����������� ����������
            button.interactable = true;
        }
        else
        {
            // ������������ �� ����� ������ � �� �� �������� �����������
            button.interactable = false;
        }
    }

    public void OnClick()
    {
        setLevelList.SetSelectedSound(soundData);
    }
}
