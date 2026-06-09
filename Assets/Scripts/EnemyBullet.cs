using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    //탄알 속성 필드 
    private Vector2 direction;
    private float speed;
    private float damage;

    //탄알 속성 초기화 
    public void Initialize(Vector2 _direction, float _speed, float _damage)
    {
        direction = _direction;
        speed = _speed;
        damage = _damage;
    }
    
    void Update()
    {
        //정해진 속성 값대로 계속해서 이동함 
        transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            PlayerField _playerField = other.gameObject.GetComponent<PlayerField>();
            _playerField.TakeDamage(damage);

            PoolingManager.Instance.objectPool.Release(this);
        }
        else if(other.gameObject.CompareTag("BulletBoundary"))
        {
            PoolingManager.Instance.objectPool.Release(this);
        }
    }
}
