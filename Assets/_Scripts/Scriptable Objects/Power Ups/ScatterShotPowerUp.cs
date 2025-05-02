using UnityEngine;

[CreateAssetMenu(fileName = "Scattershot", menuName = "Byte86/Powerups/Scattershot")]
public class ScatterShotPowerUp : PowerupData {

    [SerializeField] private int additionalProjectiles = 2; // +2 means total 3 shots
    [SerializeField] private float spreadAngle = 15f;

    public override void Activate(GameObject player) {
        ShootingPoint shooter = player.GetComponent<ShootingPoint>();
        if (shooter != null) {
            shooter.scattershotActive = true;
            shooter.scatterCount = 1 + additionalProjectiles;
            shooter.scatterAngle = spreadAngle;
        }
    }

    public override void Deactivate(GameObject player) {
        ShootingPoint shooter = player.GetComponent<ShootingPoint>();
        if (shooter != null) {
            shooter.scattershotActive = false;
            shooter.scatterCount = 1;
        }
    }
}

