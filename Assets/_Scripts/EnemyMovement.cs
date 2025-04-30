using UnityEngine;

[RequireComponent(typeof(EnemyShooting))]
public class EnemyMovement : MonoBehaviour {
    public float shootRange = 5f;
    private EnemyShooting shooter;
    private Transform player;
    private Camera mainCam;

    private bool isOnScreen = false;
    private bool reachedScreen = false;
    private bool canMove = true;
    private EnemyStats stats;

    void Awake() {
        stats = GetComponent<EnemyStats>();
    }

    void Start() {
        shooter = GetComponent<EnemyShooting>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCam = Camera.main;
    }

    void Update() {
        if (!canMove || player == null) return;

        CheckIfOnScreen();

        if (!reachedScreen) {
            if (!isOnScreen) {
                MoveDownward();
                return;
            } else {
                reachedScreen = true;
            }
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > shootRange) {
            MoveTowardsPlayer();
        } else {
            RotateTowardsPlayer();
            shooter.TryShoot();
        }
    }

    void MoveDownward() {
        transform.position += stats.moveSpeed * Time.deltaTime * Vector3.down;
    }

    void MoveTowardsPlayer() {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.deltaTime);
        RotateTowardsPlayer();
    }

    void RotateTowardsPlayer() {
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // assuming default enemy faces "up"
    }

    void CheckIfOnScreen() {
        Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);
        isOnScreen = viewPos.x >= 0f && viewPos.x <= 1f &&
                     viewPos.y >= 0f && viewPos.y <= 1f &&
                     viewPos.z > 0f;
    }

     void Die() {
        shooter.enabled = false;
        canMove = false;
        stats.Die();
    }
}
