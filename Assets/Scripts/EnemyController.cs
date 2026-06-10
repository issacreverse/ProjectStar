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
    protected float moveSpeed;
    protected float moveDir = 1f;

    //공격을 담당하는 스크립트 
    private EnemyAttackController attackController;

    //받아온 적 정보 
    protected EnemyData data;
    private EnemyBulletPatternData bulletPatternData;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        //적 속성 값 내려받기
        data = DataManager.Instance.GetEnemyData(enemyId);
        if(data != null)
        {
            moveSpeed = data.moveSpeed;
        }
        
        //탄알 패턴 속성 값 내려받기 
        GetBulletPatternDataFromManager();
        
    }
    protected virtual void GetBulletPatternDataFromManager()
    {
        bulletPatternData = DataManager.Instance.GetEnemyBulletPatternData(enemyId);
        if(bulletPatternData != null)
        {
            attackController = GetComponent<EnemyAttackController>();
            attackController.Initialize(bulletPatternData);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {   
        //적 이동 관련 
        if(transform.position.y < -4)
        {
            moveDir = 1f;
        }
        if(transform.position.y > 4)
        {
            moveDir = -1f;
        }
        transform.Translate(Vector2.up * moveDir * moveSpeed * Time.deltaTime);
    } 
    
    //다른 스크립트들은 이 함수를 사용하여 해당스크립트로부터 id를 가져옵니다. 이 스크립트가 본체라는 뜻.
    public string GetEnemyId()
    {
        return enemyId;
    }

}
