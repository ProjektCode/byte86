using UnityEngine;

[CreateAssetMenu(fileName = "Damage Boost", menuName = "Byte86/Powerups/Damage Boost")]
public class DamageBoostPowerUp : PowerupData {
    [Range(1f, 2f)][SerializeField] private float multiplier = 1.25f;

    private float orgDmg;

    public override void Activate(GameObject player) {

        ShootingPoint shoot = player.GetComponent<ShootingPoint>();
        if (shoot == null) return;

        GameObject bulletObj = shoot.bulletPrefab;
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet == null) return;

        orgDmg = bullet.damage;
        bullet.damage *= multiplier;
        Bullet.boostedActive = true;
        Debug.Log("Damage is now: " + bullet.damage);
    }

    public override void Deactivate(GameObject player) {

        ShootingPoint shoot = player.GetComponent<ShootingPoint>();
        if (shoot == null) return;

        GameObject bulletObj = shoot.bulletPrefab;
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet == null) return;

        bullet.damage = orgDmg;
        Bullet.boostedActive = false;
        Debug.Log("Bullet's org damage: " + orgDmg + " - Bullet's Changed damage is now: " + bullet.damage);

    }

    private void OnValidate() => multiplier = Mathf.Round(multiplier * 100f) / 100f;

}

