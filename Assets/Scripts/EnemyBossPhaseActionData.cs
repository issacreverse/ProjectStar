using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//페이즈 조건문 종류
public enum PhaseConditionType
{
    None,
    Default,                //페이즈 1 조건. (=조건 없음)
    HpBelowRatio,           //일정 비율 미만으로 체력이 떨어졌을 때
    TimeElapsed,            //적이 스폰된 후 일정 시간이 경과했을 때
    PatternFinished         //적의 패턴이 끝났을 때         //미정
}
public class PhaseCondition
{
    PhaseConditionType type;
    float value;

    public PhaseCondition(string _type, float _value)
    {
        if(Enum.TryParse(_type, out PhaseConditionType type))
        {
            this.type = type;
        }
        else
        {
            Debug.Log("Error: Failed parsing to PhaseConditionType");
        }
        value = _value;
    }
    public bool CheckCondition(EnemyBossAttackController _this)
    {
        switch(type)
        {
            case PhaseConditionType.Default:
                return true;
            case PhaseConditionType.HpBelowRatio:
                return (_this._enemyField.GetHitPoints() <= _this._enemyField.GetHitPointsMax() * value);
            case PhaseConditionType.TimeElapsed:
                //추가해주세요
                break;
            case PhaseConditionType.PatternFinished:
                //추가해주세요
                break;
        }
        return false;
    }
}
//페이즈 행동 종류
public enum PhaseActionType
{
    None,
    ChangeBulletPattern,    //탄알 패턴 변경
    ChangeMovementPattern,      //이동 패턴 변경
    ChangeField             //체력, 이동 속도 등 적 상태 변경
}
public class PhaseAction
{
    PhaseActionType type;
    string bulletPatternId;             //이 코드 좀 구린 듯. 여기에 저장하지 말 걸. 초기화도 굳이 굳이 필요없는데.
    string movementPatternId;
    float value; 

    public PhaseAction(string _type, string _bulletPatternId, string _movementPatternId, float _value)
    {
        if(Enum.TryParse(_type, out PhaseActionType type))
        {
            this.type = type;
        }
        else
        {
            Debug.Log("Error: Failed parsing to PhaseActionType");
        }
        bulletPatternId = _bulletPatternId;
        movementPatternId = _movementPatternId;
        value = _value;
    }
    public void DoAction(EnemyBossAttackController _this)
    {
        switch(type)
        {
            case PhaseActionType.ChangeBulletPattern: 
                Debug.Log($"Set BulletPattern to: {bulletPatternId}");
                _this.SetBulletPattern(bulletPatternId);
                break;
            case PhaseActionType.ChangeMovementPattern:
                Debug.Log($"Set MovementPattern to: {movementPatternId}");
                _this.enemyController.SetMovementPattern(movementPatternId);
                break;
            case PhaseActionType.ChangeField:
                //이것도 어떤 필드 바뀔 건지 enum 타입 만들고 json 형식도 바꾸고...엉엉 
                break;
        }
    }
}

public class ConditionActionObject
{
    PhaseCondition condition;
    PhaseAction[] actions;

    public ConditionActionObject(PhaseCondition condition, PhaseAction[] actions)
    {
        this.condition = condition;
        this.actions = actions;
    }
    public bool CheckCondition(EnemyBossAttackController _this)
    {
        return condition.CheckCondition(_this);
    }
    public void DoAction(EnemyBossAttackController _this)
    {
        foreach(var action in actions)
        {
            action.DoAction(_this);
        }
    }
}

//페이즈 별 행동 정보를 담고 있는 데이터 타입
[Serializable]
public class EnemyBossPhaseActionData
{
    public string id = "none";                                              //페이즈 데이터 고유 id
    
    public PhaseConditionData condition = new PhaseConditionData();         //페이즈 조건 데이터
    public PhaseActionData[] actions = new PhaseActionData[0];              //페이즈 행동 데이터
}

//페이즈의 분기 조건 타입
[Serializable]
public class PhaseConditionData
{
    public string type = "none";                            //페이즈 조건 타입 (Enum PhaseConditionType으로 변환할 거임)

    public float value = 0f;                                //페이즈 조건문에 쓰이는 값
}

//특정 페이즈에 취하는 행동 혹은 상태 변화 타입
[Serializable]
public class PhaseActionData
{
    public string type = "none";                            //페이즈 행동 타입 (Enum PhaseActionType으로 변환할 거임)

    public string bulletPatternId = "none";                 //페이즈 행동 중 탄알 패턴 변화 시 바뀔 탄알 패턴 id

    public string movementPatternId = "none";               //페이즈 행동 중 이동 패턴 변화 시 바뀔 이동 패턴 id

    public float value = 0f;                                //페이즈 행동에 쓰이는 값
}
