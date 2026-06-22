using System;
using System.Collections;
using UnityEngine;

public class Character_Lumia : PlayerCharacterBase
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
    //protected override float UltimateCoolDown => ultimateCoolDown;

    protected override float DownCoolDownDecreaseRatio => downCoolDownDecreaseRatio;

    
    //캐릭터 필드
    [Header("Character Field")]
    [SerializeField] private string characterId = "Lumia";
    [SerializeField] private float maxHitPoints = 300f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float slowMoveSpeed = 3f;
    [SerializeField] private float reviveHitPointsRatio = 0.25f;
    [SerializeField] private float reviveCoolDown = 25f;
    [SerializeField] private float downCoolDownDecreaseRatio = 0.4f;

    [Header("Base Attack")]
    [SerializeField] private float baseAttackDamage = 5;
    [SerializeField] private float baseAttackRate = 5f;
    [SerializeField] private float baseAttackBulletSpeed = 30f;
    [SerializeField] private GameObject baseAttackPrefab;

    [Header("Sub Attack")]
    [SerializeField] private float subAttackDamage = 30;
    //[SerializeField] private int subAttackCount = 1;
    [SerializeField] private float subAttackBulletSpeed = 14f;
    [SerializeField] private float subAttackCoolDown = 5f;
    [SerializeField] private GameObject subAttackPrefab;

    [Header("Skill")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashInvincibleDuration = 0.4f;
    [SerializeField] private float skillCoolDown = 12f;

    [Header("Ultimate")]
    //[SerializeField] private float ultimateCoolDown = 60f;

    //외부 참조 
    [Header("External References")]
    [SerializeField] private Transform shootingPos;

    //캐릭터 함수 구현부 
    //구현 주의사항: 이동관련, Collider 등은 모두 부모 오브젝트에 붙어있으므로, 
    //PlayerController를 반드시 참조해서 그것을 기준으로 할 것. 
    //예) transform -> playerController.transform
    protected override void BaseAttack()
    {
        //무색 탄환을 전면으로 빠르게 연사. 
        //탄 크기 넓음 -> 탄알 프리팹 반영 필요
        //공격력 5

        ShootBullet(baseAttackPrefab, shootingPos, BulletForm.Normal, ElementType.Plain, baseAttackDamage, baseAttackBulletSpeed);
    }
    protected override void SubAttack()
    {
        //무색 추적 미사일을 발사.
        //가장 앞에 있는 적을 추적
        //이동 경로는 살짝 흔들림
        //공격력 30초
        //쿨타임 5초

        ShootBullet(subAttackPrefab, shootingPos, BulletForm.HomingWiggleMissle, ElementType.Plain, subAttackDamage, subAttackBulletSpeed);
    }
    protected override void Skill()
    {
        //방향키 입력 방향으로 짧은 대쉬
        //대쉬 중 회피 판정 
        //쿨타임 12초
        StartCoroutine(Invincible());
        StartCoroutine(Dash());
    }
    protected override void Ability()
    {
        //없음
    }
    protected override void Ultimate()
    {
        //화면에 보이는 모든 적 탄환 삭제.
        //쿨타임 60초
        PoolingManager.Instance.EraseAllActiveBullets();
    } 

    private IEnumerator Invincible()
    {
        Collider2D collider = gameObject.GetComponentInParent<CircleCollider2D>();
        collider.enabled = false;
        yield return new WaitForSeconds(dashInvincibleDuration);
        collider.enabled = true;
    }
    private IEnumerator Dash()
    {
        PlayerController player = gameObject.GetComponentInParent<PlayerController>();
        Vector2 direction = player.GetInputDirection();

        float elapsed = 0f;
        while(elapsed < dashInvincibleDuration)
        {
            player.transform.Translate(direction * (dashDistance / dashInvincibleDuration) * Time.deltaTime);
            yield return null;
            elapsed += Time.deltaTime;
        }
    }
}
