using UnityEngine;

public class Background : MonoBehaviour
{
 [Header("Scroll Settings")]
    public float scrollSpeed = 2f;
    [Range(0f, 1f)] public float parallaxStrength = 0.5f;

    [Header("Tiling")]
    public GameObject[] tilePrefabs; // Array of prefab variations
    public int tilesY = 3;

    private float tileHeight;
    private Transform[] tiles;

    void Start() {
        if (tilePrefabs.Length == 0) {
            Debug.LogError("No tile prefabs assigned!");
            return;
        }

        // Get height from first prefab
        SpriteRenderer sr = tilePrefabs[0].GetComponent<SpriteRenderer>();
        tileHeight = sr.bounds.size.y;

        tiles = new Transform[tilesY];
        for (int i = 0; i < tilesY; i++) {
            GameObject tile = InstantiateRandomTile();
            tile.transform.position = transform.position + Vector3.down * tileHeight * i;
            tiles[i] = tile.transform;
        }
    }

    void Update() {
        float scrollDelta = scrollSpeed * parallaxStrength * Time.deltaTime;

        for (int i = 0; i < tiles.Length; i++) {
            tiles[i].position += Vector3.down * scrollDelta;

            if (tiles[i].position.y < Camera.main.transform.position.y - tileHeight) {
                float highestY = GetHighestTileY();
                ReplaceTile(i, highestY + tileHeight);
            }
        }
    }

    void ReplaceTile(int index, float newY) {
        Destroy(tiles[index].gameObject);

        GameObject newTile = InstantiateRandomTile();
        newTile.transform.position = new Vector3(transform.position.x, newY, 0);
        tiles[index] = newTile.transform;
    }

    GameObject InstantiateRandomTile() {
        int randomIndex = Random.Range(0, tilePrefabs.Length);
        GameObject tile = Instantiate(tilePrefabs[randomIndex], transform);

        // Random horizontal flip
        Vector3 scale = tile.transform.localScale;
        scale.x = Random.value > 0.5f ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        tile.transform.localScale = scale;

        return tile;
    }

    float GetHighestTileY() {
        float highest = float.MinValue;
        foreach (Transform t in tiles)
            if (t.position.y > highest)
                highest = t.position.y;
        return highest;
    }
}
