using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerPowerupHandler : MonoBehaviour {
    private List<PowerupInstance> offensivePowerups = new();
    private List<PowerupInstance> supportPowerups = new();

    private const int MaxPerCategory = 2;

    public void ApplyPowerup(PowerupData newPowerup) {
        var list = newPowerup.PowerupType == PowerupData.PowerupCategory.Offensive ? offensivePowerups : supportPowerups;

        // If it's already active, refresh it
        var existing = list.Find(p => p.data == newPowerup);
        if (existing != null) {
            StopCoroutine(existing.coroutine);

            float refreshDuration = newPowerup.GetDuration();
            if (newPowerup.PowerupType == PowerupData.PowerupCategory.Offensive && PowerCoreSyncPowerUp.Active) refreshDuration *= PowerCoreSyncPowerUp.GetMultiplier;

            existing.coroutine = StartCoroutine(RemoveAfterDuration(newPowerup, list, list.IndexOf(existing), refreshDuration));
            return;
        }

        // Remove oldest if at max
        if (list.Count >= MaxPerCategory) {
            var oldest = list[0];
            oldest.data.Deactivate(gameObject);
            StopCoroutine(oldest.coroutine);
            list.RemoveAt(0);

            // Shift UI: Clear old slot 0
            GameManager.Instance.ClearPowerupUI(oldest.data.PowerupType, 0);
        }

        // NEW: Adjust duration if PowerCoreSync is active
        float finalDuration = newPowerup.GetDuration();
        if (newPowerup.PowerupType == PowerupData.PowerupCategory.Offensive && PowerCoreSyncPowerUp.Active) finalDuration *= PowerCoreSyncPowerUp.GetMultiplier;

        // Add new powerup
        newPowerup.Activate(gameObject);
        var newInstance = new PowerupInstance {
            data = newPowerup,
            coroutine = StartCoroutine(RemoveAfterDuration(newPowerup, list, list.Count, finalDuration)) // New index is current count
        };
        list.Add(newInstance);

        GameManager.Instance.UpdatePowerupUI(newPowerup, list.Count - 1); // 0 or 1
        Debug.Log("[Powerup Handler] Duration of: " + newPowerup.PowerUpName + " - is: " + finalDuration);
    }

    private IEnumerator RemoveAfterDuration(PowerupData powerup, List<PowerupInstance> list, int slotIndex, float duration) {
        yield return new WaitForSeconds(duration);
        var instance = list.Find(p => p.data == powerup);
        if (instance != null) {
            powerup.Deactivate(gameObject);
            list.Remove(instance);
            GameManager.Instance.ClearPowerupUI(powerup.PowerupType, slotIndex);
        }
    }

    private class PowerupInstance {
        public PowerupData data;
        public Coroutine coroutine;
    }
}

