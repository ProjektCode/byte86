using UnityEngine;

public class PowerupDropper : MonoBehaviour {
    [Range(0f, 1f)]
    public float dropChance = 0.25f;
    public GameObject[] powerupPrefabs; // Array of powerup prefabs

    public void TryDropPowerup() {
        if (powerupPrefabs.Length == 0) return;

        if (Random.value <= dropChance) {
            int index = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[index], transform.position, Quaternion.identity);
        }
    }

    private void OnValidate() {
        dropChance = Mathf.Round(dropChance * 100f) / 100f;
    }
}
