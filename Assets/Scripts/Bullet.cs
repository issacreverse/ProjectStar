using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{   
    //탄알 속성 
    [SerializeField] private float speed = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.x > 80)
        {
            PoolingManager.Instance.objectPool.Release(this);
        }
            

        this.transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
