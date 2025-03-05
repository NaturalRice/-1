using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 玩家移动速度
    public SpriteRenderer mapBounds; // 地图的SpriteRenderer组件
    
    public ToolbarUI toolbarUI;
    
    private NPCDialog currentInteractableNPC;// 新增字段：当前可交互的NPC
    
    private Rigidbody2D rigidbody2d;// 控制角色物理行为的Rigidbody2D组件

    private void Start()
    {
        // 获取角色的Rigidbody2D组件
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        // 获取玩家输入
        float moveX = Input.GetAxis("Horizontal"); // 水平方向输入
        float moveY = Input.GetAxis("Vertical");   // 垂直方向输入

        // 计算移动向量
        Vector2 movement = new Vector2(moveX, moveY);

        // 移动玩家
        Vector3 newPosition = transform.position + (Vector3)movement * moveSpeed * Time.deltaTime;

        // 限制玩家在地图边界内移动
        if (mapBounds != null)
        {
            Bounds bounds = mapBounds.bounds;
            newPosition.x = Mathf.Clamp(newPosition.x, bounds.min.x, bounds.max.x);
            newPosition.y = Mathf.Clamp(newPosition.y, bounds.min.y, bounds.max.y);
        }

        // 更新玩家位置
        transform.position = newPosition;

        // 检查玩家是否按下了Tab键
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (UIManager.Instance != null)
            {
                if (UIManager.Instance.TalkPanelGo1.activeSelf && UIManager.Instance.TalkPanelGo1 != null)
                {
                    UIManager.Instance.TalkPanelGo1.SetActive(false);
                    UIManager.Instance.ResumeGame();
                }
                else if (UIManager.Instance.TalkPanelGo2.activeSelf && UIManager.Instance.TalkPanelGo2 != null)
                {
                    UIManager.Instance.TalkPanelGo2.SetActive(false);
                    UIManager.Instance.ResumeGame();
                }
                else if (UIManager.Instance.TalkPanelGo3.activeSelf && UIManager.Instance.TalkPanelGo3 != null)
                {
                    UIManager.Instance.TalkPanelGo3.SetActive(false);
                    UIManager.Instance.ResumeGame();
                }
                else
                {
                    if (UIManager.Instance.TalkPanelGo0.activeSelf && UIManager.Instance.TalkPanelGo0 != null)
                    {
                        UIManager.Instance.CloseTalkPanel();
                    }
                    else
                    {
                        UIManager.Instance.OpenTalkPanel();
                    }
                }
            }
            else
            {
                Debug.LogError("UIManager or TalkPanelGo0 is null!");
            }
        }
        
        // 新增：空格键触发对话
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Talk();
        }
        
        if(Input.GetKeyDown(KeyCode.Q)) {
            currentInteractableNPC?.GetComponent<NPC_AI>().ExecuteSectSkill();
        }

        // 检查玩家是否按下了Delete键
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Debug.Log("Delete key pressed!");
            QuitGame();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Pickable")
        {
            InventoryManager.Instance.AddToBackpack(collision.GetComponent<Pickable>().type);
            Destroy(collision.gameObject);

        }
        
        if (collision.CompareTag("NPC"))
        {
            // 获取NPC的对话组件
            currentInteractableNPC = collision.GetComponent<NPCDialog>();
            Debug.Log("靠近NPC，按空格键对话");
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            currentInteractableNPC = null;
            Debug.Log("离开NPC");
        }
    }

    public void Talk()
    {
        if (currentInteractableNPC != null)
        {
            // 直接打开当前可交互NPC的对话界面
            UIManager.Instance.OpenNPCDialog(currentInteractableNPC);
        }
        else
        {
            Debug.Log("附近没有可交互的NPC");
        }
    }

    public void ThrowItem(GameObject itemPrefab,int count)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject go =  GameObject.Instantiate(itemPrefab);
            Vector2 direction = Random.insideUnitCircle.normalized * 1.2f;
            go.transform.position = transform.position + new Vector3(direction.x,direction.y,0);
            go.GetComponent<Rigidbody2D>().AddForce(direction*3);
        }
    }
    
    private void QuitGame()
    {
        Debug.Log("QuitGame method called!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑器模式下停止播放
#else
        Application.Quit(); // 在发布的游戏中退出
#endif
    }
}