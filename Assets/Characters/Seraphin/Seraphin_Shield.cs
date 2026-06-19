using UnityEngine;

public class Seraphin_Shield : MonoBehaviour
{
    //내부 필드
    private int shieldHits;
    private int maxShieldHits;

    public void Init(int maxShieldHits)
    {
        shieldHits = 0;
        this.maxShieldHits = maxShieldHits;
    }

    // Update is called once per frame
    void Update()
    {
        if(shieldHits >= maxShieldHits)
        {
            shieldHits = 0;
            Destroy(gameObject);
        }
    }
    private void GetHit()
    {
        shieldHits++;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("EnemyBullet"))
        {
            other.gameObject.GetComponent<EnemyBullet>().Release();
            GetHit();
        }
    }
}
