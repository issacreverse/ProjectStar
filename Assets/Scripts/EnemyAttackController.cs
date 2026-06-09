//using UnityEditor.ShortcutManagement;
using UnityEngine;
using System.Collections;

public class EnemyAttackController : MonoBehaviour
{   
    //공격에 필요한 속성이 담겨있는 타입 
    private EnemyBulletPatternData data;
    private float attackTimer;
    //플레이어 객체. 조준탄을 위해 저장해둔다. 
    private GameObject player;

    //초기화
    public void Initialize(EnemyBulletPatternData _data)
    {
        Debug.Log("00");
        data = _data;
        attackTimer = 0f;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.Log("Error: Can't Find Player Object");
            return;
        }
    }

    void Update()
    {
        //공격 주기가 돌면 공격을 한다 
        attackTimer += Time.deltaTime;
        if(attackTimer >= data.attackInterval)
        {
            Debug.Log("attack!");
            attackTimer = 0f;
            Attack();
        }
    }

    //어떤 공격을 할지 결정 
    private void Attack()
    {
        Debug.Log("1");
        Debug.Log(data.bulletForm);
        switch(data.bulletForm)
        {
            case BulletForm.Aim:
                Debug.Log("2");
                FireAim();
                break;
            case BulletForm.Circle:
                Debug.Log("3");
                FireCircle();
                break;
            case BulletForm.Fan:
                Debug.Log("4");
                FireFan();
                break;
            default: 
                Debug.Log("5");
                break;
        }
    }
    
    //공격 형태에 따른 공격의 종류들 
    private void FireAim()
    {   
        float fireInterval = data.fireInterval;
        StartCoroutine(FireAimCoroutine(fireInterval));
    }
    public IEnumerator FireAimCoroutine(float fireInterval)
    {
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = player.transform.position;

        Vector2 direction = (targetPosition - startPosition);

        float speed = data.bulletSpeed;
        int count = data.bulletsPerFire;

        for(int i=0; i<count; i++)
        {
            yield return new WaitForSeconds(fireInterval);
            SpawnBullet(direction, speed);
        }
    }
    private void FireCircle()
    {
        
    }
    private void FireFan()
    {
        Debug.Log("Fire Fan Form Bullets");
        int count = data.bulletsPerFire;
        float spread = data.spreadAngle;
        float speed = data.bulletSpeed;

        float startAngle = -spread / 2f;
        float angleStep = count > 1 ? spread / (count - 1) : 0f;

        for (int i=0; i<count; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = GetDirectionFromAngle(angle);

            SpawnBullet(dir, speed);
        }
    }

    private Vector2 GetDirectionFromAngle(float angleDegree)
    {
        float radian = (angleDegree-90f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(radian), -Mathf.Cos(radian));
    }

    private void SpawnBullet(Vector2 direction, float speed)
    {
        EnemyBullet bullet = PoolingManager.Instance.objectPool.Get();
        bullet.transform.position = transform.position;
        bullet.Initialize(direction, speed);
    }
}
