using UnityEngine;

[RequireComponent(typeof(EnemyShooting))]
public class EnemyMovement : MonoBehaviour {
    public float shootRange = 5f;
    [SerializeField] private float yThreshold = 3f;

    private EnemyShooting shooter;
    private Transform player;
    private Camera mainCam;

    private bool isOnScreen = false;
    private bool reachedScreen = false;
    private EnemyStats stats;
    private float rotSpeed = 100;

    void Awake() {
        stats = GetComponent<EnemyStats>();
    }

    void Start() {
        shooter = GetComponent<EnemyShooting>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCam = Camera.main;
    }

    void Update() {
        if (!stats.canMove || player == null) return;

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
        if (!stats.canMove) return;
        // Vector2 direction = player.position - transform.position;
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // assuming default enemy faces "up"
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f); // Calculate target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
    }

    void CheckIfOnScreen() {
        // Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);
        // isOnScreen = viewPos.x >= 0f && viewPos.x <= 1f &&
        //              viewPos.y >= 0f && viewPos.y <= 1f &&
        //              viewPos.z > 0f;
        float camTopEdge = mainCam.transform.position.y + mainCam.orthographicSize;
        isOnScreen = transform.position.y <= camTopEdge - yThreshold;
    }

     void Die() {
        shooter.enabled = false;
        stats.Die();
    }
}
