using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Byte86/WaveConfig")]
public class WaveConfig : ScriptableObject {
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 1f;
}
