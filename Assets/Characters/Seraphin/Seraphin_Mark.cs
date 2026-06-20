using System;
using Unity.VisualScripting;
using UnityEngine;

public class Seraphin_Mark : Bullet
{
    //내부 필드
    private bool isAttached;
    private EnemyField _enemyField;

    void Awake()
    {
        isAttached = false;
    }
    protected override void Update()    
    {
        //부착되었다면 더이상 일반 탄알처럼 행동하지 않는다. 
        if(isAttached)
            return;
        base.Update();
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        //부착되지 않았다면 부착되거나, 일반 탄알처럼 행동한다. 
        if(!isAttached)
        {
                //적과 충돌하면 데미지를 준다. 
            if(other.gameObject.CompareTag("Enemy"))
            {
                //적에게 자식 오브젝트로 부착한다. 
                //이 오브젝트에서 필요없는 콜라이더를 지우고,
                //적 오브젝트에 스크립트를 넣는다. 
                transform.SetParent(other.transform);
                gameObject.GetComponent<CircleCollider2D>().enabled = false;
                other.AddComponent<Seraphin_Mark>().SetIsAttached();
                //데미지를 준다. 
                //속성 적은 추가 데미지를 준다 -> 나중에 추가 예정.
                _enemyField = other.gameObject.GetComponent<EnemyField>();
                _enemyField.TakeDamage(bulletDamage);

                Destroy(this);

                //표식이 붙은 다음에는 적이 파괴되기 전까지 파괴하지 않는다. 
                //Destroy(gameObject);
            }
            //벽에 충돌하면 없어지기만 한다. 
            else if(other.gameObject.CompareTag("BulletBoundary"))
            {
                Destroy(gameObject);
            }
        }
        //부착되었다면 적의 컴포넌트처럼 행동하고, 일반 탄알처럼 행동하지 않는다. 
        if(isAttached)
        {
            print(2);
            if(other.gameObject.CompareTag("Bullet"))
            {
                if(other.GetComponent<Bullet>(). bulletType == ElementType.Light)
                {
                    print(1);
                    //새로 충돌한 탄알 프리팹이 1배수 데미지를 먼저 넣고,
                    //부착된 이 스크립트에서 추가배수 데미지를 마저 넣어준다.
                    if(_enemyField == null)
                        _enemyField = gameObject.GetComponent<EnemyField>();
                    _enemyField.TakeDamage(other.GetComponent<Bullet>().bulletDamage * (GameConstants.SYNC_DAMAGE_MULTIPLIER - 1f));
                }
            }
        }
    }
    public void SetIsAttached()
    {
        isAttached = true;
    }
}
