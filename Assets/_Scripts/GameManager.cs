using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem; // To handle UI

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
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
        scoreText.text = "Score: " + score.ToString();
    }

    public void UpdateWave(int wave) {
        waveNumber = wave;
        waveText.text = "Wave: " + waveNumber.ToString();
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

    private void UpdateUI() {
        scoreText.text = "SCORE: " + score.ToString();
        waveText.text = "WAVE: " + waveNumber.ToString();
    }

    public void QuitGame(){
        Application.Quit();
    }
}
