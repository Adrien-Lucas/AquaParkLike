using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Attach to : Canvas
/// This components manages all the user interfaces of the game
/// </summary>
public class InGameUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image progressionBar;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private Button mushroomButton;
    [SerializeField] private TextMeshProUGUI mushroomCountText;
    [SerializeField] private int mushroomCount;
    
    private bool gameEnded;
    Movements playerMovements;
    Movements[] allMovements;
    private List<float> leaderboard;
    
    private void Start()
    {
        startButton.onClick.AddListener(StartButton);
        restartButton.onClick.AddListener(RestartButton);
        mushroomButton.onClick.AddListener(MushroomButton);
        
        Initialize();
    }

    public void Initialize() //Used for UI initialization
    {
        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
        mushroomButton.gameObject.SetActive(false);
        mushroomCountText.text = "x" + mushroomCount;

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

    /// <summary>
    /// This function changes the UI to the "End" UI
    /// </summary>
    public void End()
    {
        mushroomButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
        gameEnded = true;
    }

    /// <summary>
    /// Called when the start button is pressed
    /// </summary>
    private void StartButton()
    {
        Movements.Moving = true;
        startButton.gameObject.SetActive(false);
        mushroomButton.gameObject.SetActive(true);
        FindObjectOfType<Player>().InitializeCamera();
    }

    /// <summary>
    /// Called when the restart button is pressed
    /// </summary>
    private void RestartButton()
    {
        //It would have been much more efficient to reload only the necessary elements, but the scene is light enough to be fully reloaded
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Called when the mushroom boost button is pressed
    /// </summary>
    private void MushroomButton()
    {
        if (mushroomCount > 0)
        {
            mushroomCount--;
            FindObjectOfType<Player>().GetComponent<Movements>().ApplyTempMultiplicator(1.5f, 3);
            mushroomCountText.text = "x" + mushroomCount;
        }
        
        if(mushroomCount <= 0)
        {
            mushroomCountText.text = "";
            mushroomButton.interactable = false;
        }
    }
}
