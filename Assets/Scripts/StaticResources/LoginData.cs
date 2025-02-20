using System;
using UnityEngine;

public static class LoginData
{

    public static bool IsLogin
    {
        get => _isLogin;
        set
        {
            if (_isLogin != value) // Проверяем, изменилось ли значение
            {
                _isLogin = value;
                OnLoginStatusChanged?.Invoke(_isLogin); // Вызываем событие
            }
        }
    }

    public static UserData UserData { get => _userData; }

    public static event Action<bool> OnLoginStatusChanged;

    private static UserData _userData;

    private static bool _isLogin;


    public static void AddUserData(UserData userData)
    {
        _userData = userData;
        IsLogin = true;
    }
}

public class UserData
{
    public string name, password;

    public UserData(string name, string password)
    {
        this.name = name;
        this.password = password;
    }
}