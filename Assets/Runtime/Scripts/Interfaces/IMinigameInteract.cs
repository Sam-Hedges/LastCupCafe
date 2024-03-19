using UnityEngine;

public interface IMinigameInteract

{
    void MinigameButton(GameObject heldItem);
    void MinigameTrigger(float delta, GameObject heldItem);
    void MinigameStick(Vector2 input, GameObject heldItem);
    void GameUI(bool active);
}