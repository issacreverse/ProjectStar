using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //플레이어의 이동과 공격을 제어하는 스크립트 
    //Player 레이어의 Physics2D Collision Matrix: EnemyBullet 하고만 충돌 
    //플레이어의 체력, 상태 판정 같은 필드 정보들은 같은 오브젝트 아래에 있는 PlayerField 스크립트에서 제어합니다. -> 이제 PlayerField 안 쓴다.
    //다른 스크립트들은 이 스크립트에서 GetPlayerId 함수를 통해 Id를 가져오되 Data는 DataManager에서 직접 가져옵니다. (호출 순서 문제 때문) -> 이제 안 한다.
    //받아온 플레이어 정보
    //private PlayerData playerData;


    //내부 필드
    private PlayerCharacterBase currentPlayerCharacter;
    private float moveSpeed;
    private float slowMoveSpeed;  

    private float ultimateTimer;        //궁극기는 쿨타임 공유이므로 PlayerController에서 제어한다.    
    
    //Input System Package 
    private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction slowMoveAction;
    private InputAction baseAttackAction;
    private InputAction subAttackAction;
    private InputAction skillAction;
    private InputAction ultimateAction;
    private InputAction switchAction1;
    private InputAction switchAction2;
    private InputAction switchAction3;

    private bool isActionsReady;    //내부 플래그. 필요한 이유는 Awake에 있던 걸 Start로 옮겼기 때문에 OnEnable에서 호출하는 것과의 문제가 생겼기 때문. (+GameManager 싱글톤)
    

    //플레이어 이동 제한 가로 세로 폭
    public float maxX = 9f;
    public float minX = -9f;
    public float maxY = 5f;
    public float minY = -5f;

    void Awake()
    {
        ultimateTimer = 0f;
        isActionsReady = false;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        //InputSystem 연결하기
        inputActions = GameManager.Instance.GetInputActionAsset();

        if(inputActions == null)
            return;
            
        moveAction = inputActions.FindAction("Move", true);
        slowMoveAction = inputActions.FindAction("SlowMove", true);
        baseAttackAction = inputActions.FindAction("BaseAttack", true);   
        subAttackAction = inputActions.FindAction("SubAttack", true); 
        skillAction = inputActions.FindAction("Skill", true); 
        ultimateAction = inputActions.FindAction("Ultimate", true); 
        switchAction1 = inputActions.FindAction("Switch1", true); 
        switchAction2 = inputActions.FindAction("Switch2", true); 
        switchAction3 = inputActions.FindAction("Switch3", true); 

        isActionsReady = true;

        EnableActions();
    }
    private void OnEnable()
    {
        if(!isActionsReady)
            return;
        EnableActions();
    }

    private void OnDisable()
    {
        DisableActions();
    }

    // Update is called once per frame
    void Update()
    {
        //플레이어 입력 처리 함수
        //이동, 공격, 스킬, 궁극기, 스위치 등의 입력을 처리한다.

        //이동
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        bool isSlowMove = slowMoveAction.IsPressed();

        float currentMoveSpeed = moveSpeed;
        if(isSlowMove)
            currentMoveSpeed= slowMoveSpeed;

        transform.Translate(moveValue.normalized * currentMoveSpeed * Time.deltaTime);

        //플레이어가 화면 밖으로 나가지 못하도록 위치를 강제조정한다. 
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;

        
        //플레이어 키 
        //기본 공격이 자동으로 나가는 게 디폴트지만 
        //나중에 다른 방식의 캐릭터가 나올 수도 있음. 
        /*
        if(baseAttackAction.IsPressed())
        {
            currentPlayerCharacter.TryBaseAttack();
        }
        */
        currentPlayerCharacter.TryBaseAttack();

        if(subAttackAction.IsPressed())
        {
            currentPlayerCharacter.TrySubAttack();
        }

        if(skillAction.IsPressed())
        {
            currentPlayerCharacter.TrySkill();
        }

        ultimateTimer -= Time.deltaTime;        //궁극기 쿨타임 감소 계산 
        if(ultimateAction.IsPressed())
        {
            currentPlayerCharacter.TryUltimate(ultimateTimer, out bool resetTimer);
            if(resetTimer)
            {
                ultimateTimer = GameConstants.ULTIMATE_COOLDOWN;
            }
        }

        if(switchAction1.IsPressed())
        {
            //첫번째 캐릭터를 사용 중이 아니라면
            //PlayerParty에서 첫번째 캐릭터를 가져온다. 
            SwitchPlayerCharacter(1);
        }

        if(switchAction2.IsPressed())
        {
            //두번째 캐릭터를 사용 중이 아니라면
            //PlayerParty에서 두번째 캐릭터를 가져온다. 
            SwitchPlayerCharacter(2);
        }

        if(switchAction3.IsPressed())
        {
            //세번째 캐릭터를 사용 중이 아니라면
            //PlayerParty에서 세번째 캐릭터를 가져온다. 
            SwitchPlayerCharacter(3);
        }
    }
    private void EnableActions()
    {
        moveAction.Enable();
        slowMoveAction.Enable();
        baseAttackAction.Enable();
        subAttackAction.Enable();
        skillAction.Enable();
        ultimateAction.Enable();
        switchAction1.Enable();
        switchAction2.Enable();
        switchAction3.Enable();
    }
    private void DisableActions()
    {
        moveAction.Disable();
        slowMoveAction.Disable();
        baseAttackAction.Disable();
        subAttackAction.Disable();
        skillAction.Disable();
        ultimateAction.Disable();
        switchAction1.Disable();
        switchAction2.Disable();
        switchAction3.Disable();
    }

    //외부 공개 함수
    public void SwitchPlayerCharacter(int characterNum)
    {
        PlayerPartyManager.Instance.SwitchCharacter(characterNum);
        //초기화 다시 진행
        GameObject currentPlayerObject = PlayerPartyManager.Instance.GetCurrentPlayerCharacter();
        currentPlayerCharacter = currentPlayerObject.GetComponent<PlayerCharacterBase>();

        moveSpeed = currentPlayerCharacter.MoveSpeed;
        slowMoveSpeed = currentPlayerCharacter.SlowMoveSpeed;
    }
    public Vector2 GetInputDirection()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        return moveValue.normalized;
    }
}
