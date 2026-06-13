using System;
using System.Collections;
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

//변화량 방식
public enum EaseType
{
    None, 
    Linear,                 //처음부터 끝까지 일정한 속도로 이동 
    EaseIn,                 //점점 빨라짐
    EaseOut,                //점점 느려짐
    EaseInOut               //처음에는 느렸다가 중간에는 빨랐다가 끝에서는 다시 느려짐
}

//좌표 기준 형식
public enum TargetMode
{
    None, 
    World,                  //월드 좌표계 기준
    Relative,               //현재 위치 기준 상대 좌표 
    SpawnRelative,          //스폰 위치 기준 상대 좌표 
    Player                  //플레이어 위치 기준 (목적지 = 플레이어)
}
//이동 스텝 종료 조건 
public enum FinishConditionType
{
    None,       
    Duration,                   //일정 시간 경과
    ArriveTarget,               //목적지 도달
    DurrationOrArriveTarget,    //일정 시간 경과 혹은 목적지 도달
    ExitScreen,                 //화면 밖으로 나갔을 때
    Never                       //강제 종료 되기 전까지 계속 유지
}
//이동 스텝 데이터 클래스
[Serializable]
public class MovementStepData
{
    //이동 스텝 방식
    public string type = "none";                                   

    //전환 조건
    public string finishCondition = "none"; 

    //오차 허용치  
    public float arriveDistance = 0f;       //목적지와의 거리 
    public float screenMargin = 0f;         //화면 끝과의 거리      

    //시간
    public float duration = 0f;             //보간등에 사용되는 시간                             
    public float delay = 0f;                //이동 스텝 선딜레이 
    
    //이동 속성 값
    public float speed = 0f;                //속도
    public float angle = 0f;                //각도

    //목적지 속성 값
    public float targetX = 0f;              //목적지 x좌표
    public float targetY = 0f;              //목적지 y좌표
    public string targetMode = "none";

    //반복 이동 속성 값: Sine의 파형, Patrol의 왕복 이동
    public float amplitude = 0f;            //Sine: 흔들리는 폭 / Patrol: 왕복 이동 범위 
    public float frequency = 0f;            //1초에 흔들리는 횟수
    public float phase = 0f;                //흔들리는 최종 위치 가산값

    //원형 속성 값
    public float radius = 0f;               //반지름
    public float angularSpeed = 0f;         //각속도(속도)

    //베지에 곡선 속성 값
    public float controlX = 0f;             //변곡점 x좌표
    public float controlY = 0f;             //변곡점 y좌표

    //가속 속성 값
    public float acceleration = 0f;         //가속도
    public float maxSpeed = 0f;             //최대 제한 속도

    //변화량 곡선
    public string easeType = "none";        

    //이동 방향으로 고개 바라보게끔 할 건지 여부 
    public bool faceMoveDirection = false;

    //이동 끝나면 오브젝트 자동 제거할 것인지 여부 
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
    //데이터 받아오기
    private MovementStepData data;
    private EnemyController e;
    private PlayerController p;

    //적 transform 값
    private Transform transform; 
    //플레이어 transform값
    private Transform player;

    //마지막 이동 방향 저장하는 임시 변수 
    private Vector3 lastMoveDirection;
    //스폰 위치 저장해두는 변수
    private Vector3 spawnPosition;

    //목적지 정보 저장
    private Vector3 target;

    //이동 스텝 타이머
    private float timeElapsed; 

