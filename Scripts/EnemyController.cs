using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f; // 敌人移动速度
    public float spawnDistance = 10f; // 生成时与玩家的最小距离

    private Transform player; // 玩家对象

    private void Start()
    {
        // 初始化玩家对象
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 随机生成位置，确保在镜头外
        Vector2 spawnPosition = GetRandomSpawnPosition();
        transform.position = spawnPosition;
    }

    private void Update()
    {
        // 追踪玩家
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检测是否与玩家碰撞
        /*if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家被敌人捕获，游戏结束！");
            Application.Quit(); // 退出游戏
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 在编辑器模式下停止运行
#endif
        }*/
    }

    private Vector2 GetRandomSpawnPosition()
    {
        // 获取玩家当前位置
        Vector2 playerPosition = player.position;

        // 随机生成位置，确保在镜头外
        Vector2 spawnPosition;
        do
        {
            spawnPosition = new Vector2(
                Random.Range(-spawnDistance, spawnDistance),
                Random.Range(-spawnDistance, spawnDistance)
            ) + playerPosition;
        } while (Vector2.Distance(spawnPosition, playerPosition) < spawnDistance);

        return spawnPosition;
    }
}