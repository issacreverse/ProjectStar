using UnityEngine;
using System.Collections;

public class EnemyAttackController : MonoBehaviour
{   
    //적의 공격만을 담당하는 스크립트. EnemyController에 들어있다. 

    //공격에 필요한 속성이 담겨있는 타입 
    protected EnemyBulletPatternData data;
    private float attackTimer;
    //플레이어 객체. 조준탄을 위해 저장해둔다. 
    protected GameObject player;

    //보스몹의 상태머신에서 movePatternData를 넘겨주기 위해서 참조를 저장한다.  (마음에 안 든다) EnemyBossPhaseActionData 클래스의 DoAction()에서 접근해야돼서 public이다...
    public EnemyController enemyController;

    //초기화
    public void Initialize(EnemyBulletPatternData _data)
    {
        data = _data;
        attackTimer = 0f;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.Log("Error: Can't Find Player Object");
            return;
        }
        enemyController = gameObject.GetComponent<EnemyController>();
    }

    protected virtual void Update()
    {
        //공격 주기가 돌면 공격을 한다 
        attackTimer += Time.deltaTime;

        if(data != null)
        {
            if(attackTimer >= data.attackInterval)
            {
                attackTimer = 0f;
                Attack(data);
            }
        }
    }

    //어떤 공격을 할지 결정 
    protected void Attack(EnemyBulletPatternData _data)
    {
        switch(_data.bulletForm)
        {
            case BulletForm.Aim:
                FireAim(_data);
                break;
            case BulletForm.Circle:
                FireCircle(_data);
                break;
            case BulletForm.Fan:
                FireFan(_data);
                break;
            default: 
                break;
        }
    }
    
    //연사(1번 공격 시 나가는 탄알 수가 1개 이상) 구현 코루틴
    //형태마다 다른 값을 제외한 값들은 자동 설정해줌. (탄속, 데미지, 공격당 연사 수, 연사 간격)
    public IEnumerator FireCoroutine(EnemyBulletPatternData _data, Vector2 direction)
    {
        float speed = _data.bulletSpeed;
        int firePerAttack = _data.firePerAttack;
        float damage = _data.bulletDamage;
        float fireInterval = _data.fireInterval;

        for(int i=0; i<firePerAttack; i++)
        {
            yield return new WaitForSeconds(fireInterval);
            SpawnBullet(direction, speed, damage);
        }
    }
    //공격 형태에 따른 공격의 종류들 
    protected void FireAim(EnemyBulletPatternData _data)
    {   
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = player.transform.position;

        Vector2 direction = (targetPosition - startPosition);
        StartCoroutine(FireCoroutine(_data, direction));
    }

    protected void FireCircle(EnemyBulletPatternData _data) //circle 공격형태는 spread=360인 fan 공격형태와 같다...
    {
        int count = _data.bulletsPerFire;
        float spread = 360;
        float startAngle = 0;
        float angleStep = count > 1 ? spread / (count - 1) : 0f;

        for(int i=0; i<count; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = GetDirectionFromAngle(angle);

            StartCoroutine(FireCoroutine(_data, dir));
        }
    }
    
    protected void FireFan(EnemyBulletPatternData _data)
    {
        int count = _data.bulletsPerFire;
        float spread = _data.spreadAngle;
        float startAngle = -spread / 2f;
        float angleStep = count > 1 ? spread / (count - 1) : 0f;

        for(int i=0; i<count; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = GetDirectionFromAngle(angle);

            StartCoroutine(FireCoroutine(_data, dir));
        }
    }

    //각도 계산용 함수
    private Vector2 GetDirectionFromAngle(float angleDegree)
    {
        //-90f: 왼쪽을 보고 발사하도록 하기 위한 보정값. 
        float radian = (angleDegree-90f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(radian), -Mathf.Cos(radian));
    }

    private void SpawnBullet(Vector2 direction, float speed, float damage)
    {
        EnemyBullet bullet = PoolingManager.Instance.objectPool.Get();
        bullet.transform.position = transform.position;
        bullet.Initialize(direction, speed, damage);
    }
    //공격 패턴을 바꿀 일이 있는 보스만 호출.
    public void SetBulletPattern(string bulletPatternId)
    {
        data = DataManager.Instance.GetEnemyBulletPatternData(bulletPatternId);
    }
}
