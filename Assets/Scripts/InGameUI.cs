using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(StartButton);
    }
    
    public void StartButton()
    {
        Movements.Moving = true;
        startButton.gameObject.SetActive(false);
        FindObjectOfType<Player>().InitializeCamera();
    }
}
