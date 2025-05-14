using UnityEngine;

[CreateAssetMenu(fileName = "Vampiric Rounds", menuName = "Byte86/Powerups/VampiricRounds")]
public class VampiricRoundsPowerUp : PowerupData {

    public static bool Active { get; private set; }
    public override void Activate(GameObject player) {
        if (Active) return; 
        Active = true;
    }

    public override void Deactivate(GameObject player) {
        if(!Active) return;
        Active = false;
    }
}
