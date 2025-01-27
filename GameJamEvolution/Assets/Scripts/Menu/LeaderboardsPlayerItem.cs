using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine.UI;

public class LeaderboardsPlayerItem : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI rankText = null;
    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] public TextMeshProUGUI scoreText = null;
    [SerializeField] private Button selectButton = null;

    private LeaderboardEntry player = null;

    private void Start()
    {
        selectButton.onClick.AddListener(Clicked);
    }

    public void Initialize(LeaderboardEntry player)
    {
        this.player = player;
        rankText.text = (player.Rank + 1).ToString();
        nameText.text = player.PlayerName;
        scoreText.text = player.Score.ToString();

        // Verificar si este jugador es el usuario actual
        string currentPlayerName = AuthenticationMenu.GetPlayerName();
        if (player.PlayerName == currentPlayerName)
        {
            selectButton.interactable = false;
            HighlightCurrentPlayer(); 
        }
    }

    private void Clicked()
    {
        if (player != null)
        {
            Debug.Log($"TODO -> Open profile for player: {player.PlayerName}");
        }
    }

    private void HighlightCurrentPlayer()
    {
        nameText.color = Color.yellow;
        rankText.color = Color.yellow;
        scoreText.color = Color.yellow;
    }
}
