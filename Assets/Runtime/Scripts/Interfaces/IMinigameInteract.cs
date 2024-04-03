using UnityEngine;

public interface IMinigameInteract

{
    void MinigameButton();
    void MinigameTrigger(float delta);
    void MinigameStick(Vector2 input);
    void GameUI(bool active);
}