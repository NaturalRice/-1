using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 玩家移动速度
    public SpriteRenderer mapBounds; // 地图的SpriteRenderer组件

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
    }
}