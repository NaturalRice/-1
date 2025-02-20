using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 敌人预制体
    public int maxEnemies = 5; // 最大敌人数
    public float spawnInterval = 3f; // 生成间隔

    private void Start()
    {
        // 开始生成敌人
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        // 如果当前敌人数未达到最大值，则生成敌人
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies)
        {
            Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}