using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AuthenticationMenu : Panel
{
    [SerializeField] private TMP_InputField usernameInput = null;
    [SerializeField] private TMP_InputField passwordInput = null;
    [SerializeField] private Button signinButton = null;
    [SerializeField] private Button signupButton = null;
    [SerializeField] private Button anonymousButton = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        anonymousButton.onClick.AddListener(AnonymousSignIn);
        signinButton.onClick.AddListener(SignIn);
        signupButton.onClick.AddListener(SignUp);
        base.Initialize();
    }

    public override void Open()
    {
        usernameInput.text = "";
        passwordInput.text = "";
        base.Open();
    }

    private void AnonymousSignIn()
    {
        SingUpManager.Singleton.SignInAnonymouslyAsync();
    }

    private void SignIn()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();
        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
        {
            SingUpManager.Singleton.SignInWithUsernameAndPasswordAsync(user, pass);

            // Guardar el nombre del jugador para el leaderboard
            SavePlayerName(user);
        }
    }

    private void SignUp()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();
        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
        {
            if (IsPasswordValid(pass))
            {
                SingUpManager.Singleton.SignUpWithUsernameAndPasswordAsync(user, pass);

                // Guardar el nombre del jugador para el leaderboard
                SavePlayerName(user);
            }
            else
            {
                ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
                panel.Open(ErrorMenu.Action.None, "Password does not match requirements. Insert at least 1 uppercase, 1 lowercase, 1 digit and 1 symbol. With minimum 8 and a maximum of 30 characters.", "OK");
            }
        }
    }

    private bool IsPasswordValid(string password)
    {
        if (password.Length < 8 || password.Length > 30)
        {
            return false;
        }

        bool hasUppercase = false;
        bool hasLowercase = false;
        bool hasDigit = false;
        bool hasSymbol = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUppercase = true;
            else if (char.IsLower(c)) hasLowercase = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (!char.IsLetterOrDigit(c)) hasSymbol = true;
        }
        return hasUppercase && hasLowercase && hasDigit && hasSymbol;
    }

    private void SavePlayerName(string playerName)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        Debug.Log($"Player name saved: {playerName}");
    }

    public static string GetPlayerName()
    {
        return PlayerPrefs.GetString("PlayerName", "Guest");
    }
}
