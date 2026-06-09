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
    [SerializeField] private GameObject bullet;

    //Input System Package 
    InputAction moveAction;
    InputAction attackAction;

    //플레이어 이동 제한 가로 세로 폭
    public float maxX = 9f;
    public float minX = -9f;
    public float maxY = 5f;
    public float minY = -5f;

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

        //플레이어가 화면 밖으로 나가지 못하도록 위치를 강제조정한다. 
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;

        
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
                GameObject bulletObject = Instantiate(bullet);
                bulletObject.transform.position = this.transform.position;
            }
        } 
    }
}
