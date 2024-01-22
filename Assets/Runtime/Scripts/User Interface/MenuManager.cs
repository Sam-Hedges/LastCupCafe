using System;   
using UnityEngine;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [Space]
    [SerializeField] private UnityEvent startGameEvent;
    
    private LevelManager levelManager;

    private void Start() {
         //levelManager = gameObject.AddComponent<LevelManager>();
    }
    
    private void OnEnable() {
        inputHandler.EnableMenuInput();
        inputHandler.StartGameEvent += LoadMenu;
    }
    
    private void OnDisable() {
        inputHandler.DisableAllInput();
        inputHandler.StartGameEvent -= LoadMenu;
    }
    
    internal void LoadMenu() {
        startGameEvent.Invoke();
        inputHandler.StartGameEvent -= LoadMenu;
    }

    public void ExitApplicatation() {
        Application.Quit();
    }

    public void StartGame() {
        levelManager.LoadScene("SampleScene");
    }
}