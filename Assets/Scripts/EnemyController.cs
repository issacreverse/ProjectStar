using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private float moveDir = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
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
