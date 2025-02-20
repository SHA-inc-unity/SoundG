using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PanelsName
{
    startGame,
    setLevel,
    createMusic,
    redactorMusic,
    scoresList,
    publishMusic,
    loginRegister,
}

public class PanelController : MonoBehaviour
{
    [SerializeField]
    private List<Panel> panels;

    void Start()
    {
        ChangePanel(PanelsName.startGame);

        var x = NetServerController.Instance;
    }

    public void ChangePanel(PanelsName newPanel)
    {
        foreach (var item in panels)
        {
            if(item.name == newPanel)
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
        }
    }

    public void ChangePanelByName(string panelName)
    {
        if (Enum.TryParse(panelName, out PanelsName panel))
        {
            ChangePanel(panel);
        }
    }
}

[Serializable]
public class Panel
{
    public PanelsName name;
    public GameObject gameObject;
}