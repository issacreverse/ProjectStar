using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBossController : EnemyController
{
    //공격을 담당하는 스크립트 
    private EnemyBossAttackController bossAttackController;

    //받아온 정보 
    private List<EnemyBossPhaseActionData> phaseActionTable;

    //EnemyController의 같은 함수를 오버라이딩합니다. 보스의 경우 탄알 패턴이 아니라 페이즈 패턴을 받아옵니다. 또한 보스는 이동 패턴을 지금 계산하지 않습니다. 
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

        //이동 패턴을 지금 받아오지 않는다. 페이즈 실행 시 그곳에서 처리한다.
    }
    protected override void Update()
    {
        base.Update();
    }
}
