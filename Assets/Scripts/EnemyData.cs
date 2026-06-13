using System;
using System.Collections;

//적 정보를 저장하는 데이터 타입. JSON 파일에서 값들을 가져올 거임.
[Serializable]
public class EnemyData
{
   public string id = "none";                            //적 고유 id

   public float hitPoints = 0;                           //적 체력

   public string movementPatternId = "none";         //적이 사용하는 이동 패턴 id 
   
   public string bulletPatternId = "none";               //적이 사용하는 탄알 패턴 id

   public string[] phaseActionIds = new string[0];       //보스 적의 경우 페이즈 데이터 id들
}
