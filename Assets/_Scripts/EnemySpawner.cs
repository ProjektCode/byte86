using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    private Camera mainCamera;

    void Awake() {
        mainCamera = Camera.main;
    }

    public IEnumerator SpawnWave(WaveConfig config) {
        for (int i = 0; i < config.enemiesToSpawn; i++) {
            SpawnEnemy(config);
            yield return new WaitForSeconds(config.spawnInterval);
        }
    }

    IEnumerator SpawnEnemies(WaveConfig config) {
        for (int i = 0; i < config.enemiesToSpawn; i++) {
            SpawnEnemy(config);
            yield return new WaitForSeconds(config.spawnInterval);
        }
    }

    void SpawnEnemy(WaveConfig config) {
        GameObject enemyPrefab = config.enemyPrefabs[Random.Range(0, config.enemyPrefabs.Length)];

        float camWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float spawnX = Random.Range(-camWidth, camWidth);
        float spawnY = mainCamera.transform.position.y + mainCamera.orthographicSize + 2f;

        Vector3 spawnPos = new(spawnX, spawnY, 0f);
        Quaternion rot = Quaternion.Euler(0, 0, 180);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, rot);

        StartCoroutine(EnableColliderOnScreen(enemy));
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
