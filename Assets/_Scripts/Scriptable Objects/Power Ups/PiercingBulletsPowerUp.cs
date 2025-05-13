using UnityEngine;

[CreateAssetMenu(fileName = "PiercingBullets", menuName = "Byte86/Powerups/PiercingBullets")]
public class PiercingBulletsPowerUp : PowerupData {
    private bool isActive = false; // To track if the power-up is active

    public override void Activate(GameObject player) {
        if (isActive) return; // Prevent reactivation if already active
        isActive = true;

        // You could also signal the shooting script to start using piercing bullets here
        player.GetComponent<PlayerShooting>().EnablePiercing(true); // Assuming you have a PlayerShooting script
    }

    public override void Deactivate(GameObject player) {
        if (!isActive) return; // Prevent deactivation if it's not active
        isActive = false;

        // You could also signal the shooting script to stop using piercing bullets
        player.GetComponent<PlayerShooting>().EnablePiercing(false); // Disable piercing bullets
    }
}
