using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{   
    //(플레이어가 쏘는) 탄알 스크립트
    //Physics2D Collision Matrix: BulletBoundary, Enemy 하고만 충돌 

    //탄알 속성 
    [SerializeField] private float speed = 5.0f;
    private float damage;

    public void Initialize(float bulletDamage)
    {
        damage = bulletDamage;
    }
    
    void Start()
    {
        //damage = DataManager.Instance.
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
    //충돌 판정 
    void OnTriggerEnter2D(Collider2D other)
    {
        //적과 충돌하면 데미지를 준다. 
        if(other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit!");
            EnemyField _enemyField = other.gameObject.GetComponent<EnemyField>();
            _enemyField.TakeDamage(damage);
            Destroy(gameObject);
        }
        //벽에 충돌하면 없어지기만 한다. 
        else if(other.gameObject.CompareTag("BulletBoundary"))
        {
            Destroy(gameObject);
        }
    }
}
