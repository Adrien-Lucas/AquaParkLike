using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image progressionBar;
    [SerializeField] private TextMeshProUGUI positionText;

    Movements playerMovements;
    Movements[] allMovements;
    private List<float> leaderboard;
    
    private void Start()
    {
        startButton.onClick.AddListener(StartButton);
        restartButton.onClick.AddListener(RestartButton);
        
        playerMovements = FindObjectOfType<Player>().GetComponent<Movements>();
        allMovements = FindObjectsOfType<Movements>();
    }

    private void Update()
    {
        progressionBar.fillAmount = (float)playerMovements.actualWaypoint / TobogganGenerator.TotalPath.Count;
        leaderboard = allMovements.Select(m => m.posOnPath.y).ToList();
        leaderboard.Sort();

        int position = leaderboard.IndexOf(playerMovements.posOnPath.y) + 1;
        string sufix = position == 1 ? "st" : position == 2 ? "nd" : position == 3 ? "rd" : "th";
        
        positionText.text = position + sufix;
    }

    public void StartButton()
    {
        Movements.Moving = true;
        startButton.gameObject.SetActive(false);
        FindObjectOfType<Player>().InitializeCamera();
    }

    public void RestartButton()
    {
        
    }
}
