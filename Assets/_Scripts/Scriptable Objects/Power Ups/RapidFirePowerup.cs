using UnityEngine;

[CreateAssetMenu(fileName = "RapidFire", menuName = "Byte86/Powerups/RapidFire")]
public class RapidFirePowerup : PowerupData {

    [Range(1f, 2f)]
    public float FireRateModifier = 1.25f;

    private float orgFireRate;
    public override void Activate(GameObject player) {
        PlayerShooting shooter = player.GetComponent<PlayerShooting>();
        orgFireRate = shooter.FireRate;

        shooter.FireRate = Mathf.Max(orgFireRate / FireRateModifier, 0.05f);
        Debug.Log("New fire rate is: " + shooter.FireRate);
    }

    public override void Deactivate(GameObject player) {
        player.GetComponent<PlayerShooting>().FireRate = orgFireRate;
        Debug.Log("old fire rate is: " + player.GetComponent<PlayerShooting>().FireRate);
    }

    private void OnValidate() => FireRateModifier = Mathf.Round(FireRateModifier * 100f) / 100f;
}
