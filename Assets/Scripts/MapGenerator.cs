using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Vector2 mapSize;

    [Range(0,1)]
    [SerializeField] private float outlinePercent;

    private void Start()
    {
        MapGenerate();
    }

    public void MapGenerate()
    {
        string holdname = "Generated Map";
        if (transform.FindChild(holdname))
        {
            DestroyImmediate(transform.FindChild(holdname).gameObject);
        }
        Transform mapHolder = new GameObject(holdname).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }
    }
}