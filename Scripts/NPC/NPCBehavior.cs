// NPCBehavior.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCBehavior", menuName = "AI/NPC Behavior")]
public class NPCBehavior : ScriptableObject
{
    [Header("移动设置")]
    public float moveSpeed = 3f;
    public float detectionRange = 5f;

    [Header("行为参数")] 
    public float aggression = 0.5f;
    public float friendliness = 0.5f;
}
