using UnityEngine;

public interface IMinigame
{
    void MinigameButton();
    void MinigameTrigger(float delta);
    void MinigameStick(Vector2 input);
}