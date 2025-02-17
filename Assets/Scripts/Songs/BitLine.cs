using NUnit.Framework;
using System;
using System.Collections.Generic;
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
    private float speed = 5f; // Скорость движения битов
    [SerializeField]
    protected string tochedWASD;

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
                    Debug.Log($"Расстояние до clicker: {distance}"); 
                    if (distance < 0.1f)
                    {
                        AddScore();
                        AddScoreEvent();
                        removeId.Add(bits.IndexOf(item));
                    }
                    else if (distance < 0.2f)
                    {
                        removeId.Add(bits.IndexOf(item));
                    }
                    else if (distance < 0.4f)
                    {
                        removeId.Add(bits.IndexOf(item));
                    }
                    else if (distance < 0.8f)
                    {
                        removeId.Add(bits.IndexOf(item));
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
