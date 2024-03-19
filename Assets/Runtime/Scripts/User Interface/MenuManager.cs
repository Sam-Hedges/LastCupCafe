using System;   
using UnityEngine;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private InputController inputController;
    [Space]
    [SerializeField] private UnityEvent startGameEvent;
    
    private LevelManager levelManager;

    private void Start() {
         //levelManager = gameObject.AddComponent<LevelManager>();
    }
    
    // private void OnEnable() {
    //     inputController.EnableMenuInput();
    //     inputController.AnyInputEvent += LoadMenu;
    // }
    //
    // private void OnDisable() {
    //     inputController.DisableAllInput();
    //     inputController.AnyInputEvent -= LoadMenu;
    // }
    //
    // internal void LoadMenu() {
    //     startGameEvent.Invoke();
    //     inputController.AnyInputEvent -= LoadMenu;
    // }

    public void ExitApplicatation() {
        Application.Quit();
    }

    public void StartGame() {
        levelManager.LoadScene("SampleScene");
    }
}