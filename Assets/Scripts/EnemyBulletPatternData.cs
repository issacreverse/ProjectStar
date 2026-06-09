using System;
using System.Collections;

//적의 탄알 패턴에 대한 정보를 담고 있는 타입. Json 형태로 저장하고 게임에서 불러온다. 
public enum BulletForm
{
    Aim = 0,
    Circle = 1,
    Fan = 2
}
[Serializable]
public class EnemyBulletPatternData
{
    public string id;
    public BulletForm bulletForm;
    public float bulletSpeed;
    public float bulletDamage;
    public int firePerAttack;
    public int bulletsPerFire;
    public float spreadAngle;
    public float attackInterval;
    public float fireInterval;
}
