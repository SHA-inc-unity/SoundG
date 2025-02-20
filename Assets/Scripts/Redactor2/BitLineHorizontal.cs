using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static JsonDataSaver;

public class BitLineHorizontal : MonoBehaviour
{
    [SerializeField]
    private TMP_Text time, tick;
    [SerializeField]
    private List<BitLineBit> buttons = new List<BitLineBit>();

    public void SetTimeTick(string time, string tick)
    {
        this.time.text = time;
        this.tick.text = tick;
    }

    public void SetActiveLines(List<int> ints)
    {
        foreach (var item in ints)
        {
            ClickLine(item);
        }
    }

    public void ClickLine(int lineNum)
    {
        if (lineNum >= 0 && lineNum < buttons.Count)
        {
            buttons[lineNum].isActive = !buttons[lineNum].isActive;
            buttons[lineNum].button.image.color = buttons[lineNum].isActive ? Color.red  : Color.white;
        }
    }

    public List<TimeValuePair> GetResult()
    {
        List<TimeValuePair> res = new List<TimeValuePair>();
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].isActive)
            {
                res.Add(new TimeValuePair(float.Parse(time.text), i));
            }
        }
        return res;
    }
}

[Serializable]
public class BitLineBit
{
    public bool isActive = false;
    public Button button;

    public BitLineBit(bool isActive, Button button)
    {
        this.isActive = isActive;
        this.button = button;
    }
}