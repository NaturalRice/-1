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
}