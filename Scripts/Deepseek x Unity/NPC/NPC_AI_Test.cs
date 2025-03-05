using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class NPC_AI_Tests {
    [Test]
    public void SwordSectTraining_IncreasesAttack() {
        var npc = new GameObject().AddComponent<NPCDialog>(); // 改为测试NPCDialog
        var ai = npc.gameObject.AddComponent<NPC_AI>();
        ai.InitializeBySect(NPC_AI.SectType.剑宗);
        npc.OnTrainingSelected(1); // 通过NPCDialog调用
        Assert.AreEqual(25, ai.attack);
    }
}
