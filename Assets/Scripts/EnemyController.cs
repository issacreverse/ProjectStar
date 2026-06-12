using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //적의 이동과 공격을 관장하는 스크립트입니다. 
    //적의 공격은 추가적으로 별도의 EnemyAttackController 스크립트에서 제어합니다.
    //적의 체력, 상태 판정 같은 필드 정보들은 같은 오브젝트 아래에 있는 EnemyField 스크립트에서 제어합니다.
    //Id 정보는 이 스크립트에 저장되어있으며: 추후 스폰 시스템이 제어하겠지만
    //다른 스크립트들은 이 스크립트에서 GetEnemyId 함수를 통해 Id를 가져오되 Data는 DataManager에서 직접 가져옵니다. (호출 순서 문제 때문)

    //적 정보 
    [SerializeField] protected string enemyId = "Enemy1";

    //적 속성 관련 필드
    public MovementPatternData movementPatternData;             //EnemyBossPhaseActionData 클래스의 DoAction()에서 접근해야돼서 public이다... 
    private EnemyMovementStep stepObj;                          //스텝 시작/일시중단 제어용 래퍼 객체 
    private MovementStepData[] steps;                           //현재 이동 패턴의 스텝들
    private int stepIdx;                                        //현재 이동 패턴의 스텝 인덱스 

    public float stepTimer;                                     //이동 스텝 duration 계산에 필요한 값 

    //공격을 담당하는 스크립트 
    private EnemyAttackController attackController;

    //받아온 적 정보 
    protected EnemyData data;
    private EnemyBulletPatternData bulletPatternData;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //적 속성 값 내려받기
        data = DataManager.Instance.GetEnemyData(enemyId);

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
        movementPatternData = DataManager.Instance.GetMovementPatternData(data.movementPatternDataId);
        //최초 이동 패턴을 실행합니다. (보스몹도 첫번째 페이즈 실행 완료 시점이다)
        StartMovement();
    }

    // Update is called once per frame
    protected virtual void Update()
    {   
        stepTimer += Time.deltaTime;
        
        //이동 스텝 종료 조건을 만족하면 전 스텝을 멈추고, 다음 스텝을 재생합니다.
        if(stepObj.CheckCondition(this))
        {
            stepObj.Stop();
            stepIdx++;
            if(stepIdx < steps.Length)
            {
                stepObj = new EnemyMovementStep(steps[stepIdx]);
                stepObj.Play();

                stepTimer = 0f;
            }
        }
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
        stepObj.Stop();
        StartMovement();
    }
    
    //현재 movementPatternData에 들어있는 이동 패턴을 실행합니다. movementPatternData 객체를 생성합니다.
    private void StartMovement()
    {   
        steps = movementPatternData.steps;
        stepIdx = 0;
        
        stepObj = new EnemyMovementStep(steps[stepIdx]);
        stepObj.Play();

        stepTimer = 0f;
    }
}
