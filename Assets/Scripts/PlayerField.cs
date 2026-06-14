using UnityEngine;

public class PlayerField : MonoBehaviour
{
    //플레이어의 필드 값을 관리하고 상태 제어를 하는 스크립트입니다. 
    //연산에 필요한 데이터는 모두 DataManager에서 직접 받아옵니다. 
    private PlayerController _playerController;
    private PlayerData playerData;
    
    //플레이어 필드
    private string playerId;
    private float hitPoints;
    private float hitPointsMax;

    void Start()
    {
        _playerController = gameObject.GetComponent<PlayerController>();
        playerId = _playerController.GetPlayerId();

        //PlayerController를 거치지 않고 DataManager로부터 직접 받아온다. 거쳐서 받아올 경우 호출 순서 때문에 Null 레퍼런스 에러 뜬다. 
        playerData = DataManager.Instance.GetPlayerData(playerId);
        hitPoints = playerData.hitPoints;
        hitPointsMax = hitPoints;
    }
    void Update()
    {
        if(hitPoints <= 0f)
        {
            hitPoints = 0;
            Die();
        }
    }

    //외부에서 호출합니다. 데미지를 준다.
    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        Debug.Log($"HIT! {damage} damage");
    }
    //죽을 때 호출됩니다. 
    public void Die()
    {
        Destroy(gameObject);
    }
}
