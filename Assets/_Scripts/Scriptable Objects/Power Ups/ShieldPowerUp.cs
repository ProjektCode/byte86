using UnityEngine;

[CreateAssetMenu(fileName = "Shield", menuName = "Byte86/Powerups/Shield")]
public class ShieldPowerup : PowerupData
{
    [SerializeField] private int maxHits = 3;
    [SerializeField] private Color shieldTint = Color.cyan;

    public override void Activate(GameObject player)
    {
        ShieldHandler handler = player.GetComponent<ShieldHandler>();
        if (handler != null)
        {
            handler.EnableShield(duration, maxHits, shieldTint);
        }
    }

    public override void Deactivate(GameObject player)
    {
        ShieldHandler handler = player.GetComponent<ShieldHandler>();
        if (handler != null)
        {
            handler.ForceDisableShield();
        }
    }
}
