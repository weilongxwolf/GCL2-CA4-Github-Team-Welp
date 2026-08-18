using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone is the player
        if (other.CompareTag("Player"))
        {
            // Automatically find your GameManager script in the scene
            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();

            if (gameManager != null)
            {
                //  Turn on the Win UI from the GameManager
                if (gameManager.winMenuUI != null) gameManager.winMenuUI.SetActive(true);

                // Turn on the blur effect if you have it set up
                if (gameManager.globalVolume != null) gameManager.globalVolume.gameObject.SetActive(true);

                // Freeze all physical game movement
                Time.timeScale = 0f;

                // Start the 5-second countdown before going to the menu
                StartCoroutine(WinSequence());

                // Turn off this trigger so it doesn't run twice
                gameObject.SetActive(false);
            }
        }
    }

    IEnumerator WinSequence()
    {
        // Waits 5 real-world seconds while the game is frozen
        yield return new WaitForSecondsRealtime(5f);

        // Unfreeze time before loading so the next scene works fine
        Time.timeScale = 1f;

        // Free the mouse cursor so you can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Go back to the main menu
        SceneManager.LoadScene("MainMenu");
    }
}
