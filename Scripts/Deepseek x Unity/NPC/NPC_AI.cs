using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class NPC_AI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    private Transform player;
    
    public enum SectType { 剑宗, 丹宗, 符宗 }
    
    public float attack = 10f;  // 基础攻击
    public float defense = 5f;  // 基础防御
    public float intelligence = 8f; // 灵智值
    
    // 根据门派初始化
    public void InitializeBySect(SectType type) {
        sectType = type;
        switch(type) {
            case SectType.剑宗:
                attack = 20f; 
                moveSpeed = 4f;
                break;
            case SectType.丹宗:
                intelligence = 15f;
                break;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Initialize(NPCBehavior behavior)
    {
        // 初始化AI行为
        moveSpeed = behavior.moveSpeed;
        detectionRange = behavior.detectionRange;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }
    
    [Header("修真属性")]
    public SectType sectType; 
    public Color sectColor = Color.red; // 新增颜色字段

    private void OnAttributeChanged() {
        GetComponent<SpriteRenderer>().color = 
            Color.Lerp(Color.white, sectColor, attack/100f);
    }
    
    public void ExecuteSectSkill() {
        switch(sectType) {
            case SectType.剑宗:
                Instantiate(swordWavePrefab, transform.position, Quaternion.identity);
                break;
            case SectType.丹宗:
                Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
                break;
            case NPC_AI.SectType.符宗:
                Instantiate(defenseRunePrefab, transform.position, Quaternion.identity);
                //ApplyDefenseBuff(); // 添加防御增益逻辑
                break;
        }
        
        // 动态加载预制件
        GameObject effectPrefab = Resources.Load<GameObject>($"Prefabs/{sectType}Skill");
        Instantiate(effectPrefab, transform.position, Quaternion.identity);
    
        // 动态创建粒子系统（可选）
        GameObject particles = new GameObject("SkillParticles");
        var mainModule = particles.AddComponent<ParticleSystem>().main;
        mainModule.startColor = sectColor;
    }
    
    // 修改 NPC_AI.cs
    [Header("技能预制体")]
    public GameObject swordWavePrefab; // 剑宗技能
    public GameObject healEffectPrefab; // 丹宗技能
    public GameObject defenseRunePrefab; // 符宗技能

    public void HealAllies(Vector3 position, float radius) {
        Collider2D[] allies = Physics2D.OverlapCircleAll(position, radius);
        foreach(var ally in allies) {
            if(ally.CompareTag("Ally")) {
                ally.GetComponent<Health>().Heal(5f);
            }
        }
    }
}