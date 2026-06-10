using UnityEngine;
using System.Collections.Generic;

public class EnemyBossAttackController : EnemyAttackController
{   
    //적의 공격만을 담당하는 스크립트. EnemyController에 들어있다. 
    
    //공격에 필요한 속성이 담겨있는 타입 
    private Dictionary<int, EnemyBulletPatternData> bulletPatternPhaseTable;

    //보스 페이즈 상태
    private int phase;
    //페이즈 전환에 필요한 정보를 받아온다. 
    private EnemyField _enemyField;

    //초기화. 받는 매개변수가 부모클래스와 다르다.
    public void Initialize(Dictionary<int, EnemyBulletPatternData> table)
    {
        bulletPatternPhaseTable = table;
        phase = 1;
        if(bulletPatternPhaseTable.TryGetValue(phase, out EnemyBulletPatternData _data))
        {
            data = _data;
        }
        else
        {
            Debug.Log("Error: Failed to initialize Boss bullet pattern data");
        }
        _enemyField = GetComponent<EnemyField>();
    }

    protected override void Update()
    {
        //보스의 현재 페이즈를 계산한다. 페이즈가 변경됐다면 보스의 패턴도 바뀐다. 
        int temp = GetPhase();
        if(phase != temp)
        {
            phase = temp;
            if(bulletPatternPhaseTable.TryGetValue(phase, out EnemyBulletPatternData _data))
            {
                data = _data;
            }
            else
            {
                Debug.Log($"Error: cannot find phase {phase}.");
            }
        }
        
        base.Update();
    }

    //보스의 현재 페이즈 번호를 반환한다. 
    //나중에 구조 바꿀 건데 지금은 이렇게 하자 좀...
    private int GetPhase()
    {
        float hitPoints = _enemyField.GetHitPoints();
        float hitPointsMax = _enemyField.GetHitPointsMax();
        if(hitPoints <= hitPointsMax * 0.6f)
        {
            if(hitPoints <= hitPointsMax * 0.3f)
            {
                Debug.Log("Phase: 3");
                return 3;
            }
            Debug.Log("Phase: 2");
            return 2;
        }
        Debug.Log("Phase: 1");
        return 1;
    }
}
