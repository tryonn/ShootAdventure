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
    [Range(0,1)]
    [SerializeField] private float obstaclePercent; //vai ser utilizado na multiplicacao | mapSie.x * mapSize.y * obstaclePercent
    [SerializeField] private List<Coord> allTileCoords;// lista que recebe os obstaculos
    [SerializeField] private Queue<Coord> shuffledTileCoords; //
    [SerializeField] private int seed = 10;
    [SerializeField] private Transform obstaclePrefab;
    [SerializeField] private Coord mapCenter;


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
        // dados para os obstaculos não ficarem no centro | criar caminhos
        mapCenter = new Coord((int)(mapSize.x / 2), (int)(mapSize.y / 2));


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
        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount ++;
            if (randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
            }
            else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount --;
            }
        }
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }

          }
            int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
            return targetAccessibleTileCount == accessibleTileCount;
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

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }
}