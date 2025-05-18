using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] GameObject floorPrefab;
    [SerializeField] int maxAnimatingTiles = 10;
    public delegate void TileReachedBottomHandler(Vector3 position);
    public event TileReachedBottomHandler OnTileReachedBottom;
    public delegate void TileFinishedMovingUpHandler(GameObject tile);
    public event TileFinishedMovingUpHandler OnTileFinishedMovingUp;

    int width = 20;
    int depth = 20;
    float moveDistance = 5f;
    float moveDuration = 2f;
    int heightOffset = 5;
    public AnimationCurve moveCurve;

    List<GameObject> floorTiles = new List<GameObject>();
    HashSet<GameObject> animatingTiles = new HashSet<GameObject>();

    void Start()
    {
        GenerateFloor();
    }

    private void GenerateFloor()
    {
        if (floorPrefab != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < depth; y++)
                {
                    Vector3 position = new Vector3(x, -heightOffset, y);
                    GameObject tile = Instantiate(floorPrefab, position, Quaternion.identity, transform);
                    floorTiles.Add(tile);
                }
            }
        }
    }

    public void SelectRandomFloor()
    {
        // Only try to animate if we haven't hit max animating tiles
        if (animatingTiles.Count >= maxAnimatingTiles) return;

        // Get tiles not currently animating
        List<GameObject> availableTiles = floorTiles.FindAll(tile => !animatingTiles.Contains(tile));

        if (availableTiles.Count == 0) return;

        // Pick a random tile from available ones
        GameObject randomTile = availableTiles[Random.Range(0, availableTiles.Count)];

        StartCoroutine(MoveTileDown(randomTile));
    }

    IEnumerator MoveTileDown(GameObject cube)
    {
        animatingTiles.Add(cube);

        Vector3 startPos = cube.transform.position;
        Vector3 endPos = startPos + Vector3.down * moveDistance;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            float curveT = moveCurve.Evaluate(t);
            cube.transform.position = Vector3.Lerp(startPos, endPos, curveT);

            elapsed += Time.deltaTime;
            yield return null;
        }
        cube.transform.position = endPos;

        OnTileReachedBottom?.Invoke(cube.transform.position);
        StartCoroutine(MoveTileUp(cube));
    }

    IEnumerator MoveTileUp(GameObject cube)
    {
        yield return new WaitForSeconds(5f);

        Vector3 startPos = cube.transform.position;
        Vector3 endPos = startPos + Vector3.up * moveDistance;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            float curveT = moveCurve.Evaluate(t);
            cube.transform.position = Vector3.Lerp(startPos, endPos, curveT);

            elapsed += Time.deltaTime;
            yield return null;
        }
        cube.transform.position = endPos;

        OnTileFinishedMovingUp?.Invoke(cube);
        yield return new WaitForSeconds(3f);

        animatingTiles.Remove(cube);
    }

    public GameObject GetTileAtPosition(Vector3 pos)
    {
        return floorTiles.Find(tile => Vector3.Distance(tile.transform.position, pos) < 0.1f);
    }
}
