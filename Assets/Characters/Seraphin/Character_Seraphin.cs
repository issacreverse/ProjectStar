using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Character_Seraphin : PlayerCharacterBase
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
    [SerializeField] private string characterId = "Seraphin";
    [SerializeField] private float maxHitPoints = 210f;
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
    [SerializeField] private float subAttackDamage = 8;
    [SerializeField] private int subAttackCount = 1;
    [SerializeField] private float subAttackBulletSpeed = 40f;
    [SerializeField] private float subAttackCoolDown = 8f;
    [SerializeField] private GameObject subAttackPrefab;

    [Header("Skill")]
    [SerializeField] private float shieldDuration = 3f;
    [SerializeField] private int maxShieldHits = 5;
    [SerializeField] private float skillCoolDown = 16f;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private Transform shieldPos;

    [Header("Ultimate")]
    [SerializeField] private float ultimateCoolDown = 60f;

    //외부 참조 
    [Header("External References")]
    [SerializeField] private Transform shootingPos;

    //캐릭터 함수 구현부 
    //구현 주의사항: 이동관련, Collider 등은 모두 부모 오브젝트에 붙어있으므로, 
    //PlayerController를 반드시 참조해서 그것을 기준으로 할 것. 
    //예) transform -> playerController.transform
    protected override void BaseAttack()
    {
        //빛 속성 탄환 전면으로 발사. 연사 빠름. 
        //탄 크기 보통
        //공격력 3

        ShootBullet(baseAttackPrefab, shootingPos.position, BulletForm.Normal, ElementType.Light, baseAttackDamage, baseAttackBulletSpeed);
    }
    protected override void SubAttack()
    {
        //빛의 표식 탄환 1발 발사. 
        //맞은 적은 5초 동안 빛 속성 피해를 20% 더 받음.
        //공격력 8
        //쿨타임 8초

        ShootBullet(subAttackPrefab, shootingPos.position, BulletForm.Normal, ElementType.Light, subAttackDamage, subAttackBulletSpeed);
    }
    protected override void Skill()
    {
        //전방의 빛의 방패 생성. 날라오는 탄알 방어. 
        //5개 방어하고 파괴.
        //지속 시간 3초
        //쿨타임 16초
        StartCoroutine(SpawnShield());
    }
    protected override void Ability()
    {
        //없음
    }
    protected override void Ultimate()
    {
        //화면에 보이는 모든 적 탄환 삭제.
        //쿨타임 60초
    } 

    private IEnumerator SpawnShield()
    {
        //실드 생성
        GameObject shield = Instantiate(shieldPrefab, shieldPos.position, Quaternion.identity);
        shield.transform.SetParent(transform);
        shield.GetComponent<Seraphin_Shield>().Init(maxShieldHits);
        yield return new WaitForSeconds(shieldDuration);
        //제한시간이 다 되면 실드 파괴 
        if(shield != null)
            Destroy(shield);
    }
}
