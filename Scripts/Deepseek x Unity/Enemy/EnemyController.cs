using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float spawnDistance = 10f;
    public float damageInterval = 1f; // 伤害间隔时间

    private Transform player;
    private bool isTouchingPlayer = false; // 是否正在接触玩家
    private float touchTimer = 0f; // 接触计时器

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // 追踪玩家
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }

        // 持续接触时计时
        if (isTouchingPlayer)
        {
            touchTimer += Time.deltaTime;
            if (touchTimer >= damageInterval)
            {
                JudgePlayerHP(-20); // 每秒扣血
                touchTimer = 0f; // 重置计时器
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true; // 标记开始接触
            touchTimer = 0f; // 重置计时器
            JudgePlayerHP(-20); // 立即扣血一次
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true; // 确保持续标记接触
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false; // 接触结束
            touchTimer = 0f; // 重置计时器
        }
    }

    private void JudgePlayerHP(int value)
    {
        EnemySpawner.Instance.AddOrDecreaseHP(value);
        
        if (EnemySpawner.Instance.CurrentHP <= 0)
        {
            Debug.Log("玩家被敌人捕获，游戏结束！");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}