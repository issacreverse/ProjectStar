using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBossController : EnemyController
{
    //공격을 담당하는 스크립트 
    private EnemyBossAttackController bossAttackController;

    //받아온 정보 
    private Dictionary<int, EnemyBulletPatternData> bulletPatternPhaseTable;

    protected override void GetBulletPatternDataFromManager()
    {
        bulletPatternPhaseTable = DataManager.Instance.GetBulletPatternPhaseTable(enemyId);
        if(bulletPatternPhaseTable != null)
        {
            bossAttackController = GetComponent<EnemyBossAttackController>();
            bossAttackController.Initialize(bulletPatternPhaseTable);
        }
        else
        {
            Debug.Log("Error: Failed to store bullet pattern phase table");
        }
    }
    protected override void Update()
    {
        base.Update();
    }
}
