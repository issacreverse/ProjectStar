using UnityEngine;

public class EnemyField : MonoBehaviour
{
    //적의 체력, 상태 판정 같은 필드 정보들을 관장하는 스크립트입니다. 
    //연산에 필요한 데이터는 모두 DataManager에서 직접 받아옵니다. 
    private EnemyController _enemyController;
    private EnemyData enemyData;

    //적 필드 
    private string enemyId;
    private float hitPoints;
    private float hitPointsMax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemyController = gameObject.GetComponent<EnemyController>();
        enemyId = _enemyController.GetEnemyId();

        //EnemyController를 거치지 않고 DataManager로부터 직접 받아온다. 거쳐서 받아오면 호출순서 때문에 Null 레퍼런스 에러 뜬다. 
        enemyData = DataManager.Instance.GetEnemyData(enemyId);
        hitPoints = enemyData.hitPoints;
        hitPointsMax = hitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if(hitPoints <= 0f)
        {
            hitPoints = 0;
            Die();
        }
    }
    //외부에서 호출하여 데미지를 준다.
    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
    }
    //죽을 때 호출됩니다. 
    public void Die()
    {
        WaveManager.Instance.RemoveEnemyFromList(_enemyController);
        Destroy(gameObject);
    }
    //체력값을 반환한다. 
    public float GetHitPoints()
    {
        return hitPoints;
    }
    //체력최대치값을 반환한다. 비율 계산이나 회복 계산할 때 사용한다. 
    public float GetHitPointsMax()
    {
        return hitPointsMax;
    }
}
