using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Vector2 mapSize;

    [Range(0,1)]
    [SerializeField] private float outlinePercent;

    // ######## atributos para os obstaculos

    [SerializeField] private List<Coord> allTileCoords;// lista que recebe os obstaculos
    [SerializeField] private Queue<Coord> shuffledTileCoords; //
    [SerializeField] private int seed = 10;
    [SerializeField] private Transform obstaclePrefab;
    [SerializeField] private int obstacleCount = 10;


    // ######## atributos para os obstaculos

    private void Start()
    {
        MapGenerate();
    }

    public void MapGenerate()
    {
        // ####### begin - implementação dos obstaculos

        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utilitys.ShuffleArray(allTileCoords.ToArray(), seed));


        // ####### and - implementação dos obstaculos

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
                Vector3 tilePosition = CoordToPosition(x,y); // coordenadas para os obstaculos
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }

        // dados para os obstaculos
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
            Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
            newObstacle.parent = mapHolder;
        }
    }

    private Vector3 CoordToPosition(int x, int y) {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    // pegando random para obstaculos
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }
    // class para os obstaculos
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y) {
            x = _x;
            y = _y;
        }
    }
}