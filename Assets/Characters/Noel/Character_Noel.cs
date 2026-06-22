using System;
using System.Collections;
using UnityEngine;

public class Character_Noel : PlayerCharacterBase
{
    //프로퍼티 구현부 
    public override string CharacterId => characterId;

    protected override float MaxHitPoints => maxHitPoints; 
    public override float MoveSpeed => moveSpeed;
    public override float SlowMoveSpeed => slowMoveSpeed;

    protected override float ReviveHitPointsRatio => reviveHitPointsRatio;
    protected override float ReviveCoolDown => reviveCoolDown;

    protected override float BaseAttackCoolDown => 1f / baseAttackRate;
    protected override float SubAttackCoolDown => subAttackCoolDown;
    protected override float SkillCoolDown => skillCoolDown;
    protected override float UltimateCoolDown => ultimateCoolDown;

    protected override float DownCoolDownDecreaseRatio => downCoolDownDecreaseRatio;

    
    //캐릭터 필드
    [Header("Character Field")]
    [SerializeField] private string characterId = "Noel";
    [SerializeField] private float maxHitPoints = 250f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float slowMoveSpeed = 3f;
    [SerializeField] private float reviveHitPointsRatio = 0.25f;
    [SerializeField] private float reviveCoolDown = 25f;
    [SerializeField] private float downCoolDownDecreaseRatio = 0.4f;

    [Header("Base Attack")]
    [SerializeField] private float baseAttackDamage = 3;
    [SerializeField] private float baseAttackRate = 5f;
    [SerializeField] private float baseAttackBulletSpeed = 30f;
    [SerializeField] private GameObject baseAttackPrefab;

    [Header("Sub Attack")]
    [SerializeField] private float subAttackCoolDown = 10f;
    [SerializeField] private float LifeDrainDuration = 1.5f; 

    [Header("Skill")]
    [SerializeField] private float healAmount = 50f;
    [SerializeField] private int healDuration = 10;
    [SerializeField] private float skillCoolDown = 40f;

    [Header("Ultimate")]
    [SerializeField] private float ultimateCoolDown = 60f;
    [SerializeField] private float ultimateHealAmount = 150f;

    //캐릭터 교체시 호출되는 이벤트에 등록하는 델리게이트. 람다함수를 저장해두고, 람다함수가 원하는 함수를 호출하도록 한다. 
    //인자 때문에 이렇게 한다. 
    private Action[] HandleSwitchCharacter;
    private bool isUltimateOn = false;
    

    //외부 참조 
    [Header("External References")]
    [SerializeField] private Transform shootingPosFront;
    [SerializeField] private Transform shootingPosDiagUp;
    [SerializeField] private Transform shootingPosDiagDown;


    //내부 함수
    protected override void Awake()
    {
        base.Awake();
        isUltimateOn = false;
        HandleSwitchCharacter = new Action[GameConstants.MAX_PARTY_CHARACTERS];
    }
    private void RecalculateBuffMultipliers()
    {
        //주변 환경에 따른 캐릭터 버프값 계산 함수. 
        //스테이지 진입 시, 그리고 스테이지 환경 변화 시 호출합니다. 

    }
    //캐릭터 함수 구현부 
    //구현 주의사항: 이동관련, Collider 등은 모두 부모 오브젝트에 붙어있으므로, 
    //PlayerController를 반드시 참조해서 그것을 기준으로 할 것. 
    //예) transform -> playerController.transform
    protected override void BaseAttack()
    {
        //물방울 탄환 3갈래 발사. 위 30도, 아래 30도, 전면 발사. 연사 빠름. 
        //탄 크기 보통
        //공격력 3

        ShootBullet(baseAttackPrefab, shootingPosFront, BulletForm.Normal, ElementType.Water, baseAttackDamage, baseAttackBulletSpeed);
        ShootBullet(baseAttackPrefab, shootingPosDiagUp, BulletForm.Normal, ElementType.Water, baseAttackDamage, baseAttackBulletSpeed);
        ShootBullet(baseAttackPrefab, shootingPosDiagDown, BulletForm.Normal, ElementType.Water, baseAttackDamage, baseAttackBulletSpeed);
    }
    protected override void SubAttack()
    {
        //1.5초 동안 흡혈 상태, 자신이 준 피해의 30%만큼 체력 회복. 쿨타임 10초.
        StartCoroutine(LifeDrainCoroutine());
    }
    private IEnumerator LifeDrainCoroutine()
    {
        WaveManager.Instance.OnEnemyDamagedHub += Heal;
        yield return new WaitForSeconds(LifeDrainDuration);
        WaveManager.Instance.OnEnemyDamagedHub -= Heal;
    }
    protected override void Skill()
    {
        //10초에 걸쳐 출전 캐릭터 3명 모두 체력을 총 50 회복. 
        //쿨타임 40초
        StartCoroutine(PartyHealCoroutine());
    }
    private IEnumerator PartyHealCoroutine()
    {
        for(int i=0; i<healDuration; i++)
        {
            PlayerPartyManager.Instance.PartyHeal(healAmount/healDuration);
            yield return new WaitForSeconds(1f);
        }
    }
    protected override void Ability()
    {
        //없음
    }
    protected override void Ultimate()
    {
        //다음에 교체되는 캐릭터의 체력을 150만큼 회복시킨다. 


        if(isUltimateOn)
            return;
        
        isUltimateOn = true;

        GameObject[] party = PlayerPartyManager.Instance.GetParty();
        int idx = 0;
        //각자 본인을 힐하는 람다 함수를 만든다.  
        foreach(GameObject character in party)
        {
            HandleSwitchCharacter[idx++] = () => {
                character.GetComponent<PlayerCharacterBase>().Heal(ultimateHealAmount);
                EraseUltimateDelegates();
                isUltimateOn = false;
            };
        }
        
        //람다함수를 본인의 switch이벤트에 등록
        for(int i=0; i<GameConstants.MAX_PARTY_CHARACTERS; i++)
        {
            PlayerPartyManager.Instance.Subscribe(i, HandleSwitchCharacter[i]);
        }
    } 
    private void EraseUltimateDelegates()
    {
        //등록했던 람다함수 제거.
        for(int i=0; i<GameConstants.MAX_PARTY_CHARACTERS; i++)
        {
            PlayerPartyManager.Instance.UnSubscribe(i, HandleSwitchCharacter[i]);
        }
    }
}
