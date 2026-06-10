using System;
using System.Collections;

 //플레이어 정보를 저장하는 데이터 타입. JSON 파일에서 값들을 가져올 거임.
[Serializable]
public class PlayerData
{
   public string id;                                                    //플레이어 고유 id
   public float hitPoints;                                              //플레이어 체력 
   public float moveSpeed;                                              //플레이어 이동속도
   public float bulletDamage;   //이후 확장시킬 거지만 일단 이렇게 해놓자.//플레이어 공격 탄알 1개당 데미지 
}
