using System.Collections;
using UnityEngine;

public class PlayerPowerupHandler : MonoBehaviour {

    private PowerupData activePowerup;
    private Coroutine powerupRoutine;

    public void ApplyPowerup(PowerupData newPowerup) {

        if(activePowerup != null && activePowerup.name == newPowerup.name) return;

        // Remove the old one if active
        if (activePowerup != null) {
            activePowerup.Deactivate(gameObject);

            if (powerupRoutine != null)
                StopCoroutine(powerupRoutine);
        }

        // Set and activate new powerup
        activePowerup = newPowerup;
        activePowerup.Activate(gameObject);
        powerupRoutine = StartCoroutine(RemoveAfterDuration(activePowerup));
    }

    private IEnumerator RemoveAfterDuration(PowerupData powerup) {
        yield return new WaitForSeconds(powerup.duration);

        // Only deactivate if it hasn’t been replaced by another powerup
        if (powerup == activePowerup) {
            activePowerup.Deactivate(gameObject);
            activePowerup = null;
        }

    }

}
