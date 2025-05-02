using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour {
    public Transform[] shootingPoints;
    public GameObject projectilePrefab;
    public Vector3 finalPosition;
    public float patternDelay = 1.5f;
    public float shotDelay = 0.2f;

    public float sideToSideAmplitude = 3f; // How far side to side
    public float sideToSideFrequency = 1f; // How fast side to side
    public float sideToSideStartDelay = 1f; // Delay before starting side to side

    public float minX = -7f; // Left boundary
    public float maxX = 7f;  // Right boundary

    [Header("Sound Settings")] 
    public AudioClip shootSFX;
    [Range(0.9f, 1.1f)] public float minPitch = 0.95f;
    [Range(0.9f, 1.1f)] public float maxPitch = 1.05f;
    

    private EnemyStats stats;
    private Vector3 centerPosition;
    private bool isInPosition = false;
    private bool canMoveSideToSide = false;
    private float moveStartTime;
    private AudioSource audioSource;

    void Awake() {
        stats = GetComponent<EnemyStats>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        StartCoroutine(MoveIntoPosition());
    }

    private IEnumerator MoveIntoPosition() {
        while (Vector3.Distance(transform.position, finalPosition) > 0.1f) {
            transform.position = Vector3.MoveTowards(transform.position, finalPosition, stats.moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = finalPosition; // Snap exactly
        centerPosition = finalPosition;
        moveStartTime = Time.time;
        isInPosition = true;

        StartCoroutine(StartSideToSideAfterDelay());
        StartCoroutine(AttackLoop());
    }

    private IEnumerator StartSideToSideAfterDelay() {
        yield return new WaitForSeconds(sideToSideStartDelay);
        moveStartTime = Time.time; // reset timer so sine starts clean
        canMoveSideToSide = true;
    }

    private void Update() {
        if (isInPosition && canMoveSideToSide && stats.canMove) {
            MoveSideToSide();
        }
    }

    private void MoveSideToSide() {
        float elapsedTime = Time.time - moveStartTime;
        float xOffset = Mathf.Sin(elapsedTime * sideToSideFrequency) * sideToSideAmplitude;
        float newX = centerPosition.x + xOffset;

        // Clamp position so boss doesn't go too far offscreen
        newX = Mathf.Clamp(newX, minX, maxX);

        transform.position = new Vector3(newX, centerPosition.y, centerPosition.z);
    }

    private IEnumerator AttackLoop() {
        while (true) {
            yield return StartCoroutine(AttackPattern1());
            yield return new WaitForSeconds(patternDelay);

            yield return StartCoroutine(AttackPattern2());
            yield return new WaitForSeconds(patternDelay);
        }
    }

    private IEnumerator AttackPattern1() {
        for (int i = 0; i < shootingPoints.Length; i++) {
            Shoot(shootingPoints[i]);
            yield return new WaitForSeconds(shotDelay);
        }
    }

    private IEnumerator AttackPattern2() {
        if (shootingPoints.Length < 4) {
            Debug.LogWarning("Not enough shooting points for Pattern 2!");
            yield break;
        }

        Shoot(shootingPoints[0]);
        Shoot(shootingPoints[shootingPoints.Length - 1]);
        yield return new WaitForSeconds(shotDelay);

        Shoot(shootingPoints[1]);
        Shoot(shootingPoints[2]);
    }

    private void Shoot(Transform shootPoint) {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(shootSFX);
        Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
    }
}
