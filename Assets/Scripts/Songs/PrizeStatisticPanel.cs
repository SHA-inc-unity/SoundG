using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrizeStatisticPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text songName, scores, prizeSize;
    [SerializeField]
    private List<TMP_Text> leaderTable;

    public void SetData(string songName, int scoresMax, int scoresNow, int prizeSize)
    {
        this.songName.text = songName;
        this.scores.text = $"Score: {scoresNow} / {scoresMax}";
        this.prizeSize.text = $"Added meat coin: {prizeSize}";

        /// Сделать лидертабл
        for (int i = 0; i < leaderTable.Count; i++)
        {

        }
    }
}
