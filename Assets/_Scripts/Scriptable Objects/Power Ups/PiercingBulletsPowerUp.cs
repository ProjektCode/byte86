using UnityEngine;

[CreateAssetMenu(fileName = "Piercing Bullets", menuName = "Byte86/Powerups/PiercingBullets")]
public class PiercingBulletsPowerUp : PowerupData {
    public static bool Active { get; private set; }

    public override void Activate(GameObject player) {
        if (Active) return; // Prevent reactivation if already active
        Active = true;
    }

    public override void Deactivate(GameObject player) {
        if (!Active) return; // Prevent deactivation if it's not active
        Active = false;
    }
}
