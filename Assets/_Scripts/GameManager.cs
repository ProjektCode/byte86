using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    private GameObject currentPlayer;

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
    }

    public void SpawnPlayer() {
        if (playerPrefab != null && playerSpawnPoint != null) {
            currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        } else {
            Debug.LogWarning("Missing playerPrefab or playerSpawnPoint on GameManager.");
        }
    }

    public GameObject GetPlayer() => currentPlayer;
}
