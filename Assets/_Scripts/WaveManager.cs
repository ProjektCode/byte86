using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour {

    public WaveConfig[] waveConfigs;
    public float delayBeforeFirstWave = 2f;
    public float delayBetweenWaves = 3f;

    private int currentWaveIndex = 0;
    private int currentWave = 1;
    private EnemySpawner spawner;

    void Start() {
        spawner = FindFirstObjectByType<EnemySpawner>();
        StartCoroutine(SpawnWavesLoop());
    }

    IEnumerator SpawnWavesLoop() {
        yield return new WaitForSeconds(delayBeforeFirstWave);

        while (true) {
            // Spawn current wave
            WaveConfig wave = waveConfigs[currentWaveIndex];
            yield return StartCoroutine(spawner.SpawnWave(wave));

            GameManager.Instance.UpdateWave(currentWave);

            // Wait until all enemies are defeated
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            // Wait before next wave
            yield return new WaitForSeconds(delayBetweenWaves);

            // Cycle to next wave
            currentWaveIndex = (currentWaveIndex + 1) % waveConfigs.Length;
            currentWave++;
        }
    }

    public void ResetWaves() {
        StopAllCoroutines();
        currentWaveIndex = 0;
        currentWave = 1;
        StartCoroutine(SpawnWavesLoop());
    }


}
