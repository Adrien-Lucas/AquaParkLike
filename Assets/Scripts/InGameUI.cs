using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    public Button restartButton;
    [SerializeField] private Image progressionBar;
    [SerializeField] private TextMeshProUGUI positionText;

    private bool gameEnded;
    Movements playerMovements;
    Movements[] allMovements;
    private List<float> leaderboard;
    
    private void Start()
    {
        startButton.onClick.AddListener(StartButton);
        restartButton.onClick.AddListener(RestartButton);
        
        Initialize();
    }

    public void Initialize()
    {
        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
        
        playerMovements = FindObjectOfType<Player>().GetComponent<Movements>();
        allMovements = FindObjectsOfType<Movements>();
    }

    private void Update()
    {
        if (gameEnded) return; //Scores are not calculated when the player as ended

        progressionBar.fillAmount = (float)playerMovements.actualWaypoint / TobogganGenerator.TotalPath.Count;
        leaderboard = allMovements.Select(m => m.posOnPath.y).ToList();
        leaderboard.Sort();

        int position = leaderboard.IndexOf(playerMovements.posOnPath.y) + 1;
        string sufix = position == 1 ? "st" : position == 2 ? "nd" : position == 3 ? "rd" : "th";
        
        positionText.text = position + sufix;
    }

    public void End()
    {
        restartButton.gameObject.SetActive(true);
        gameEnded = true;
    }

    public void StartButton()
    {
        Movements.Moving = true;
        startButton.gameObject.SetActive(false);
        FindObjectOfType<Player>().InitializeCamera();
    }

    public void RestartButton()
    {
        //It would have been much more efficient to reload only the necessary elements, but the scene is light enough to be fully reloaded
        SceneManager.LoadScene(0);
    }
}
