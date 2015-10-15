using UnityEngine;
using System.Collections;
using System;

public class Spawner : MonoBehaviour
{
    LivingEntity playerEntity;
    Transform playerT;

    [SerializeField] private Wave[] waves;          // vertor com os atributos para spawn dos enemys
    [SerializeField] private Enemy enemy;           // guarda o prefab enemy

    Wave currentWave;                               
    int currentWaveNumber;                          

    int enemyRemainingToSpawn;                     // enemy restante para ser instanciado
    int enemisRemainingAlive;
    float nextSpawnTime;                           // tempo para o proximo spawn

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;
    bool isDisabled;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnDeathPlayer;
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if (enemyRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemyRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine(SpawnEnemy());
            }
        }

    }


    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColour = tileMat.color;
        Color flashColour = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    public void OnDeathPlayer()
    {
        isDisabled = true;
    }

    private void OnEnemyDeath()
    {
        enemisRemainingAlive --;
        if (enemisRemainingAlive == 0)
        {
            NextWave();
        }
    }

    private void NextWave()
    {
        currentWaveNumber++;
        currentWave = waves[currentWaveNumber - 1];

        enemyRemainingToSpawn = currentWave.enemyCount;
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
