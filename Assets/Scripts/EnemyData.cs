using System;
using System.Collections;

 //적 정보를 저장하는 데이터 타입. JSON 파일에서 값들을 가져올 거임.
[Serializable]
public class EnemyData
{
   public string id;
   public float moveSpeed;
   public string bulletPatternId;
}
