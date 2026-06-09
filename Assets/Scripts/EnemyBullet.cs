using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    //탄알 속성 필드 
    private Vector2 direction;
    private float speed;

    //탄알 속성 초기화 
    public void Initialize(Vector2 _direction, float _speed)
    {
        direction = _direction;
        speed = _speed;
    }
    
    void Update()
    {
        //정해진 속성 값대로 계속해서 이동함 
        transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);
    }
}
