using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
    //플레이어 속성
    //이동 관련 필드
    public float moveSpeed = 5.0f; 
    //공격 관련 필드
    public float attackPerSec = 10f;
    private float attackTimer;  

    //Input System Package 
    InputAction moveAction;
    InputAction attackAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");    
        attackTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //이동
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        transform.Translate(moveValue * moveSpeed * Time.deltaTime);
        
        //공격 
        float attackValue = attackAction.ReadValue<float>(); 
        if(attackValue != 0)
        {
            attackTimer += Time.deltaTime;
            float attackInterval = 1f / attackPerSec;

            //공격 주기가 돌았다
            if(attackTimer >= attackInterval)   
            {
                attackTimer = 0f;
                //Debug.Log("Attack!");
                if(PoolingManager.Instance == null)
                {
                    Debug.Log("PoolingManager is null");
                }
                else if(PoolingManager.Instance.objectPool == null)
                {
                    Debug.Log("objectPool is null");
                }
                Bullet bulletObject = PoolingManager.Instance.objectPool.Get();
                bulletObject.transform.position = this.transform.position;
            }
        } 
    }
}
