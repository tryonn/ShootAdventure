using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Wave[] waves;          // vertor com os atributos para spawn dos enemys
    [SerializeField] private Enemy enemy;           // guarda o prefab enemy

    Wave currentWave;                               
    int currentWaveNumber;                          

    int enemyRemainingToSpawn;                     // enemy restante para ser instanciado
    float nextSpawnTime;                           // tempo para o proximo spawn

    private void Start()
    {
        NextWave();
    }

    private void Update()
    {
        if (enemyRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemyRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
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
