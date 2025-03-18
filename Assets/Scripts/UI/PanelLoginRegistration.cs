using TMPro;
using UnityEngine;

public class PanelLoginRegistration : MonoBehaviour
{
    [SerializeField]
    private PanelController panelController;
    [SerializeField]
    private TMP_InputField nameLoginTextEditor, emailLoginTextEditor, passwordLoginTextEditor;
    [SerializeField]
    private TMP_InputField nameRegistrationTextEditor, emailRegistrationTextEditor, passwordRegistrationTextEditor, repasswordRegistrationTextEditor;

    public void Login()
    {
        Login(nameLoginTextEditor.text, passwordLoginTextEditor.text);
    }

    public async void Login(string name, string pass)
    {
        (string result, string name, int coin) task = await NetServerController.Instance.Login(name, pass);
        Debug.Log(task);

        if (task.result == "true")
        {
            LoginData.AddUserData(new UserData(task.name, pass, task.coin));
            panelController.ChangePanel(PanelsName.startGame);
        }
    }

    public async void Registration()
    {
        if (passwordRegistrationTextEditor.text == repasswordRegistrationTextEditor.text)
        {
            string task = await NetServerController.Instance.Register(nameRegistrationTextEditor.text, emailRegistrationTextEditor.text, passwordRegistrationTextEditor.text);
            if (task == "true")
                Login(nameRegistrationTextEditor.text, passwordRegistrationTextEditor.text);
            else
                Debug.LogError(task);
        }
    }
}
