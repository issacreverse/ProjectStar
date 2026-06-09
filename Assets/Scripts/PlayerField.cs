using UnityEngine;

public class PlayerField : MonoBehaviour
{
    [SerializeField] private float hitPoints = 100f;

    void Update()
    {
        if(hitPoints <= 0f)
        {
            hitPoints = 0;
            Die();
        }
    }

    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}
