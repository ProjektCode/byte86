using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [System.Serializable]
    public class Wave {
        public GameObject[] enemyPrefabs;
        public int enemiesToSpawn = 5;
        public float spawnInterval = 1f;
    }

    public Wave[] waves;
    public float waveDelay = 3f;

    private Camera mainCamera;
    private int currentWave = 0;
    private bool spawning = false;

    void Start() {
        mainCamera = Camera.main;
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves() {
        yield return new WaitForSeconds(2f); // Short delay before first wave

        while (currentWave < waves.Length) {
            spawning = true;
            Wave wave = waves[currentWave];

            for (int i = 0; i < wave.enemiesToSpawn; i++)
            {
                SpawnEnemy(wave);
                yield return new WaitForSeconds(wave.spawnInterval);
            }

            spawning = false;
            currentWave++;
            yield return new WaitForSeconds(waveDelay);
        }
    }

    void SpawnEnemy(Wave wave) {
        // Pick random enemy from wave
        GameObject enemyPrefab = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Length)];

        // Get random X position within screen bounds
        float camWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float spawnX = Random.Range(-camWidth, camWidth);

        // Spawn just above the top of the screen
        float spawnY = mainCamera.transform.position.y + mainCamera.orthographicSize + 2f;

        Vector3 spawnPos = new(spawnX, spawnY, 0f);
        Quaternion rot = Quaternion.Euler(0, 0, 180);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, rot);

        // Disable collider until on-screen
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