    public EnemyMovementStep(EnemyController _e, MovementStepData _data)
    {
        e = _e;
        data = _data;
        transform = _e.transform;
        player = _e.player.transform;
        lastMoveDirection = Vector3.left;
        spawnPosition = transform.position;

        target = new Vector3(data.targetX, data.targetY, 0);

        timeElapsed = 0f;
    }
    #region Condition Check Functions()
    public bool IsFinished()
    {
        if(Enum.TryParse<FinishConditionType>(data.finishCondition, out FinishConditionType type))
        {
            switch(type)        //구현부 추가해주세요 
            {
                case FinishConditionType.Duration:
                    return timeElapsed >= data.duration;

                case FinishConditionType.ArriveTarget:
                    return IsArrived(target, data.arriveDistance);

                case FinishConditionType.DurrationOrArriveTarget:
                    return (timeElapsed >= data.duration) || IsArrived(target, data.arriveDistance);

                case FinishConditionType.ExitScreen:
                    return IsOutOfScreen(data.screenMargin);

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
    private bool IsArrived(Vector3 target, float arriveDistance)
    {
        return Vector3.Distance(transform.position, target) <= arriveDistance;
    }
    private bool IsOutOfScreen(float margin)
    {
        if(Camera.main == null)
            return false;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewPos.x < -margin ||
               viewPos.x > 1f + margin ||
               viewPos.y < -margin ||
               viewPos.y > 1f + margin;
    }
    #endregion

    #region Calculate Functions()
    //각도를 받아서 방향을 나타내는 벡터값으로 변환하는 함수 
    private Vector3 AngleToDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;

        return new Vector3(
            Mathf.Cos(rad),
            Mathf.Sin(rad),
            0f
        ).normalized;
    }
    //목적지를 좌표 기준 형식에 따라 설정해주는 함수 
    private Vector3 ResolveTargetPosition(MovementStepData data, Vector3 currentPosition)
    {
        Vector3 target = new Vector3(data.targetX, data.targetY, currentPosition.z);

        if(data.targetMode == "Relative")
        {
            return currentPosition + target;
        }

        if(data.targetMode == "Player")
        {
            if(player != null)
                return new Vector3(player.position.x, player.position.y, transform.position.z);

            return currentPosition;
        }

        if(data.targetMode == "SpawnRelative")
        {
            return spawnPosition + target;
        }

        if(data.targetMode == "World")
        {
            return new Vector3(data.targetX, data.targetY, currentPosition.z);
        }

        return target;
    }
    //변화량을 반영함. 일정 속도로 이동하는 것에서 다른 것으로 
    private float ApplyEase(float t, string easeType)
    {
        switch (easeType)
        {
            case "EaseIn":
                return t * t;

            case "EaseOut":
                return 1f - (1f - t) * (1f - t);

            case "EaseInOut":
                return t < 0.5f
                    ? 2f * t * t
                    : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

            default:
                return t;
        }
    }
    //베지에 곡선 계산 함수. 다음 프레임에 이동할 위치를 계산해준다. 
    private Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;

        return u * u * p0
             + 2f * u * t * p1
             + t * t * p2;
    }
    //이동 방향에 맞추어 적의 고개를 틀어 조정해준다. //적의 스프라이트가 어느쪽을 향해있는지에 따라 코드 바뀌기 때문에 체크 필요 !!!
    private void ApplyFaceDirection(MovementStepData data, Vector3 direction)
    {
        if(!data.faceMoveDirection)
            return;

        if(direction.sqrMagnitude <= 0.0001f)
            return;

        //적의 스프라이트가 왼쪽을 바라보고 있다고 가정. 
        transform.right = -direction;
    }
    #endregion

    #region Control Functions()
    //이동 스텝을 재생, 각 스텝의 이동은 코루틴으로 실행됨. enemyController에서 이 함수를 호출(코루틴 실행).
    public IEnumerator Play()
    {
        if(Enum.TryParse<MovementStepType>(data.type, out MovementStepType type))
        {
            switch(type)
            {
                case MovementStepType.Stay:
                    yield return Stay(data);
                    break;
                case MovementStepType.Straight:
                    yield return Straight(data);
                    break;
                case MovementStepType.MoveTo:
                    yield return MoveTo(data);
                    break;
                case MovementStepType.Sine:
                    yield return Sine(data);
                    break;
                case MovementStepType.DashToPlayer:
                    yield return DashToPlayer(data);
                    break;
                case MovementStepType.ChasePlayer:
                    yield return ChasePlayer(data);
                    break;
                case MovementStepType.Patrol:
                    yield return Patrol(data);
                    break;
                case MovementStepType.Circle:
                    yield return Circle(data);
                    break;
                case MovementStepType.Bezier:
                    yield return Bezier(data);
                    break;
                case MovementStepType.Accelerate:
                    yield return Accelerate(data);
                    break;
                case MovementStepType.Homing:
                    yield return Homing(data);
                    break;
                case MovementStepType.ExitScreen:
                    yield return ExitScreen(data);
                    break;
            }
        }
        else
        {
            Debug.Log("Error: Failed to parse to Enum type: MovementStepType");
        }
        
    }
    private void FinishStep()
    {
        //enemyController에게 알리기.
        if(data.destroyWhenFinished)
        {
            e.DestroyWhenFinished();
        }
        e.NextMovement();
    }
    #endregion

    #region movementType Functions()
    //아래는 이동 처리 함수들...
    //
    //
    public IEnumerator Stay(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;

        while(!IsFinished())
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        FinishStep();
    }
    public IEnumerator Straight(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);
        
        timeElapsed = 0f;
        Vector3 direction = AngleToDirection(data.angle);

        //매프레임 방향을 조정해서 조금씩 움직이는 방식
        while (!IsFinished())
        {
            Vector3 move = direction * data.speed * Time.deltaTime;
            transform.position += move;

            lastMoveDirection = direction;                  
            ApplyFaceDirection(data, direction);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    public IEnumerator MoveTo(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;

        Vector3 from = transform.position;
        //목적지 설정은 한 번만. 
        Vector3 to = ResolveTargetPosition(data, from);

        while (!IsFinished())
        {
            Vector3 before = transform.position;

            //duration 값이 0보다 크면 duration에 맞춰서 부드럽게 이동. 
            if(data.duration > 0f)
            {
                float t = Mathf.Clamp01(timeElapsed / data.duration);
                t = ApplyEase(t, data.easeType);

                transform.position = Vector3.Lerp(from, to, t);
            }
            //duration 값이 0이면 고정 speed값으로 목적지까지 이동.
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    to,
                    data.speed * Time.deltaTime
                );
            }

            Vector3 moveDir = transform.position - before;

            //이동 했을 경우 
            if(moveDir.sqrMagnitude > 0.0001f)
            {      
                //이동 방향으로 고개 틀게끔 조정 
                lastMoveDirection = moveDir.normalized;
                ApplyFaceDirection(data, lastMoveDirection);
            }

            if(IsArrived(to, data.arriveDistance))
                break;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    public IEnumerator Sine(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;

        Vector3 start = transform.position;
        Vector3 forwardDir = AngleToDirection(data.angle);
        Vector3 sideDir = new Vector3(-forwardDir.y, forwardDir.x, 0f);

        while (!IsFinished())
        {
            Vector3 before = transform.position;

            Vector3 forwardMove = forwardDir * data.speed * timeElapsed;

            float sineValue = Mathf.Sin(
                timeElapsed * data.frequency * Mathf.PI * 2f + data.phase
            );

            Vector3 sineMove = sideDir * sineValue * data.amplitude;

            transform.position = start + forwardMove + sineMove;

            Vector3 moveDir = transform.position - before;

            if(moveDir.sqrMagnitude > 0.0001f)
            {
                lastMoveDirection = moveDir.normalized;
                ApplyFaceDirection(data, lastMoveDirection);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    public IEnumerator DashToPlayer(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;

        Vector3 direction = Vector3.left;

        //목적지 방향 계산 한 번만.
        if(player != null)
        {
            direction = (player.position - transform.position).normalized;
        }

        while (!IsFinished())
        {
            transform.position += direction * data.speed * Time.deltaTime;

            lastMoveDirection = direction;
            ApplyFaceDirection(data, direction);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    public IEnumerator ChasePlayer(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;

        while (!IsFinished())
        {
            //목적지 방향 계산 매 프레임마다 반복.
            if(player != null)
            {
                Vector3 direction = (player.position - transform.position).normalized;

                transform.position += direction * data.speed * Time.deltaTime;

                lastMoveDirection = direction;
                ApplyFaceDirection(data, direction);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    public IEnumerator Patrol(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;

        //시작 위치를 중심으로 axis를 따라 왕복 이동
        Vector3 center = transform.position;
        Vector3 axis = AngleToDirection(data.angle);

        while (!IsFinished())
        {
            Vector3 before = transform.position;

            float offset = Mathf.Sin(timeElapsed * data.speed) * data.amplitude;
            transform.position = center + axis * offset;

            Vector3 moveDir = transform.position - before;

            if(moveDir.sqrMagnitude > 0.0001f)
            {
                lastMoveDirection = moveDir.normalized;
                ApplyFaceDirection(data, lastMoveDirection);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    public IEnumerator Circle(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;

        Vector3 center = transform.position;
        float startAngle = data.angle;

        while (!IsFinished())
        {
            Vector3 before = transform.position;

            float currentAngle = startAngle + data.angularSpeed * timeElapsed;
            //호 길이 구하기
            Vector3 offset = AngleToDirection(currentAngle) * data.radius;

            //호 길이만큼 이동 
            transform.position = center + offset;

            Vector3 moveDir = transform.position - before;

            if(moveDir.sqrMagnitude > 0.0001f)
            {
                lastMoveDirection = moveDir.normalized;
                ApplyFaceDirection(data, lastMoveDirection);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    public IEnumerator Bezier(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;
        //p0: 시작점, p1: 변곡점, p2: 도착점
        Vector3 p0 = transform.position;
        Vector3 p1 = new Vector3(data.controlX, data.controlY, transform.position.z);
        Vector3 p2 = ResolveTargetPosition(data, p0);

        while (!IsFinished())
        {
            Vector3 before = transform.position;

            //진행률(0~1)
            float t = 0f;

            if(data.duration > 0f)
                t = Mathf.Clamp01(timeElapsed / data.duration);

            //진행률 변화 곡선 적용 
            t = ApplyEase(t, data.easeType);

            //베지에 곡선에 따른 다음 이동 값 저장 
            transform.position = GetBezierPoint(p0, p1, p2, t);

            Vector3 moveDir = transform.position - before;

            if(moveDir.sqrMagnitude > 0.0001f)
            {
                lastMoveDirection = moveDir.normalized;
                ApplyFaceDirection(data, lastMoveDirection);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    public IEnumerator Accelerate(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;
        float currentSpeed = data.speed;

        Vector3 direction = AngleToDirection(data.angle);

        while (!IsFinished())
        {
            //가속 반영 
            currentSpeed += data.acceleration * Time.deltaTime;

            //최대 속도 초과 시 제한 
            if(data.maxSpeed > 0f)
                currentSpeed = Mathf.Min(currentSpeed, data.maxSpeed);

            transform.position += direction * currentSpeed * Time.deltaTime;

            lastMoveDirection = direction;
            ApplyFaceDirection(data, direction);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    //ChasePlayer와 비슷하나 회전속도 제한이 있어서 좀 더 부드럽게 따라감. 피할 수 있는 유도탄 개념 
    public IEnumerator Homing(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        timeElapsed = 0f;

        Vector3 currentDirection = lastMoveDirection;

        if(currentDirection == Vector3.zero)
            currentDirection = AngleToDirection(data.angle);

        while (!IsFinished())
        {
            if(player != null)
            {
                Vector3 targetDirection = (player.position - transform.position).normalized;

                //바로 direction 값을 넣는 것이 아니라 RotateTowards로 부드럽게 회전
                currentDirection = Vector3.RotateTowards(
                    currentDirection,
                    targetDirection,
                    data.angularSpeed * Mathf.Deg2Rad * Time.deltaTime,
                    0f
                ).normalized;
            }

            transform.position += currentDirection * data.speed * Time.deltaTime;

            lastMoveDirection = currentDirection;
            ApplyFaceDirection(data, currentDirection);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FinishStep();
    }
    public IEnumerator ExitScreen(MovementStepData data)
    {
        if(data.delay > 0f)
            yield return new WaitForSeconds(data.delay);

        //한 방향으로 계속 이동 
        Vector3 direction = AngleToDirection(data.angle);

        //화면 밖 나가면 종료 
        while (!IsOutOfScreen(data.screenMargin))
        {
            transform.position += direction * data.speed * Time.deltaTime;

            lastMoveDirection = direction;
            ApplyFaceDirection(data, direction);

            yield return null;
        }

        FinishStep();
    }
    #endregion
}
