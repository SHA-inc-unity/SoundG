using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Audio;

public class BitLineRedactor : BitLine
{
    [SerializeField]
    private SongDataControllerRedactor songDataControllerRedactor;
    [SerializeField]
    private AudioSource redactorSource;
    [SerializeField]
    private int numLine;

    private SoundData selectedSong;

    private void Start()
    {
        selectedSong = songDataControllerRedactor.SelectedSong;
    }

    private new void Update()
    {
        if (Input.GetKeyDown(GetKey(tochedWASD)))
        {
            if (songDataControllerRedactor.IsAddingOrRemoving)
            {
                AddBit();
            }
            else
            {
                RemoveBit();
            }
        }

        MoveBits();
    }

    private void AddBit()
    {
        JsonDataSaver.TimeValuePair timeValuePair = new JsonDataSaver.TimeValuePair(redactorSource.time, numLine);
        selectedSong.Bits.Add(timeValuePair);
        CreateBitOnEnd(new DataBit(timeValuePair));
    }

    private void RemoveBit()
    {
        List<int> removeId = new List<int>();
        foreach ((GameObject, DataBit) item in bits)
        {
            if (item.Item1 != null && clicker != null)
            {
                float distance = Vector3.Distance(item.Item1.transform.position, clicker.transform.position);
                Debug.Log($"Расстояние до clicker: {distance}");
                if (distance < 0.8f)
                {
                    removeId.Add(bits.IndexOf(item));
                }
            }
        }
        foreach (var id in removeId)
        {
            Destroy(bits[id].Item1);
            bits.RemoveAt(id);
            selectedSong.Bits.Remove(bits[id].Item2.data);
        }
    }
}
