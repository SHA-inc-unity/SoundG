using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public enum Scores
{
    Perfect,
    Good,
    Normal,
    Bad,
    Lol,
}
[Serializable]
public class ScoreData
{
    [SerializeField]
    public Scores scores;
    [SerializeField]
    public int num;
    [SerializeField]
    public Color color;
}

public class SongDataController : MonoBehaviour
{
    public SoundData SelectedSong { get => selectedSong; set => selectedSong = value; }
    public int Score { 
        get => score; 
        set 
        {
            score = value;
            scoreText.text = score.ToString();
        } 
    }

    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    protected BitLine bitLineA, bitLineW, bitLineS, bitLineD;
    [SerializeField]
    private List<ScoreData> scores;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private PrizeStatisticPanel prizeStatistic;

    private SoundData selectedSong;
    private int score = 0;

    public ScoreData AddScore(Scores x)
    {
        for (int i = 0; i < scores.Count; i++)
            if (scores[i].scores == x)
            {
                Score += scores[i].num;
                return scores[i];
            }
        return null;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    protected void Start()
    {
        SelectedSong = GameData.SelectedSong;
        if (SelectedSong == null || SelectedSong.Sound == null)
        {
            Debug.LogError("Выбранная песня не задана!");
            return;
        }

        audioSource.clip = SelectedSong.Sound;
        audioSource.Play();

        StartCoroutine(PlayBits());
    }

    protected IEnumerator PlayBits(float startTimeOffset = 0f)
    {
        float distance = Vector3.Distance(bitLineA.GetStartPosition(), bitLineA.GetEndPosition());
        float travelTime = distance / bitLineA.GetSpeed();

        int i = 0;
        while (i < SelectedSong.Bits.Count)
        {
            var bit = SelectedSong.Bits[i];
            i++;

            float startTime = bit.time - travelTime;
            if (startTime < startTimeOffset)
                continue;

            float delay = startTime - audioSource.time;
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            Debug.Log($"Бит {bit.value} создаётся в {startTime} сек, чтобы прибыть в {bit.time} сек");

            GetBitLine(bit.value)?.CreateBit(new DataBit(bit));
        }

        yield return new WaitForSeconds(5);

        audioSource.Pause();

        GiveVictory();
    }

    private async Task GiveVictory()
    {
        int maxScore = SelectedSong.Bits.Count * scores.Max(s => s.num);
        int nowScore = Score;

        float scoreP = nowScore / maxScore;
        int res = await NetServerController.Instance.GetPrize(scoreP);

        ShowPrizePanel(res, nowScore, maxScore);
    }

    private void ShowPrizePanel(int prize, int nowScore, int maxScore)
    {
        prizeStatistic.gameObject.SetActive(true);
        prizeStatistic.SetData(SelectedSong.Name, maxScore, nowScore, prize);
    }

    private BitLine GetBitLine(int value)
    {
        return value switch
        {
            0 => bitLineA,
            1 => bitLineW,
            2 => bitLineS,
            3 => bitLineD,
            _ => null
        };
    }
}
