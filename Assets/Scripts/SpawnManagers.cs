using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManagers : MonoBehaviour
{
    public GameObject player;
    public CanvasManager canvasManager;

    public GameObject enemyParent;
    public EnemyManager enemyManager;

    [Tooltip("Order of spawners corresponds to when they are active")]
    public BoxCollider[] enemySpawners;
    private int activeSpawners = 1;          //Based as index range for enemySpawner array

    public int spawnLimit = 20;
    public bool canSpawn = false;
    public float startingSpawnDelay = 3f;    //Starting amount of time between each enemy spawns
    public float minSpawnDelay = 1f;         //Minimum amount of time between each enemy spawns
    public float spawnDelayDecayTime = 0.1f; //How much time gets reduced per difficulty increase
    private float spawnDelay;

    private float timer = 0f;

    public void Setup(ref GameObject gamePlayer)
    {
        spawnDelay = startingSpawnDelay;

        player = gamePlayer;
        enemyManager.SetUp(ref player, ref canvasManager);
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn && enemyParent.transform.childCount <= spawnLimit)
        {
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        timer += Time.deltaTime;

        if(timer > spawnDelay)
        {
            timer = 0f;
            SpawnEnemy(enemyManager.GetEnemy());
        }
    }

    //Interval when increasing map size
    //Must ensure activeSpawners doesn't go past
    //Enemyspawner array count

    Vector3 RandomSpawnLocation()
    {
        //Select random spawner
        int spawner = Random.Range(0, activeSpawners+1);

        //Select random location within spawner
        Vector3 spawnRange = enemySpawners[spawner].size/2;

        float randomXPosition = UnityEngine.Random.Range(-spawnRange.x, spawnRange.x);
        float randomZPosition = UnityEngine.Random.Range(-spawnRange.z, spawnRange.z);

        Vector3 spawnLocation = enemySpawners[spawner].transform.position;
        spawnLocation.x += randomXPosition;
        spawnLocation.z += randomZPosition;

        return spawnLocation;
    }

    void SpawnEnemy(EnemySO enemySO)
    {
        //randomly pick spawner
        Vector3 spawn = RandomSpawnLocation();

        //call enemy manager
        GameObject newEnemy = Instantiate(enemySO.enemy, enemyParent.transform);
        newEnemy.transform.position = spawn;

        //open enemy type, and update values depending on difficulty
        enemyManager.UpdateEnemyStats(enemySO, ref newEnemy);
    }

    public void IncreaseDifficulty(int difficulty)
    {
        enemyManager.IncreaseDifficulty(difficulty);

        //Reduce spawn time
        if(spawnDelay > minSpawnDelay)
        {
            spawnDelay -= spawnDelayDecayTime;
        }

        switch (difficulty)
        {
            case 2:
                //Stage 2 spawners
                activeSpawners = 2;
                break;
            case 4:
                //Stage 3 spawners
                activeSpawners = 3;
                break;
            case 6:
                //Stage 4 spawners
                activeSpawners = 4;
                break;
        }
    }

    public void EnableSpawning()
    {
        canSpawn = true;
    }

    public void DisableSpawning()
    {
        canSpawn = false;
        timer = 0;
    }

    public void ResetSpawningManager()
    {
        DisableSpawning();

        //Rest spawn information
        spawnDelay = startingSpawnDelay;
        activeSpawners = 1;

        //Destroy remaining enemy objects
        foreach(Transform child in enemyParent.transform)
        {
            Destroy(child.gameObject);
        }

        enemyManager.ResetGame();
    }

    public void FreezeEnemies()
    {
        foreach(Transform child in enemyParent.transform)
        {
            BasicEnemyAI enemy = child.gameObject.GetComponent<BasicEnemyAI>();
            if(enemy != null)
            {
                enemy.SlowdownSpeed();
            }
        }
    }

    public void UnfreezeEnemies()
    {
        foreach (Transform child in enemyParent.transform)
        {
            BasicEnemyAI enemy = child.gameObject.GetComponent<BasicEnemyAI>();
            if (enemy != null)
            {
                enemy.SetDefaultSpeed();
            }
        }
    }
}
