using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBossController : EnemyController
{
    //공격을 담당하는 스크립트 
    private EnemyBossAttackController bossAttackController;

    //받아온 정보 
    private List<EnemyBossPhaseActionData> phaseActionTable;

    //EnemyController의 같은 함수를 오버라이딩합니다. 보스의 경우 탄알 패턴이 아니라 페이즈 패턴을 받아옵니다. 
    protected override void GetPatternDataFromManager()
    {
        phaseActionTable = DataManager.Instance.GetEnemyBossPhaseActionData(enemyId);
        if(phaseActionTable != null)
        {
            bossAttackController = GetComponent<EnemyBossAttackController>();
            bossAttackController.Initialize(phaseActionTable);
        }
        else
        {
            Debug.Log("Error: Failed to get Enemy Boss Phase Action Data");
        }
    }
    protected override void Update()
    {
        base.Update();
    }
}
