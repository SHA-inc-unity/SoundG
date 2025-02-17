using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BitLine : MonoBehaviour
{
    public Vector3 GetStartPosition() => start.transform.position;
    public Vector3 GetEndPosition() => clicker.transform.position;
    public float GetSpeed() => speed;

    [SerializeField]
    private SongDataController songDataController;
    [SerializeField]
    protected GameObject start, end, clicker;
    [SerializeField]
    private GameObject bit;
    [SerializeField] 
    private float speed = 5f;
    [SerializeField]
    protected string tochedWASD;
    [SerializeField]
    private float force = 2f, lifetime = 2f;
    [SerializeField]
    private GameObject textPrefab;

    protected List<(GameObject, DataBit)> bits = new List<(GameObject, DataBit)>();


    public void CreateBit(DataBit dataBit)
    {
        bits.Add((Instantiate(bit, start.transform), dataBit));
    }

    public void CreateBitOnEnd(DataBit dataBit)
    {
        bits.Add((Instantiate(bit, clicker.transform), dataBit));
    }

    public void Clear()
    {
        while (bits.Count != 0)
        {
            Destroy(bits[0].Item1);
            bits.Remove(bits[0]);
        }
    }

    protected void Update()
    {
        if (Input.GetKeyDown(GetKey(tochedWASD)))
        {
            List<int> removeId = new List<int>();
            foreach ((GameObject, DataBit) item in bits)
            {
                if (item.Item1 != null && clicker != null)
                {
                    float distance = Vector3.Distance(item.Item1.transform.position, clicker.transform.position);
                    Scores scores = Scores.Lol;
                    if (distance < 0.1f)
                    {
                        scores = Scores.Perfect;
                    }
                    else if (distance < 0.2f)
                    {
                        scores = Scores.Good;
                    }
                    else if (distance < 0.4f)
                    {
                        scores = Scores.Normal;
                    }
                    else if (distance < 0.8f)
                    {
                        scores = Scores.Bad;
                    }

                    if (scores != Scores.Lol)
                    {
                        ScoreData scoreData = songDataController.AddScore(scores);
                        if (scoreData != null)
                        {
                            AddScoreEvent(scoreData);
                            removeId.Add(bits.IndexOf(item));
                            break;
                        }
                    }
                }
            }
            foreach (var id in removeId)
            {
                Destroy(bits[id].Item1);
                bits.RemoveAt(id);
            }
        }

        MoveBits();
    }

    private void AddScoreEvent(ScoreData scores)
    {
        SpawnText(scores.scores.ToString(), scores.color);
    }

    private void SpawnText(string message, Color color)
    {
        GameObject newTextObj = Instantiate(textPrefab, clicker.transform.position, Quaternion.identity);
        TextMeshPro tmp = newTextObj.GetComponent<TextMeshPro>();

        tmp.text = message;
        tmp.color = color;

        Rigidbody rb = newTextObj.AddComponent<Rigidbody>();
        rb.useGravity = false;

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * force;
        rb.linearVelocity = randomDirection;

        Destroy(newTextObj, lifetime);
    }

    protected KeyCode GetKey(string str)
    {
        if (Enum.TryParse(str.ToUpper(), out KeyCode key))
        {
            return key;
        }
        return KeyCode.None;
    }


    protected void MoveBits()
    {
        for (int i = bits.Count - 1; i >= 0; i--)
        {
            if (bits[i].Item1 != null)
            {
                bits[i].Item1.transform.position = Vector3.MoveTowards(bits[i].Item1.transform.position, end.transform.position, speed * Time.deltaTime);

                if (Vector3.Distance(bits[i].Item1.transform.position, end.transform.position) < 0.1f)
                {
                    Destroy(bits[i].Item1);
                    bits.RemoveAt(i);
                }
            }
        }
    }
}
