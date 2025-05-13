using UnityEngine;

[CreateAssetMenu(fileName = "Vampiric Rounds", menuName = "Byte86/Powerups/VampiricRounds")]
public class VampiricRoundsPowerUp : PowerupData {

    public static bool VampiricRoundsActive { get; private set; }
    public override void Activate(GameObject player) {
        if (VampiricRoundsActive) return;
        
        VampiricRoundsActive = true;
         PlayerShooting p = player.GetComponent<PlayerShooting>();
         p.EnableVampiricRounds(VampiricRoundsActive);
        Debug.Log("Vampiric Round Activated");
    }

    public override void Deactivate(GameObject player) {
        VampiricRoundsActive = false;
        PlayerShooting p = player.GetComponent<PlayerShooting>();
         p.EnableVampiricRounds(VampiricRoundsActive);
        Debug.Log("Vampiric Round Deactived");
    }
}
