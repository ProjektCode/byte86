using UnityEngine;

[CreateAssetMenu(fileName = "Evasion", menuName = "Byte86/Powerups/Evasion")]
public class EvasionPowerup : PowerupData {
    [SerializeField, Range(0f, 1f)] private float evadeChance = 0.25f;
    [SerializeField] private Color EvasionTint = Color.purple;

    public override void Activate(GameObject player) {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        Debug.Log("Evasion Activated");
        stats.SetEvasionActive(true, evadeChance, EvasionTint);
    }

    public override void Deactivate(GameObject player) {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        Debug.Log("Evasion Deactivated");
        stats.SetEvasionActive(false, 0f, EvasionTint);
    }

    private void OnValidate() => evadeChance = Mathf.Round(evadeChance * 100f) / 100f;
}
