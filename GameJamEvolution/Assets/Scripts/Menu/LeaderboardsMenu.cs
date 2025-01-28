using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;

public class LeaderboardsMenu : Panel
{
    [SerializeField] private int playersPerPage = 25;
    [SerializeField] private LeaderboardsPlayerItem playerItemPrefab = null;
    [SerializeField] private RectTransform playersContainer = null;
    [SerializeField] public TextMeshProUGUI pageText = null;
    [SerializeField] private Button nextButton = null;
    [SerializeField] private Button prevButton = null;
    [SerializeField] private Button closeButton = null;

    private int currentPage = 1;
    private int totalPages = 0;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        ClearPlayersList();
        closeButton.onClick.AddListener(ClosePanel);
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);
        base.Initialize();
    }

    public override void Open()
    {
        pageText.text = "-";
        nextButton.interactable = false;
        prevButton.interactable = false;
        base.Open();
        ClearPlayersList();
        currentPage = 1;
        totalPages = 0;
        LoadPlayers(1);
    }

    private void AddScore()
    {
        AddScoreAsync(10); // Cambia el puntaje según tus necesidades
    }
    public async void AddScoreAsync(int score)
    {
        try
        {
            // Obtener el nombre del jugador desde PlayerPrefs
            string playerName = AuthenticationMenu.GetPlayerName();

            // Crear un diccionario con los metadatos
            var metadata = new Dictionary<string, object>
        {
            { "PlayerName", playerName } // Metadato con el nombre del jugador
        };

            // Crear las opciones para enviar la puntuación
            var options = new AddPlayerScoreOptions
            {
                Metadata = metadata // Agregar el diccionario de metadatos
            };

            // Enviar la puntuación con las opciones
            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(
                "test", // Nombre del leaderboard
                score,
                options
            );

            Debug.Log($"Score {score} submitted for player {playerName}");
            LoadPlayers(currentPage);
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to add score: {exception.Message}");
        }
       
    }

    public async void LoadPlayers(int page)
    {
        nextButton.interactable = false;
        prevButton.interactable = false;
        try
        {
            GetScoresOptions options = new GetScoresOptions
            {
                Offset = (page - 1) * playersPerPage,
                Limit = playersPerPage,
                IncludeMetadata = true
            };

            var scores = await LeaderboardsService.Instance.GetScoresAsync("test", options);
            ClearPlayersList();

            foreach (var score in scores.Results)
            {
                LeaderboardsPlayerItem item = Instantiate(playerItemPrefab, playersContainer);

                item.Initialize(score);
            }
            totalPages = Mathf.CeilToInt((float)scores.Total / playersPerPage);
            currentPage = page;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to load players: {exception.Message}");
        }

        pageText.text = $"{currentPage}/{totalPages}";
        nextButton.interactable = currentPage < totalPages && totalPages > 1;
        prevButton.interactable = currentPage > 1 && totalPages > 1;
    }
    private void NextPage()
    {
        if (currentPage + 1 > totalPages)
        {
            LoadPlayers(1);
        }
        else
        {
            LoadPlayers(currentPage + 1);
        }
    }

    private void PrevPage()
    {
        if (currentPage - 1 <= 0)
        {
            LoadPlayers(totalPages);
        }
        else
        {
            LoadPlayers(currentPage - 1);
        }
    }

    private void ClosePanel()
    {
        Close();
    }

    private void ClearPlayersList()
    {
        LeaderboardsPlayerItem[] items = playersContainer.GetComponentsInChildren<LeaderboardsPlayerItem>();
        if (items != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Destroy(items[i].gameObject);
            }
        }
    }
}
