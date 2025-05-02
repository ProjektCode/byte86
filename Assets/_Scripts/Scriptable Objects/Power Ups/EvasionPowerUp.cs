using UnityEngine;

[CreateAssetMenu(fileName = "Evasion", menuName = "Byte86/Powerups/Evasion")]
public class EvasionPowerup : PowerupData {
    [SerializeField, Range(0f, 1f)] private float evadeChance = 0.25f;

    public override void Activate(GameObject player) {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.SetEvasionActive(true, evadeChance);
    }

    public override void Deactivate(GameObject player) {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.SetEvasionActive(false, 0f);
    }

    private void OnValidate() => evadeChance = Mathf.Round(evadeChance * 100f) / 100f;
}
