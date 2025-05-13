using UnityEngine;

[CreateAssetMenu(fileName = "Damage Boost", menuName = "Byte86/Powerups/Damage Boost")]
public class DamageBoostPowerUp : PowerupData {
    [Range(1f, 2f)][SerializeField] private float multiplier = 1.25f;
    private float originalDamage;
    public static bool BoostedActive { get; private set; }

    public override void Activate(GameObject player) {
        var shoot = player.GetComponent<PlayerShooting>();
        if (shoot == null) return;

        var bullet = shoot.bulletPrefab.GetComponent<PlayerBullet>();
        if (bullet == null) return;

        originalDamage = bullet.ReturnOrgDamage();
        Debug.Log("Original Damage: " + originalDamage);
        bullet.damage *= multiplier;
        Debug.Log("Boosted Damage: " + bullet.damage);
        BoostedActive = true;
    }

    public override void Deactivate(GameObject player) {
        var shoot = player.GetComponent<PlayerShooting>();
        if (shoot == null) return;

        var bullet = shoot.bulletPrefab.GetComponent<PlayerBullet>();
        if (bullet == null) return;

        bullet.damage = originalDamage;
        Debug.Log("Damage Reset to: " + originalDamage);
        BoostedActive = false;
    }

    private void OnValidate() => multiplier = Mathf.Round(multiplier * 100f) / 100f;
}


