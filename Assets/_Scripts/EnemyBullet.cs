using UnityEngine;

public class EnemyBullet : Projectile {
    protected override string TargetTag => "Player";

}
