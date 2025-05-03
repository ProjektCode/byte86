using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public Image OffenceImageSlot1;
    public Image OffenceImageSlot2;
    public Image SupportImageSlot1;
    public Image SupportImageSlot2;
    [Space]
    public GameObject gameOverPanel;


    private GameObject currentPlayer;
    private int score = 0;
    private int waveNumber = 1;

    void Awake() {
        // Singleton pattern
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        SpawnPlayer();
        UpdateUI();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) QuitGame();
    }

    public void SpawnPlayer() {
        if (playerPrefab != null && playerSpawnPoint != null) {
            currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        } else {
            Debug.LogWarning("Missing playerPrefab or playerSpawnPoint on GameManager.");
        }
    }

    public GameObject GetPlayer() => currentPlayer;

    // Update score and wave UI
    public void UpdateScore(int points) {
        score += points;
        scoreText.text = "SCORE: " + score.ToString();
    }

    public void UpdateWave(int wave) {
        waveNumber = wave;
        waveText.text = "WAVE: " + waveNumber.ToString();
    }

    // Handle game over
    public void GameOver() {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Stop the game (pause)
    }

    // Restart the game
    public void RestartGame() {

        // Destroy existing player
        if (currentPlayer != null) Destroy(currentPlayer);
        
        gameOverPanel.SetActive(false); // Hide game over screen

        // Destroy all existing enemies
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            Destroy(enemy);
        }

        // Destroy all existing enemies
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet")) {
            Destroy(bullet);
        }

         // Reset wave progression
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null) waveManager.ResetWaves();

        score = 0; // Reset score
        waveNumber = 1; // Reset wave
        UpdateUI();
        SpawnPlayer(); // Respawn player
        Time.timeScale = 1f; // Resume time
    }

    public void UpdatePowerupUI(PowerupData powerup, int slotIndex) {
        if (powerup == null) return;

        switch (powerup.PowerupType) {
            case PowerupData.PowerupCategory.Offensive:
                if (slotIndex == 0 && OffenceImageSlot1 != null) {
                    OffenceImageSlot1.sprite = powerup.icon;
                    OffenceImageSlot1.enabled = true;
                } else if (slotIndex == 1 && OffenceImageSlot2 != null) {
                    OffenceImageSlot2.sprite = powerup.icon;
                    OffenceImageSlot2.enabled = true;
                }
                break;

            case PowerupData.PowerupCategory.Support:
                if (slotIndex == 0 && SupportImageSlot1 != null) {
                    SupportImageSlot1.sprite = powerup.icon;
                    SupportImageSlot1.enabled = true;
                } else if (slotIndex == 1 && SupportImageSlot2 != null) {
                    SupportImageSlot2.sprite = powerup.icon;
                    SupportImageSlot2.enabled = true;
                }
                break;
        }
    }

    public void ClearPowerupUI(PowerupData.PowerupCategory category, int slotIndex) {
        switch (category) {
            case PowerupData.PowerupCategory.Offensive:
                if (slotIndex == 0 && OffenceImageSlot1 != null) {
                    OffenceImageSlot1.sprite = null;
                    OffenceImageSlot1.enabled = false;
                } else if (slotIndex == 1 && OffenceImageSlot2 != null) {
                    OffenceImageSlot2.sprite = null;
                    OffenceImageSlot2.enabled = false;
                }
                break;

            case PowerupData.PowerupCategory.Support:
                if (slotIndex == 0 && SupportImageSlot1 != null) {
                    SupportImageSlot1.sprite = null;
                    SupportImageSlot1.enabled = false;
                } else if (slotIndex == 1 && SupportImageSlot2 != null) {
                    SupportImageSlot2.sprite = null;
                    SupportImageSlot2.enabled = false;
                }
                break;
        }
    }

    private void UpdateUI() {
        scoreText.text = "SCORE: " + score.ToString();
        waveText.text = "WAVE: " + waveNumber.ToString();

        OffenceImageSlot1.enabled = false;
        OffenceImageSlot2.enabled = false;
        SupportImageSlot1.enabled = false;
        SupportImageSlot2.enabled = false;
    }

    public void QuitGame(){
        Application.Quit();
    }
}
