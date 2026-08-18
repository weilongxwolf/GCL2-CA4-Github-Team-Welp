using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public int health = 100;

    // UI Setting
    public GameObject pauseMenuUI;

    // Effect Stting
    public Volume globalVolume;

    private DepthOfField depthOfField;
    private bool isPaused = false;

    private void Start()
    {
        // Try to extract the Depth of Field component from the assigned volume profile
        if (globalVolume != null && globalVolume.profile.TryGet(out depthOfField))
        {
            // Ensure the blur is turned off when the game starts
            depthOfField.active = false;
        }
    }
    public void OnPause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    public void ResumeGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false); // Turn off Pause UI

        // Toggle DOF component off
        if (depthOfField != null) depthOfField.active = false;

        Time.timeScale = 1f; // Unfreeze time
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void PauseGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true); // Turn on Pause UI

        // Toggle DOF component on
        if (depthOfField != null) depthOfField.active = true;

        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Damage")
        {

        }
    }

    private void DecreasedHealth(int decreaseAmount)
    {
        health -= decreaseAmount;

        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Time.timeScale = 0f;
    }

}
