using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Byte86/WaveConfig")]
public class WaveConfig : ScriptableObject {
    public GameObject[] enemyPrefabs;
    public int enemiesToSpawn = 5;
    public float spawnInterval = 1f;
}
