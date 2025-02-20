using UnityEngine;
using UnityEngine.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

public class PanelStartGame : MonoBehaviour
{
    [SerializeField]
    private Button setGameLevel, createMusicBits, publishMusic;

    private void Start()
    {
        CheckOnlineFunc(LoginData.IsLogin);

        LoginData.OnLoginStatusChanged += HandleLoginStatusChange;
    }

    private void OnDestroy()
    {
        LoginData.OnLoginStatusChanged -= HandleLoginStatusChange;
    }

    private void HandleLoginStatusChange(bool isLoggedIn)
    {
        CheckOnlineFunc(isLoggedIn);
    }

    private void CheckOnlineFunc(bool isOnline)
    {
        if (isOnline)
        {
            setGameLevel.interactable = true;
            createMusicBits.interactable = true;
            publishMusic.interactable = true;
        }
        else
        {
            setGameLevel.interactable = true;
            createMusicBits.interactable = false;
            publishMusic.interactable = false;
        }
    }
}
