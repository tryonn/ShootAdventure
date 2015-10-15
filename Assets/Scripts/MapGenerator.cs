using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Map[] maps;
    [SerializeField] private int mapIndex;

    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform navMeshFloor;
    
    [SerializeField] private Vector2 maxMapSize;
    [SerializeField] private Transform navMaeshMaskPrefab;


    [SerializeField] private float tileSize;

    // ######## atributos para os obstaculos
    [Range(0,1)]
    [SerializeField] private float obstaclePercent; //vai ser utilizado na multiplicacao | mapSie.x * mapSize.y * obstaclePercent
    [Range(0,1)]
    [SerializeField] private float outlinePercent;
    private List<Coord> allTileCoords;// lista que recebe os obstaculos
    [SerializeField] private Queue<Coord> shuffledTileCoords; //
    [SerializeField]
    private Queue<Coord> shuffledOpenTileCoords; //

    [SerializeField] private Transform obstaclePrefab;
    private Map currentMap;
    private Transform[,] tileMap; // spawner rondomico


    // ######## atributos para os obstaculos

    private void Start()
    {
        MapGenerate();
    }

    public void MapGenerate()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];

        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, .5f, currentMap.mapSize.y * tileSize);

        // ####### begin - implementação dos obstaculos
        //generating coords
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utilitys.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
        // dados para os obstaculos não ficarem no centro | criar caminhos
        //mapCenter = new Coord((int)(currentMap.mapSize.x / 2f), (int)(currentMap.mapSize.y / 2f));


        // ####### and - implementação dos obstaculos
        //create map holder object
        string holdname = "Generated Map";
        if (transform.FindChild(holdname))
        {
            DestroyImmediate(transform.FindChild(holdname).gameObject);
        }
        Transform mapHolder = new GameObject(holdname).transform;
        mapHolder.parent = transform;

        // spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x,y); // coordenadas para os obstaculos
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }

        // dados para os obstaculos
        // spawning obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);


        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount ++;
            if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight,(float) prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1- outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);


                // Cores dos obstaculos
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);

            }
            else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount --;
            }
        }

        shuffledOpenTileCoords = new Queue<Coord>(Utilitys.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));



        // dados navmeshMaskPrefab
        // create navmeshmask
        Transform maskLeft = Instantiate(navMaeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navMaeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navMaeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y)/2f) * tileSize;

        Transform maskBotton = Instantiate(navMaeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBotton.parent = mapHolder;
        maskBotton.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        // dados do navmesh
        navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;


    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

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
            int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
            return targetAccessibleTileCount == accessibleTileCount;
        }


    private Vector3 CoordToPosition(int x, int y) {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }


    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x,0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);
        return tileMap[x, y];
    }

    // pegando random para obstaculos
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }



    // class para os obstaculos

    [System.Serializable]
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
    [System.Serializable]
    public class Map {
        public Coord mapSize;
        [Range(0,1)]
        public float outlinePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColour;
        public Color backgroundColour;


        public Coord mapCenter {
            get {
                return new Coord(mapSize.x / 2, mapSize.y /2);
            }
        }
    }
}