using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
    //플레이어의 이동과 공격을 제어하는 스크립트 
    //Player 레이어의 Physics2D Collision Matrix: EnemyBullet 하고만 충돌 
    //플레이어의 체력, 상태 판정 같은 필드 정보들은 같은 오브젝트 아래에 있는 PlayerField 스크립트에서 제어합니다.
    //Id 정보는 이 스크립트에 저장되어있으며: 추후 스폰 시스템이 제어하겠지만
    //다른 스크립트들은 이 스크립트에서 GetPlayerId 함수를 통해 Id를 가져오되 Data는 DataManager에서 직접 가져옵니다. (호출 순서 문제 때문)
    
    //플레이어 정보
    [SerializeField] private string playerId = "Player1";
    //이동 관련 필드
    public float moveSpeed = 5.0f; 
    //공격 관련 필드
    public float attackPerSec = 10f;
    private float attackTimer;  
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePos;

    //받아온 플레이어 정보
    private PlayerData playerData;

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
        //플레이어 정보 받아오기 
        playerData = DataManager.Instance.GetPlayerData(playerId);

        //InputSystem 연결하기
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
                bulletObject.GetComponent<Bullet>().Initialize(playerData.bulletDamage); //비용이 많아보인다면 맞음. 어차피 로직 enemy처럼 바꿀거라 괜춘.
                bulletObject.transform.position = firePos.position;
            }
        } 
    }
    public string GetPlayerId()
    {
        return playerId;
    }
}
