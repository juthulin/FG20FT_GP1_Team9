using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-40)]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private int maxAliveAtOnce = 15;
    [SerializeField] private int timeBetweenSpawns = 1;
    [Space]
    [SerializeField] private Transform[] spawnPoints;

    public Vector3 ConvergePoint { get; private set; } = Vector3.zero;
    public List<Transform> SpawnedEnemies { get; } = new List<Transform>();
    public bool PlayerHasCargo { get; set; } = default;

    public static EnemySpawner instance;
    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
        //_player = GameManager.Player.transform;
        //playerTransform = GameObject.Find(playerPosGOName).transform;
    }

    private void Start()
    {
        StartCoroutine(SpawnUpdate());
        StartCoroutine(ConvergePointUpdate());
    }

    private Vector3 SetSpawnPoint()
    {
        Vector3 playerPosition = playerTransform.position;
        
        float rndX = Random.Range(-1f, 1f);
        float rndY = Random.Range(-1f, 1f);
        float rndZ = Random.Range(-1f, 1f);

        Vector3 dirToSpawnPoint = (new Vector3(rndX, rndY, rndZ)).normalized;

        Vector3 closestSpawn = Vector3.positiveInfinity;
        float spwnDst = default;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if ((spawnPoints[i].position - playerPosition).magnitude < closestSpawn.magnitude)
            {
                closestSpawn = spawnPoints[i].position;
                spwnDst = spawnPoints[i].localScale.magnitude;
            }
        }
        
        float rndSpawnDistance = Random.Range(0f, spwnDst);
        
        return closestSpawn + dirToSpawnPoint * rndSpawnDistance;
    }

    private void SpawnEnemy()
    {
        GameObject enemy = EnemyPool.Instance.GetPooledObject(EnemyType.BasicEnemy);
        
        enemy.transform.position = SetSpawnPoint();
        enemy.transform.rotation = Quaternion.identity;
        enemy.GetComponent<EnemyVariant01>().Initialize(playerTransform);
        enemy.SetActive(true);
        
        SpawnedEnemies.Add(enemy.transform);
    }

    private IEnumerator SpawnUpdate()
    {
        while (true)
        {
            if (SpawnedEnemies.Count < maxAliveAtOnce)
            {
                SpawnEnemy();
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
    
    private IEnumerator ConvergePointUpdate()
    {
        while (true)
        {
            yield return new WaitWhile(() => PlayerHasCargo == true);

            Vector3 sum = Vector3.zero;
            float t = SpawnedEnemies.Count;
            for (int i = 0; i < t; i++)
            {
                sum += SpawnedEnemies[i].position;
            }

            Vector3 averagePt = sum / t;
            ConvergePoint = Vector3.Lerp(averagePt, playerTransform.position, 0.25f);

            yield return new WaitForSeconds(1);
        }
        
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(ConvergePoint, timeBetweenSpawns);

        if (spawnPoints.Length == 0) return;
        
        Gizmos.color = Color.blue;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Gizmos.DrawWireSphere(spawnPoints[i].position, spawnPoints[i].localScale.magnitude);
        }
    }

    private void OnValidate()
    {
        if(spawnPoints.Length == 0)
            Debug.LogError("EnemySpawner Warning: There are no spawn points in the scene!");
    }
}
