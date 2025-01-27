using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine.UI;

public class MainMenu : Panel
{

    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private Button logoutButton = null;
    [SerializeField] private Button leaderboardsButton = null;
    
    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        logoutButton.onClick.AddListener(SignOut);
        leaderboardsButton.onClick.AddListener(Leaderboards);
        base.Initialize();
    }
    
    public override void Open()
    {
        UpdatePlayerNameUI();
        base.Open();
    }
    
    private void SignOut()
    {
        SingUpManager.Singleton.SignOut();
    }
    
    private void UpdatePlayerNameUI()
    {
        nameText.text = AuthenticationMenu.GetPlayerName();
    }
    
    private void Leaderboards()
    {
        PanelManager.Open("leaderboards");
    }

}