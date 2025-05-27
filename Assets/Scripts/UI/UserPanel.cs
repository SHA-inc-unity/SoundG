
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text username, coins;
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
            coins.text = $"Coin: {LoginData.UserData.coin}";
        }
        button.interactable = !isLoggedIn;
    }

    private async void Start()
    {
        if (LoginData.IsLogin)
        {
            int updatedCoin = await NetServerController.Instance.GetMeatCoin(LoginData.UserData.name);
            coins.text = $"Coin: {updatedCoin}";
        }

        HandleLoginStatusChange(LoginData.IsLogin);
    }
}
