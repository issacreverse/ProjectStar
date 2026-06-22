using System.Collections;
using UnityEngine;
using System;

public abstract class PlayerCharacterBase: MonoBehaviour
{
    //구현 요구 프로퍼티 
    public abstract string CharacterId {get;}

    protected abstract float MaxHitPoints {get;}
    public abstract float MoveSpeed {get;}
    public abstract float SlowMoveSpeed {get;}

    protected abstract float ReviveHitPointsRatio {get;}
    protected abstract float ReviveCoolDown {get;}

    protected abstract float BaseAttackCoolDown {get;}
    protected abstract float SubAttackCoolDown {get;}
    protected abstract float SkillCoolDown {get;}
    protected abstract float UltimateCoolDown {get;}

    protected abstract float DownCoolDownDecreaseRatio {get;}

    //구현 요구 함수
    protected abstract void BaseAttack();
    protected abstract void SubAttack();
    protected abstract void Skill();
    protected abstract void Ultimate();
    protected abstract void Ability();

    //내부 필드
    private float currentHitPoints;
    private bool isCharacterDown;

    private float baseAttackTimer;
    private float subAttackTimer;
    private float skillTimer;
    private float ultimateTimer;

    //상속 함수
    protected virtual void Awake()
    {
        currentHitPoints = MaxHitPoints;
        isCharacterDown = false;
        baseAttackTimer = 0f;
        subAttackTimer = 0f;
        skillTimer = 0f;
        ultimateTimer = 0f;
    }
    protected virtual void Update()
    {
        //쿨타임 타이머를 돌게합니다.
        //캐릭터가 다운 상태일 경우 쿨타임 타이머가 느리게 돕니다. 
        if(isCharacterDown)
        {
            baseAttackTimer = 0.01f; //다운 상태에서는 기본 공격 쿨타임이 의미 없지만 0으로 할 경우 무한 공격이 가능해질 수 있으므로 아주 작은 값으로 설정함. 
            subAttackTimer -= Time.deltaTime * DownCoolDownDecreaseRatio;
            skillTimer -= Time.deltaTime * DownCoolDownDecreaseRatio;
            ultimateTimer -= Time.deltaTime * DownCoolDownDecreaseRatio;
        }
        else
        {
            baseAttackTimer -= Time.deltaTime;
            subAttackTimer -= Time.deltaTime;
            skillTimer -= Time.deltaTime;
            ultimateTimer -= Time.deltaTime;
        }
        

        //캐릭터의 체력이 0이 될 경우 
        //캐릭터를 다운 시킵니다. 
        if(currentHitPoints <= 0)
        {
            currentHitPoints = 0;
            CharacterDown();
        }
    } 
    protected void ShootBullet(GameObject bulletPrefab, Transform spawnPos, BulletForm bulletForm, ElementType bulletType, float bulletDamage, float bulletSpeed)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPos.position, spawnPos.rotation);
        bullet.GetComponent<Bullet>().Initialize(bulletForm, bulletType, bulletDamage, bulletSpeed);
    }
    protected void ShootBullet_Bezier(GameObject bulletPrefab, Transform spawnPos, ElementType bulletType, float bulletDamage, float bulletSpeed, Vector3 controlPointOffset, Vector3 endPointOffset)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPos.position, spawnPos.rotation);
        //베지에 곡선의 제어점과 끝점을 설정
        Vector3 controlPoint = spawnPos.position + controlPointOffset;
        Vector3 endPoint = spawnPos.position + endPointOffset;

        bullet.GetComponent<Bullet>().SetBezierPoints(spawnPos.position, controlPoint, endPoint);
        bullet.GetComponent<Bullet>().Initialize(BulletForm.Bezier, bulletType, bulletDamage, bulletSpeed);
    }
    protected virtual void HandleStageEnvironmentChange()
    {
        //스테이지 환경 변수가 바뀌었을 때 호출되는 콜백 함수. 
        //나중에 이벤트 등록해두는 거 잊지 않기. 
        //각 캐릭터 함수는 이 함수를 오버라이딩하고 본인의 함수를 실행한다. (예: 버프 계산 함수)
    }
    public virtual void OnSwitchCharacter()
    {
        //switch했을 때 실행할 함수 각자 구현 
    }
    //내부 함수
    private void CharacterDown()
    {
        //캐릭터가 다운 되었을 때 호출합니다. 
        //부활 쿨타임을 시작합니다. 
        //자동 교체가 일어나야하고 -> 나중에 추가
        //교체할 캐릭터가 없으면 패배 처리 -> 나중에 추가 


        isCharacterDown = true;
        StartCoroutine(CharacterRevive());
    }
    private IEnumerator CharacterRevive()
    {
        //부활 쿨타임을 계산하고 
        //쿨타임이 다 되면 일정 비율의 체력으로 부활합니다. 


        yield return new WaitForSeconds(ReviveCoolDown);
        isCharacterDown = false;  
        currentHitPoints = MaxHitPoints * ReviveHitPointsRatio;
    }

    //공개 함수 
    public void TakeDamage(float damage)
    {
        currentHitPoints -= damage;
        //피격 무적 시간
    }
    public void Heal(float amount)
    {
        currentHitPoints += amount;
        if(currentHitPoints >= MaxHitPoints)
            currentHitPoints = MaxHitPoints;
    }
    public void TryBaseAttack()
    {
        if(baseAttackTimer > 0f)
            return;
        BaseAttack();
        baseAttackTimer = BaseAttackCoolDown;
    }
    public void TrySubAttack()
    {
        if(subAttackTimer > 0f)
            return;
        SubAttack();
        subAttackTimer = SubAttackCoolDown;
    }
    public void TrySkill()
    {
        if(skillTimer > 0f)
            return;
        Skill();
        skillTimer = SkillCoolDown;
    }
    public void TryUltimate()
    {
        if(ultimateTimer > 0f)
            return;
        Ultimate();
        ultimateTimer = UltimateCoolDown;
    }
    public void TryAbility()
    {
        Ability();
    }
}



//캐릭터 속성 대항 타입
public enum ElementType
{
    Plain,
    Light,
    Dark,
    Water,
    Star   
}
