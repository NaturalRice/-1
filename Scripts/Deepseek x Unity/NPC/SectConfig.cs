using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSectConfig", menuName = "Sect Config")]
public class SectConfig : ScriptableObject
{
    [Header("门派设置")]
    public NPC_AI.SectType sectType; // 门派类型
    public Color sectColor; // 门派颜色
    public GameObject skillPrefab; // 门派技能预制体
}
