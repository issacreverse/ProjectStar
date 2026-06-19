using UnityEngine;
using System.Collections;
using System;

public class EnemyController : MonoBehaviour
{
    //적의 이동과 공격을 관장하는 스크립트입니다. 
    //적의 공격은 추가적으로 별도의 EnemyAttackController 스크립트에서 제어합니다.
    //적의 체력, 상태 판정 같은 필드 정보들은 같은 오브젝트 아래에 있는 EnemyField 스크립트에서 제어합니다.
    //Id 정보는 이 스크립트에 저장되어있으며: 추후 스폰 시스템이 제어하겠지만
    //다른 스크립트들은 이 스크립트에서 GetEnemyId 함수를 통해 Id를 가져오되 Data는 DataManager에서 직접 가져옵니다. (호출 순서 문제 때문)


    //상수 값
    private const float TOUCH_DAMAGE_COOLTIME = 1f;

    //적 정보 
    [SerializeField] protected string enemyId = "Enemy1";

    //적 속성 관련 필드
    [NonSerialized] public MovementPatternData movementPatternData;             //EnemyBossPhaseActionData 클래스의 DoAction()에서 접근해야돼서 public이다... 
    private EnemyMovementStep stepObj;                                          //스텝 시작/일시중단 제어용 래퍼 객체 
    private MovementStepData[] steps;                                           //현재 이동 패턴의 스텝들
    private int stepIdx;                                                        //현재 이동 패턴의 스텝 인덱스 

    //현재 실행 중인 이동 스텝 코루틴
    private Coroutine currentMovementCoroutine;

    //공격을 담당하는 스크립트 
    private EnemyAttackController attackController;
    
    //플레이어 정보
    [NonSerialized] public GameObject player;

    //받아온 적 정보 
    protected EnemyData data;
    private EnemyBulletPatternData bulletPatternData;

    //적 충돌 정보: 몇 초마다 충돌 처리 할 건지
    private float touchDamageCoolTime = TOUCH_DAMAGE_COOLTIME;
    private bool istouchDamageReady;
    private float touchDamage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.Log("Error: Can't Find Player Object");
            return;
        }
        //적 속성 값 내려받기
        data = DataManager.Instance.GetEnemyData(enemyId);

        istouchDamageReady = true;
        touchDamage = data.touchDamage;

        //패턴 속성 값 내려받기 
        GetPatternDataFromManager();
    }

    //적의 탄알 패턴 및 이동 패턴을 DataManager로부터 받아옵니다.
    protected virtual void GetPatternDataFromManager()
    {
        //탄알 패턴을 받아온다. 
        string bulletPatternId = DataManager.Instance.GetEnemyData(enemyId).bulletPatternId;
        bulletPatternData = DataManager.Instance.GetEnemyBulletPatternData(bulletPatternId);  
        if(bulletPatternData != null)
        {
            attackController = GetComponent<EnemyAttackController>();
            attackController.Initialize(bulletPatternData);
        }
        //이동 패턴을 받아온다.
        movementPatternData = DataManager.Instance.GetMovementPatternData(data.movementPatternId);

        //최초 이동 패턴을 실행합니다. (보스몹도 첫번째 페이즈 실행 완료 시점이다)
        StartMovement();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    } 
    
    //다른 스크립트들은 이 함수를 사용하여 해당스크립트로부터 id를 가져옵니다. 이 스크립트가 본체라는 뜻.
    public string GetEnemyId()
    {
        return enemyId;
    }
    //이동 패턴이 바꿀 일이 있는 보스만 호출. 
    public void SetMovementPattern(string movementPatternId)
    {
        movementPatternData = DataManager.Instance.GetMovementPatternData(movementPatternId);

        //이전 거 중지하고 새로운 거 실행
        if(currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
            currentMovementCoroutine = null;
        }
        StartMovement();
    }
    //필드 변경 패턴이 있는 보스만 호출
    public void SetField(string changeFieldType, float value)
    {
        if(Enum.TryParse<ChangeFieldType>(changeFieldType, out ChangeFieldType type))
        {
            switch(type)
            {
                case ChangeFieldType.ChangeMoveSpeed:
                    stepObj.ChangeMoveSpeed(value);
                    break;
                case ChangeFieldType.ChangeHitPoints:
                    gameObject.GetComponent<EnemyField>().ChangeHitPoints(value);
                    break;
                case ChangeFieldType.ChangeTouchDamage:
                    touchDamage = value;
                    break;
                default: 
                    break;
            }
        }
        else
        {
            Debug.Log("Error: Failed to parse to Enum type ChangeFieldType");
        }

    }
    //현재 movementPatternData에 들어있는 이동 패턴을 실행합니다. movementPatternData 객체를 생성합니다.
    private void StartMovement()
    {   
        if(movementPatternData == null)
            return;
        steps = movementPatternData.steps;
        stepIdx = 0;
        

        stepObj = new EnemyMovementStep(this, steps[stepIdx]);
        currentMovementCoroutine = StartCoroutine(stepObj.Play());

    }
    //stepObj의 FinishStep()이 호출되면 자동으로 이 함수를 호출하여 다음 스텝 실행을 시도합니다. 
    public void NextMovement()
    {
        stepIdx++;

        if(stepIdx >= steps.Length)
            return;

        stepObj = new EnemyMovementStep(this, steps[stepIdx]);
        currentMovementCoroutine = StartCoroutine(stepObj.Play());
    }
    public void DestroyWhenFinished()
    {
        WaveManager.Instance.RemoveEnemyFromList(this);
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(istouchDamageReady)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(ApplyTouchDamage());
            }
        }
        if(other.gameObject.CompareTag("BulletBoundary"))
        {
            gameObject.GetComponent<EnemyField>().Die();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(istouchDamageReady)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(ApplyTouchDamage());
            }
        }
    }

    public IEnumerator ApplyTouchDamage()
    {
        istouchDamageReady = false;
        player.GetComponentInChildren<PlayerCharacterBase>().TakeDamage(data.touchDamage);
        if(data.destroyWhenTouch)
        {
            gameObject.GetComponent<EnemyField>().Die();
        }
        yield return new WaitForSeconds(touchDamageCoolTime);
        istouchDamageReady = true;
    }
}
