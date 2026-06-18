using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{   
    //(플레이어가 쏘는) 탄알 스크립트
    //Physics2D Collision Matrix: BulletBoundary, Enemy 하고만 충돌 
    
    //고정 값
    //탄알 종류가 많지 않을 뿐더러 같은 탄알 방식은 값도 같을 거라고 판단해서 고정 값을 사용하기로 했습니다. 
    private const float HOMING_ROTATE_SPEED = 240f;
    private const float RETARGET_INTERVAL = 0.2f;
    private const float WIGGLE_FREQUENCY = 8f;
    private const float WIGGLE_AMOUNT = 0.4f;

    //내부 필드

    //공통 필드
    private BulletForm bulletForm;
    private ElementType bulletType;
    private float bulletSpeed;
    private float bulletDamage;
    private Vector3 moveDirection = Vector3.right;

    //HomingWiggleMissile 필드
    private Transform target;
    private float retargetTimer;
    
    public void Initialize(BulletForm bulletForm, ElementType bulletType, float bulletDamage, float bulletSpeed)
    {
        this.bulletForm = bulletForm;
        this.bulletType = bulletType;
        this.bulletDamage = bulletDamage;
        this.bulletSpeed = bulletSpeed;

        if(bulletForm == BulletForm.HomingWiggleMissle)
        {
            retargetTimer = 0f;
            target = FindNearestEnemy();
            if(target != null)
            {
                moveDirection = (target.position - transform.position).normalized;
            }
            else
            {
                moveDirection = Vector3.right;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        switch(bulletForm)
        {
            case BulletForm.Normal:
                MoveNormal();
                break;
            case BulletForm.HomingWiggleMissle:
                MoveHomingWiggleMissile();
                break;
        }
        
    }
    //충돌 판정 
    void OnTriggerEnter2D(Collider2D other)
    {
        //적과 충돌하면 데미지를 준다. 
        if(other.gameObject.CompareTag("Enemy"))
        {
            EnemyField _enemyField = other.gameObject.GetComponent<EnemyField>();
            //데미지를 준다. 
            //속성 적은 추가 데미지를 준다 -> 나중에 추가 예정.
            _enemyField.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
        //벽에 충돌하면 없어지기만 한다. 
        else if(other.gameObject.CompareTag("BulletBoundary"))
        {
            Destroy(gameObject);
        }
    }
    //Normal 함수
    private void MoveNormal()
    {
        transform.Translate(moveDirection * bulletSpeed * Time.deltaTime);
    }

    //HomingWiggleMissile 함수
    private Transform FindNearestEnemy()
    {   
        List<EnemyController> enemyList = WaveManager.Instance.GetActiveEnemyList();

        Transform nearest = null;
        float nearestDistanceSqr = float.MaxValue;

        foreach (EnemyController enemy in enemyList)
        {
            if (enemy == null)
                continue;

            float distanceSqr =
                (enemy.transform.position - transform.position).sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }
    private void MoveHomingWiggleMissile()
    {
            retargetTimer -= Time.deltaTime;

        if (retargetTimer <= 0f)
        {
            target = FindNearestEnemy();
            retargetTimer = RETARGET_INTERVAL;
        }

        if (target != null)
        {
            Vector3 targetDirection =
                (target.position - transform.position).normalized;

            Vector3 sideDirection =
                Vector3.Cross(Vector3.forward, targetDirection).normalized;

            Vector3 wiggle =
                sideDirection *
                Mathf.Sin(Time.time * WIGGLE_FREQUENCY) *
                WIGGLE_AMOUNT;

            Vector3 desiredDirection =
                (targetDirection + wiggle).normalized;

            moveDirection = Vector3.RotateTowards(
                moveDirection,
                desiredDirection,
                HOMING_ROTATE_SPEED * Mathf.Deg2Rad * Time.deltaTime,
                0f
            );
        }

        transform.position +=
            moveDirection * bulletSpeed * Time.deltaTime;

        RotateToMoveDirection();
    }
    private void RotateToMoveDirection()
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation =  Quaternion.Euler(0f, 0f, angle);
    }
}

public enum BulletForm
{
    Normal,
    HomingMissile,
    HomingWiggleMissle
}
