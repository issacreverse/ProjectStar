using UnityEngine;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;


public class EnemyBossAttackController : EnemyAttackController
{   
    //적의 공격만을 담당하는 스크립트. EnemyController에 들어있다. 
    
    //공격에 필요한 속성이 담겨있는 타입 
    private List<EnemyBossPhaseActionData> phaseTable;
    private List<ConditionActionObject> phaseObjects;

    //페이즈 번호
    private int phaseIdx;

    //적 정보를 받아온다. 
    [HideInInspector] public EnemyField _enemyField;

    //부모 클래스의 St

    //초기화. 받는 매개변수가 부모클래스와 다르다.
    public void Initialize(List<EnemyBossPhaseActionData> table)
    {
        phaseTable = new List<EnemyBossPhaseActionData>();
        phaseTable = table;
        phaseIdx = 0;
        MakeConditionActionObjects();

        //페이즈 액션에서 제어하는 값 참조 가져오기 
        _enemyField = GetComponent<EnemyField>();

        //첫번째 페이즈로 초기화
        if(phaseObjects.Count > 0)
        {
            phaseObjects[0].DoAction(this);
        }
    }

    protected override void Update()
    {
        base.Update();

        //보스의 페이즈 조건을 검사해서 해당 조건에 맞는 페이즈 액션을 실행한다.
        CheckBossPhase();
    }
    private void MakeConditionActionObjects()
    {
        phaseObjects = new List<ConditionActionObject>();
        foreach(var data in phaseTable)
        {
            PhaseCondition o1 = new PhaseCondition(data.condition.type, data.condition.value);
            PhaseAction[] o2s = new PhaseAction[data.actions.Length];
            int idx = 0;
            foreach(var action in data.actions)
            {
                PhaseAction o2 = new PhaseAction(action.type, action.bulletPatternId, action.value);
                o2s[idx++] = o2;
            }
            ConditionActionObject obj = new ConditionActionObject(o1, o2s);
            phaseObjects.Add(obj);
        }
    }
    private void CheckBossPhase()
    {
        if(phaseIdx+1 >= phaseObjects.Count)
            return;

        if(phaseObjects[phaseIdx+1].CheckCondition(this))
        {
            phaseIdx++;
            phaseObjects[phaseIdx].DoAction(this);
        }
    }
}


