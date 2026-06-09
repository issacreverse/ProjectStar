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
        if(this.transform.position.x > 80)
        {
            Destroy(this);
        }
        this.transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
