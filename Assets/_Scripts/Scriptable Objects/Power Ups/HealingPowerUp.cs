using UnityEngine;

[CreateAssetMenu(fileName = "Healing", menuName = "Byte86/Powerups/Healing")]
public class HealingPowerup : PowerupData {
    [SerializeField] private float HealAmountPerSecond = 5f; // Healing amount per second
    [SerializeField] private float TickDelay = 1f;

    public override void Activate(GameObject player) {
        PlayerStats stats = player.GetComponent<PlayerStats>();

        // Prevent other powerups from being active
        if (stats.isHealing) return; // Skip if healing is already active

        stats.StartHealing(HealAmountPerSecond, duration, TickDelay); // Start healing
    }

    public override void Deactivate(GameObject player) {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.StopHealing(); // Stop healing when deactivated
    }
}
