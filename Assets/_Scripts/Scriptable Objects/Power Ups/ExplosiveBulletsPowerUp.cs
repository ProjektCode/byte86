using UnityEngine;

[CreateAssetMenu(menuName = "Byte86/Powerups/ExplosiveBullets")]
public class ExplosiveBulletsPowerUp : PowerupData {
    public float explosionRadius = 1.5f;
    public LayerMask enemyLayer;

    public override void Activate(GameObject player) {
        PlayerShooting shooting = player.GetComponent<PlayerShooting>();
        if (shooting != null) {
            shooting.SetExplosiveBulletsActive(true, explosionRadius, enemyLayer);
        }
    }

    public override void Deactivate(GameObject player) {
        PlayerShooting shooting = player.GetComponent<PlayerShooting>();
        if (shooting != null) {
            shooting.SetExplosiveBulletsActive(false);
        }
    }
}
