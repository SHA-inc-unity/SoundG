using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BitLine : MonoBehaviour
{
    public Vector3 GetStartPosition() => start.transform.position;
    public Vector3 GetEndPosition() => clicker.transform.position;
    public float GetSpeed() => speed;

    [SerializeField]
    private GameObject start, end, clicker;
    [SerializeField]
    private GameObject bit;
    [SerializeField] 
    private float speed = 5f; // Скорость движения битов
    [SerializeField]
    private string tochedWASD;

    private List<GameObject> bits = new List<GameObject>();


    public void CreateBit()
    {
        bits.Add(Instantiate(bit, start.transform));
    }

    private void Update()
    {
        if (Input.GetKeyDown(GetKey(tochedWASD)))
        {
            List<int> removeId = new List<int>();
            foreach (GameObject item in bits)
            {
                if (item != null && clicker != null)
                {
                    float distance = Vector3.Distance(item.transform.position, clicker.transform.position);
                    Debug.Log($"Расстояние до clicker: {distance}");
                    if (distance < 0.8f)
                    {
                        removeId.Add(bits.IndexOf(item));
                    }
                }
            }
            foreach (var id in removeId)
            {
                Destroy(bits[id]);
                bits.RemoveAt(id);
            }
        }

        MoveBits();
    }

    private KeyCode GetKey(string str)
    {
        switch (str)
        {
            case "A":
                return KeyCode.A;
            case "W":
                return KeyCode.W;
            case "S":
                return KeyCode.S;
            case "D":
                return KeyCode.D;
            default:
                return KeyCode.None;
        }
    }

    private void MoveBits()
    {
        for (int i = bits.Count - 1; i >= 0; i--)
        {
            if (bits[i] != null)
            {
                bits[i].transform.position = Vector3.MoveTowards(bits[i].transform.position, end.transform.position, speed * Time.deltaTime);

                if (Vector3.Distance(bits[i].transform.position, end.transform.position) < 0.1f)
                {
                    Destroy(bits[i]);
                    bits.RemoveAt(i);
                }
            }
        }
    }
}
