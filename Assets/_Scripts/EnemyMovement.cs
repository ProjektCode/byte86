using UnityEngine;

[RequireComponent(typeof(EnemyShooting))]
public class EnemyMovement : EnemyStats {
    public float shootRange = 5f;

    private EnemyShooting shooter;
    private Transform player;
    private Camera mainCam;

    private bool isOnScreen = false;
    private bool reachedScreen = false;
    private bool canMove = true;

    void Start() {
        shooter = GetComponent<EnemyShooting>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCam = Camera.main;
    }

    void Update() {
        if(!canMove) return;

        if (player == null) return;

        CheckIfOnScreen();

        if (!reachedScreen) {
            // Step 2: Move down until visible
            if (!isOnScreen) {
                MoveDownward();
                return;
            } else {
                reachedScreen = true;
            }
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > shootRange) {
            // Step 4: Move until in range
            MoveDownward();
        } else {
            // Step 5: In range and on screen, shoot
            shooter.TryShoot();
        }
    }

    void MoveDownward() {
        transform.position += moveSpeed * Time.deltaTime * Vector3.down;
    }

    void CheckIfOnScreen() {
        Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);
        isOnScreen = viewPos.x >= 0f && viewPos.x <= 1f &&
                      viewPos.y >= 0f && viewPos.y <= 1f &&
                      viewPos.z > 0f;
    }

    protected override void Die() {

        shooter.enabled = false;
        canMove = false;
        base.Die();
    }
}
