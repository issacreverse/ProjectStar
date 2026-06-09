using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{   
    //탄알 속성 
    [SerializeField] private float speed = 5.0f;
   
    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit!");
            Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag("BulletBoundary"))
        {
            Destroy(gameObject);
        }
    }
}
