using System.Collections;
using TMPro;
using UnityEngine;

public class AttenshonManager : MonoBehaviour
{
    public static AttenshonManager Instanse;
    [SerializeField]
    private GameObject Attenshon;
    [SerializeField]
    private TMP_Text text;
    private Coroutine currentCoroutine;

    private void Start()
    {
        Instanse = this;
    }

    public void ShowMessage(string message)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ShowAndHide(message));
    }

    private IEnumerator ShowAndHide(string message)
    {
        text.text = message;
        Attenshon.SetActive(true);
        yield return new WaitForSeconds(5f);
        Attenshon.SetActive(false);
        currentCoroutine = null;
    }
}
