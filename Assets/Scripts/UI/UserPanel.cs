
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text username;
    [SerializeField]
    private Button button;

    private void Awake()
    {
        LoginData.OnLoginStatusChanged += HandleLoginStatusChange;
    }

    private void OnDestroy()
    {
        LoginData.OnLoginStatusChanged -= HandleLoginStatusChange;
    }

    private void HandleLoginStatusChange(bool isLoggedIn)
    {
        if (LoginData.UserData != null && LoginData.UserData.name != null)
        {
            username.text = LoginData.UserData.name;
        }
        button.interactable = !isLoggedIn;
    }

    private void Start()
    {
        HandleLoginStatusChange(LoginData.IsLogin);
    }
}
