using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundObject : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private Image image;

    public void SetData(SoundData soundData)
    {
        this.text.SetText(soundData.GetName());
        this.image = soundData.GetImage();
    }
}

public class SoundData
{
    string name;
    Image image;

    public SoundData(string text, Image image)
    {
        this.name = text;
        this.image = image;
    }

    public Image GetImage()
    {
        return image;
    }

    public string GetName()
    {
        return name;
    }
}