using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    private Camera mainCamera;
    private int enemyIndex = 0;

    void Awake() {
        mainCamera = Camera.main;
    }

    public IEnumerator SpawnWave(WaveConfig config) {
        for (int i = 0; i < config.enemiesToSpawn; i++) {
            SpawnEnemy(config);
            yield return new WaitForSeconds(config.spawnInterval);
        }
    }

    void SpawnEnemy(WaveConfig config) {
        // Use enemyIndex to spawn enemies in order
        GameObject enemyPrefab = config.enemyPrefabs[enemyIndex];

        float camWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float spawnX = Random.Range(-camWidth, camWidth);
        float spawnY = mainCamera.transform.position.y + mainCamera.orthographicSize + 2f;

        Vector3 spawnPos = new(spawnX, spawnY, 0f);
        Quaternion rot = Quaternion.Euler(0, 0, 180);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, rot);

        // Start coroutine to enable the collider once the enemy is on screen
        StartCoroutine(EnableColliderOnScreen(enemy));

        // Increment the enemyIndex and wrap around if it exceeds the array length
        enemyIndex = (enemyIndex + 1) % config.enemyPrefabs.Length;
    }



    IEnumerator EnableColliderOnScreen(GameObject enemy) {
        if (enemy.TryGetComponent<Collider2D>(out var col)) col.enabled = false;

        while (enemy != null) {
            Vector3 viewPos = mainCamera.WorldToViewportPoint(enemy.transform.position);
            if (viewPos.y < 1f && viewPos.y > 0f && viewPos.x > 0f && viewPos.x < 1f) {
                if (col != null) col.enabled = true;
                break;
            }
            yield return null;
        }
    }
}
