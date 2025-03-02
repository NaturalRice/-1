using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    public int HP;//最大生命值
    public float CurrentHP;//Luna的当前生命值
    public int MP;//最大蓝量
    public float CurrentMP;//luna的当前蓝量
    
    [Header("敌人设置")]
    public Sprite enemySprite; // 直接引用敌人贴图
    public Vector2 colliderSize = new Vector2(0.8f, 1.2f); // 碰撞体大小
    public int maxEnemies = 5; // 最大敌人数
    public float spawnInterval = 3f; // 生成间隔
    
    public AnimationCurve difficultyCurve;

    private void Awake()
    {
        Instance = this;
        CurrentHP = 100;
        CurrentMP = 100;
        HP =100;
        MP =100;
    }
    
    private void Update()
    {
        if (CurrentMP <= 100)
        {
            AddOrDecreaseMP(Time.deltaTime);
        }
        if (CurrentHP <= 100)
        {
            AddOrDecreaseHP(Time.deltaTime);
        }
    }
    
    private void Start()
    {
        // 开始生成敌人
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }
    
    public void AddOrDecreaseHP(float value)
    {
        CurrentHP += value;
        if (CurrentHP>=HP)
        {
            CurrentHP = HP;
        }
        if (CurrentHP<=0)
        {
            CurrentHP = 0;
        }
        UIManager.Instance.SetHPValue(CurrentHP/HP);
    }
    
    public void AddOrDecreaseMP(float value)
    {
        CurrentMP += value;
        if (CurrentMP >= MP)
        {
            CurrentMP = MP;
        }
        if (CurrentMP <= 0)
        {
            CurrentMP = 0;
        }
        UIManager.Instance.SetMPValue(CurrentMP / MP);
    }

    private void SpawnEnemy()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemies) return;

        // 动态创建敌人对象
        GameObject enemy = new GameObject("Enemy");
        enemy.tag = "Enemy";

        // 添加 SpriteRenderer
        SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
        renderer.sprite = enemySprite;
        renderer.sortingLayerName = "NPC"; // 确保和NPC同一层级
        renderer.sortingOrder = 1;

        // 添加 Rigidbody2D
        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // 添加 Collider
        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        collider.size = colliderSize;

        // 添加 EnemyController 并配置参数
        EnemyController controller = enemy.AddComponent<EnemyController>();
        controller.moveSpeed = 3f;
        controller.spawnDistance = 10f;

        // 设置初始位置
        enemy.transform.position = GetRandomSpawnPosition(controller.spawnDistance);
    }
    
    private Vector2 GetRandomSpawnPosition(float distance)
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 spawnPos;
        do
        {
            spawnPos = new Vector2(
                Random.Range(-distance, distance),
                Random.Range(-distance, distance)
            ) + (Vector2)player.position;
        } while (Vector2.Distance(spawnPos, player.position) < distance);
        
        return spawnPos;
    }
}