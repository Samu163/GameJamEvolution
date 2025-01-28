using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

public class LeaderboardsPlayerItem : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI rankText = null;
    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] public TextMeshProUGUI scoreText = null;

    private LeaderboardEntry player = null;

    private void Start()
    {
    }

    public void Initialize(LeaderboardEntry player)
    {
        this.player = player;

        rankText.text = (player.Rank + 1).ToString();

        // Manejar el caso donde Metadata sea null
        string playerName = "Unknown";
        if (!string.IsNullOrEmpty(player.Metadata))
        {
            try
            {
                var metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(player.Metadata);
                if (metadata != null && metadata.TryGetValue("PlayerName", out string name))
                {
                    playerName = name;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing metadata: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Metadata is null or empty for player.");
        }

        nameText.text = playerName;

        scoreText.text = player.Score.ToString();

        string currentPlayerName = AuthenticationMenu.GetPlayerName();
        if (playerName == currentPlayerName)
        {
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
