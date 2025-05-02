using UnityEngine;

[CreateAssetMenu(fileName = "RapidFire", menuName = "Byte86/Powerups/RapidFire")]
public class RapidFirePowerup : PowerupData {

    [Range(0.1f, 1f)]
    public float FireRateModifier = 0.25f;

    private float orgFireRate;
    public override void Activate(GameObject player) {
        orgFireRate = player.GetComponent<ShootingPoint>().FireRate;
        player.GetComponent<ShootingPoint>().FireRate *= FireRateModifier;
        Debug.Log("New fire rate is: " + player.GetComponent<ShootingPoint>().FireRate);
    }

    public override void Deactivate(GameObject player) {
        player.GetComponent<ShootingPoint>().FireRate = orgFireRate;
        Debug.Log("old fire rate is: " + player.GetComponent<ShootingPoint>().FireRate);
    }

    private void OnValidate() => FireRateModifier = Mathf.Round(FireRateModifier * 100f) / 100f;
}
