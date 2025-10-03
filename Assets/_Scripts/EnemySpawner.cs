using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    private Camera mainCamera;
    private int enemyIndex = 0;

    void Awake() {
        mainCamera = Camera.main;
    }

    public IEnumerator SpawnWave(WaveConfig config) {
        enemyIndex = 0;

        for (int i = 0; i < config.enemyPrefabs.Length; i++) {
            SpawnEnemy(config);
            yield return new WaitForSeconds(config.spawnInterval);
        }
    }

    void SpawnEnemy(WaveConfig config) {
        GameObject enemyPrefab = config.enemyPrefabs[enemyIndex];

        float camWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float spawnY = mainCamera.transform.position.y + mainCamera.orthographicSize + 2f;

        // Get half-width of the enemy sprite in world units
        float halfWidth = 0f;
        SpriteRenderer sr = enemyPrefab.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) {
            halfWidth = sr.bounds.extents.x;
        }

        // Adjust X range so entire sprite stays in screen bounds
        float spawnX = UnityEngine.Random.Range(-camWidth + halfWidth, camWidth - halfWidth);

        Vector3 spawnPos = new(spawnX, spawnY, 0f);
        Quaternion rot = Quaternion.Euler(0, 0, 180);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, rot);

        ApplyDifficultyScaling(enemy);

        StartCoroutine(EnableColliderOnScreen(enemy));

        // Cycle to next enemy in list
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

    void ApplyDifficultyScaling(GameObject enemy) {
        float multiplier = GetComponent<WaveManager>().DifficultyMultiplier;

        if (enemy.TryGetComponent<EnemyStats>(out var stats)) {
            stats.MaxHealth = Mathf.RoundToInt(stats.MaxHealth * multiplier);
            stats.SetCurrentHealth(stats.MaxHealth);
            stats.healthBar.MaxValue = Convert.ToInt32(stats.MaxHealth);
            stats.healthBar.Change(0); // Refresh UI
            Debug.Log("[Enemy Spawner] " + enemy.name + ": New Max Health is: " + stats.MaxHealth);
        }

        if (enemy.TryGetComponent<EnemyShooting>(out var shooter)) {
            shooter.bulletPrefab.GetComponent<EnemyBullet>().damage *= multiplier;
            Debug.Log("[Enemy Spawner] " + enemy.name + ": New Max Damage is: " + shooter.bulletPrefab.GetComponent<EnemyBullet>().damage);
        }
    }

}
