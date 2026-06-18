using System;
using System.Collections;

//적의 탄알 패턴에 대한 정보를 담고 있는 타입. Json 형태로 저장하고 게임에서 불러온다. 
public enum EnemyBulletForm
{
    Aim = 0,                                //조준형. 초탄발사 시 플레이어 위치로 전탄 발사
    Circle = 1,                             //원형
    Fan = 2                                 //부채꼴형
}
[Serializable]
public class EnemyBulletPatternData
{
    public string id = "none";              //탄알 패턴 고유 id. 

    public EnemyBulletForm bulletForm = 0;       //공격 형태

    public float bulletSpeed = 0;           //탄속
    public float bulletDamage = 0;          //탄알 1개당 데미지

    public int firePerAttack = 0;           //1번 공격 시 몇 번 발사하는지
    public int bulletsPerFire = 0;          //1번 발사 시 탄알이 총 몇 개가 나가는지

    public float spreadAngle = 0;           //탄이 퍼지는 각도. 특정 공격 형태에서 필요

    public float attackInterval = 0;        //공격 주기
    public float fireInterval = 0;          //1번 공격이 연사하는 방식일 때 연사탄간의 딜레이 시간
}
