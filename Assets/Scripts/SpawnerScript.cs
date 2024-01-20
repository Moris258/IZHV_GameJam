using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public float SpawnFreqMean = 10.0f;
    public float SpawnFreqDev = 2.0f;
    public int StartingSpawnCount = 5;
    private float spawnTimer = 0.0f;
    private float nextSpawn = 0.0f;
    public bool SpawnEnemies = true;
    public Vector2 SpawnArea = new Vector2(75f, 55f);
    public int MaxEnemies = 20;
    private string spawnLayer = "Enemy";

    public List<GameObject> EnemyPrefabs = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        SpawnerSetup();
    }

    // Update is called once per frame
    void Update()
    {
        if(SpawnEnemies && !GameManager.Instance.GamePaused && GetEnemyCount() < MaxEnemies){
            spawnTimer += Time.deltaTime;
            if(spawnTimer >= nextSpawn){
                spawnTimer -= nextSpawn;

                SpawnEnemy();

                nextSpawn = RandomNormal(SpawnFreqMean, SpawnFreqDev);
            }
        }
    }

    void SpawnEnemy(){
        if(EnemyPrefabs.Count <= 0) return;

        int enemy = (int)RandomInterval(0, EnemyPrefabs.Count);
        if(enemy >= EnemyPrefabs.Count) enemy = EnemyPrefabs.Count - 1;

        GameObject spawnedEnemy = Instantiate(EnemyPrefabs[enemy], transform);
        spawnedEnemy.transform.position = new Vector3(RandomInterval(-SpawnArea.x, SpawnArea.x), RandomInterval(-SpawnArea.y, SpawnArea.y), 0.0f);

        spawnedEnemy.layer = LayerMask.NameToLayer(spawnLayer);
    }

    void SpawnerSetup(){
        spawnTimer = 0f;
        nextSpawn = RandomNormal(SpawnFreqMean, SpawnFreqDev);
        for(int i = 0; i < StartingSpawnCount; i++)
            SpawnEnemy();
    }

    public static float RandomInterval(float min, float max){
        return UnityEngine.Random.value * (max - min) + min;
    }
    public static float RandomNormal(float mean, float std)
    {
        var v1 = 1.0f - UnityEngine.Random.value;
        var v2 = 1.0f - UnityEngine.Random.value;
        
        var standard = Math.Sqrt(-2.0f * Math.Log(v1)) * Math.Sin(2.0f * Math.PI * v2);
        
        return (float)(mean + std * standard);
    }

    public int GetEnemyCount(){
        return transform.childCount;
    }
}
