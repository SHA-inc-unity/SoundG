using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static JsonDataSaver;

public class RedactorBitLineHorizontal : MonoBehaviour
{
    [SerializeField]
    private BitLineHorizontal bitLineHorizontal;
    [SerializeField]
    private Transform content;

    private List<BitLineHorizontal> bitLines = new List<BitLineHorizontal>();
    private int ticks = 0;
    private float tickInTime = 0;
    private SoundData selectedSong;

    void Start()
    {
        selectedSong = GameData.SelectedSong;
        ticks = (int)(((float)selectedSong.Sound.length / 60f) * (float)selectedSong.BPM);
        tickInTime = 60f / (float)selectedSong.BPM;

        CreateBitLines();
    }

    private void CreateBitLines()
    {
        RectTransform rectTransform = content.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(215, rectTransform.sizeDelta.y);

        Dictionary<string, (int, List<TimeValuePair>)> json = JsonDataSaver.LoadBitList();
        List<TimeValuePair> bits = json.ContainsKey(selectedSong.Name) ? json[selectedSong.Name].Item2 : new List<TimeValuePair>();

        for (int i = 0; i < ticks; i++)
        {
            rectTransform.sizeDelta += new Vector2(210, 0);
            var bitline = Instantiate(bitLineHorizontal, content);
            float targetTime = i * tickInTime;

            float prevTime = (i - 0.5f) * tickInTime;
            float nextTime = (i + 0.5f) * tickInTime;

            // Выбираем биты, которые ближе к targetTime, чем к соседним точкам
            List<int> activeLines = bits
                .Where(bit => Math.Abs(bit.time - targetTime) <= Math.Abs(bit.time - prevTime) &&
                              Math.Abs(bit.time - targetTime) <= Math.Abs(bit.time - nextTime))
                .Select(bit => bit.value) // Получаем только значения битов
                .Distinct() // Убираем дубликаты
                .ToList();

            bitline.SetTimeTick(targetTime.ToString(), i.ToString());
            bitline.SetActiveLines(activeLines);

            bitLines.Add(bitline);
        }
    }

    public void SaveAndExit()
    {
        List<TimeValuePair> bitList = new List<TimeValuePair>();
        foreach (var item in bitLines)
        {
            bitList.AddRange(item.GetResult());
        }

        var json = JsonDataSaver.LoadBitList();
        if (json.ContainsKey(selectedSong.Name))
            json[selectedSong.Name] = (selectedSong.BPM, bitList);
        else
            json.Add(selectedSong.Name, (selectedSong.BPM, bitList));
        JsonDataSaver.SaveBitList(json);
        selectedSong.ResetOwner(new OwnerData(LoginData.UserData.name, OwnerType.owner));
        MuzPackSaver.SaveMuzPack(selectedSong);

        SceneManager.LoadScene("MainMenu");
    }
}
