using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //적 정보 
    [SerializeField] private string enemyId = "Enemy1";

    //적 속성 관련 필드
    [SerializeField] private float moveSpeed;
    private float moveDir = 1f;

    //공격을 담당하는 스크립트 
    private EnemyAttackController attackController;

    //받아온 적 정보 
    EnemyData data;
    EnemyBulletPatternData bulletPatternData;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //적 속성 값 내려받기
        data = DataManager.Instance.GetEnemyData(enemyId);
        if(data != null)
        {
            moveSpeed = data.moveSpeed;
        }
        
        //탄알 패턴 속성 값 내려받기 
        bulletPatternData = DataManager.Instance.GetEnemyBulletPatternData(enemyId);
        if(bulletPatternData != null)
        {
            attackController = GetComponent<EnemyAttackController>();
            attackController.Initialize(bulletPatternData);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        //적 이동 관련 
        if(transform.position.y < -5)
        {
            moveDir = 1f;
        }
        if(transform.position.y > 5)
        {
            moveDir = -1f;
        }
        transform.Translate(Vector2.up * moveDir * moveSpeed * Time.deltaTime);
    } 
}
