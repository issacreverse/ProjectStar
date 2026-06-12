using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

//이동 형식
public enum MovementStepType
{
    None,                   //없음
    Stay,                   //가만히 있는다. 
    Straight,               //특정 방향으로 직선이동한다.
    MoveTo,                 //특정 좌표로 이동한다
    Sine,                   //기본 방향으로 이동하면서 좌우/상하로 흔들림
    DashToPlayer,           //일정 순간의 플레이어 위치를 보고 돌진. 유도 기능 아님.
    ChasePlayer,            //계속 플레이어를 추적 
    Patrol,                 //두 지점 사이를 왕복 
    Circle,                 //특정 중심을 기준으로 원형으로 이동 
    Bezier,                 //베이즈 곡선 모형: 부드럽게 곡선이동
    Accelerate,             //점점 빨라지거나 점점 느려진다.
    Homing,                 //플레이어 방향으로 회전하며 따라감. (ChasePlayer는 즉시 방향 전환, Homing은 서서히 방향 전환)
    ExitScreen              //Straight와 동일, 하지만 화면 밖을 확실하게 나감을 명시 
}

//보간 방식
public enum EaseType
{
    None, 
    Linear, 
    EaseIn,
    EaseOut,
    EaseInOut
}

//좌표 기준 형식
public enum TargetMode
{
    None, 
    World,
    Relative, 
    SpawnRelative, 
    Player
}
//이동 스텝 종료 조건 
public enum FinishConditionType
{
    None,
    Duration,
    ArriveTarget,
    DurrationOrArriveTarget,
    ExitScreen,
    Never
}
//이동 스텝 데이터 클래스
[Serializable]
public class MovementStepData
{
    public string type = "none";                                    //이동 형식

    //전환 조건
    public string finishCondition = "none";    
    public float arriveDistance = 0f;
    public float screenMargin = 0f;                    

    public float duration = 0f;                                     //
    public float delay = 0f;
    
    public float speed = 0f;
    public float angle = 0f;

    public float targetX = 0f;
    public float targetY = 0f;
    public string targetMode = "none";

    public float amplitude = 0f;
    public float frequency = 0f;
    public float phase = 0f;

    public float radius = 0f;
    public float angularSpeed = 0f;

    public float acceleration = 0f;
    public float maxSpeed = 0f;

    public string easeType = "none";

    public bool faceMoveDirection = false;
    public bool destroyWhenFinished = false;
}

//이동 패턴 데이터 클래스 
[Serializable]
public class MovementPatternData
{
    public string id = "none";                                      //이동 패턴 고유 id

    public MovementStepData[] steps = new MovementStepData[0];      //하나의 이동 패턴을 이루는 이동 스텝 데이터 시퀀스를 저장.
}

//enum 해석 및 제어를 위한 래퍼 클래스 
public class EnemyMovementStep
{
    private MovementStepData data;

    public EnemyMovementStep(MovementStepData _data)
    {
        data = _data;
    }
    public bool CheckCondition(EnemyController _this)
    {
        if(Enum.TryParse<FinishConditionType>(data.finishCondition, out FinishConditionType type))
        {
            switch(type)        //구현부 추가해주세요 
            {
                case FinishConditionType.Duration:
                    if(_this.stepTimer >= data.duration)
                    {
                        return true;
                    }
                    break;
                case FinishConditionType.ArriveTarget:
                    //추가해주세요
                    break;
                case FinishConditionType.DurrationOrArriveTarget:
                    //추가해주세요
                    break;
                case FinishConditionType.ExitScreen:
                    //추가해주세요
                    break;
                case FinishConditionType.Never:
                    return false;
            }
        }
        else
        {
            Debug.Log("Error: Failed to parse to Enum type: FinishConditionType");
        }
        return false;
    }
    //이동 스텝을 중지
    public void Stop()
    {
        
    }
    //이동 스텝을 재생
    public void Play()
    {
        if(Enum.TryParse<MovementStepType>(data.type, out MovementStepType type))
        {
            switch(type)
            {
                case MovementStepType.Stay:
                    Stay(data);
                    break;
                case MovementStepType.Straight:
                    Straight(data);
                    break;
                case MovementStepType.MoveTo:
                    MoveTo(data);
                    break;
                case MovementStepType.Sine:
                    Sine(data);
                    break;
                case MovementStepType.DashToPlayer:
                    DashToPlayer(data);
                    break;
                case MovementStepType.ChasePlayer:
                    ChasePlayer(data);
                    break;
                case MovementStepType.Patrol:
                    Patrol(data);
                    break;
                case MovementStepType.Circle:
                    Circle(data);
                    break;
                case MovementStepType.Bezier:
                    Bezier(data);
                    break;
                case MovementStepType.Accelerate:
                    Accelerate(data);
                    break;
                case MovementStepType.Homing:
                    Homing(data);
                    break;
                case MovementStepType.ExitScreen:
                    ExitScreen(data);
                    break;
            }
        }
        else
        {
            Debug.Log("Error: Failed to parse to Enum type: MovementStepType");
        }
        
    }
    private void Stay(MovementStepData data)
    {
        
    }
    private void Straight(MovementStepData data)
    {
        
    }
    private void MoveTo(MovementStepData data)
    {
        
    }
    private void Sine(MovementStepData data)
    {
        
    }
    private void DashToPlayer(MovementStepData data)
    {
        
    }
    private void ChasePlayer(MovementStepData data)
    {
        
    }
    private void Patrol(MovementStepData data)
    {
        
    }
    private void Circle(MovementStepData data)
    {
        
    }
    private void Bezier(MovementStepData data)
    {
        
    }
    private void Accelerate(MovementStepData data)
    {
        
    }
    private void Homing(MovementStepData data)
    {
        
    }
    private void ExitScreen(MovementStepData data)
    {
        
    }
}
