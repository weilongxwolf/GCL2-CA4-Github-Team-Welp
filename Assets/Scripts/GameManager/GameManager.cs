using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Player Setting
    public float health = 100;
    private float maxHealth = 100f;

    // UI Setting
    public GameObject pauseMenuUI;
    public GameObject deathMenuUI;
    public Image healthBarImage;

    // Effect Settings
    public Volume globalVolume;

    private DepthOfField depthOfField;
    private bool isPaused = false;
    private bool isDead = false;

    private void Start()
    {
        // Try to extract the Depth of Field component from the assigned volume profile
        if (globalVolume != null && globalVolume.profile.TryGet(out depthOfField))
        {
            // Ensure the blur is turned off when the game starts
            depthOfField.active = false;
        }

        // Initialize the health bar view at start
        UpdateHealthUI();
    }
    public void ChangeHealth(float changeAmount)
    {
        // Ignore all damage if the player is already dead
        if (isDead) return;

        health += changeAmount;

        // Keep health clamped between 0 and max health values
        health = Mathf.Clamp(health, 0f, maxHealth);

        UpdateHealthUI();

        if (health <= 0f)
        {
            Die();
        }
    }
    private void UpdateHealthUI()
    {
        // Check if the image reference exists before modifying it
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = health / maxHealth;
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
    public void Restart()
    {
        Time.timeScale = 1f; // Unfreeze time so the game doesn't restart stuck
        SceneManager.LoadScene("Level");
    }
    public void MainMenu()
    {
        Time.timeScale = 1f; // Unfreeze time so the game doesn't restart stuck
        SceneManager.LoadScene("MainMenu");
    }
    public void StartLevel()
    {
        // Ensure the mouse cursor hides and locks when entering the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("Level");
    }
    public void Credit()
    {
        SceneManager.LoadScene("Credit");
    }
    public void Quit()
    {
        Application.Quit();
    }
    private void TakeDamageFromZombie(float damageAmount)
    {
        // Pass a negative value to change health downward
        ChangeHealth(-damageAmount);
    }
    private void Die()
    {
        isDead = true;

        // Find the Player object using your tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            // Grab the PlayerInput component and disable it completely
            UnityEngine.InputSystem.PlayerInput playerInput = playerObj.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false; // This will now compile perfectly!
            }
        }

        // Show death screen and camera overlay effects
        if (deathMenuUI != null) deathMenuUI.SetActive(true);
        if (depthOfField != null) depthOfField.active = true;

        // Start count down
        StartCoroutine(RespawnSequence());
    }
    IEnumerator RespawnSequence()
    {
        // Wait exactly 5 seconds before player spawn
        yield return new WaitForSeconds(5f);

        // Reload the game level
        SceneManager.LoadScene("Level");
    }
}
