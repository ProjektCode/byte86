using UnityEngine;

[CreateAssetMenu(fileName = "Jug", menuName = "Byte86/Powerups/Jug")]
public class JuggernautPowerup : PowerupData {
    [SerializeField] private float healPercent = 0.05f;
    [SerializeField] private float maxHealthBoostPercent = 0.25f;

    private float addedMaxHealth;
    private bool isActive = false;

    public override void Activate(GameObject player) {
        if(isActive) return;
        PlayerStats stats = player.GetComponent<PlayerStats>();
        ApplyPowerup(stats);
    }

    public override void Deactivate(GameObject player) {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        ExpirePowerup(stats);
    }

    private void ApplyPowerup(PlayerStats player) {
        isActive = true;
        // Heal based on current max health
        float healAmount = player.MaxHealth * healPercent;
        player.Heal(healAmount);

        // Boost max health
        addedMaxHealth = player.MaxHealth * maxHealthBoostPercent;
        player.MaxHealth += addedMaxHealth;

        // Optionally heal the added amount
        player.Heal(addedMaxHealth);
    }

    private void ExpirePowerup(PlayerStats player) {
        if(!isActive) return;
        player.MaxHealth -= addedMaxHealth;

        float cHealth = player.GetCurrentHealth();

        if (cHealth > player.MaxHealth) player.SetCurrentHealth(Mathf.Clamp(player.GetCurrentHealth(), 0, player.MaxHealth));

        addedMaxHealth = 0;

        isActive = false;
    }
}
