using System;
using UnityEngine.AI;

public enum SpawnPoint
{
    None,

    //화면 밖 기본 방향 
    RightOutside, 
    TopOutside,  
    BottomOutside,  

    //화면 안쪽 고정 위치
    Center,  

    //코너
    TopRight,  
    BottomRight, 

    //랜덤
    RandomTop, 
    RandomBottom,  
    RandomRight,    

    //플레이어 기준
    NearPlayer,  
    BehindPlayer,  

    LineRight,
    LineTop,
    LineBottom
}
[Serializable]
public class EnemyGroup
{
    public string enemyId = "none";                                 //스폰되는 적의 id

    public int count = 0;                                           //해당 적을 몇 마리 스폰 할 것인지

    public float startDelay = 0;                                    //웨이브 시작 후 각 EnemyGroup이 몇 초 뒤에 스폰될 건지. 동시에 여러 그룹이 스폰될 수 있다. 
    public float spawnInterval = 0;                                 //한 EnemyGroup내에서 적들을 몇 초 간격으로 스폰할 건지

    public string spawnPoint = "none";                              //적의 스폰 위치. Enum SpawnPoints 타입으로 제어.

    //스폰 위치 조정 offset
    public float spawnPosOffsetX = 0f;                              //x좌표 offset
    public float spawnPosOffsetY = 0f;                              //y좌표 offset

    //Line형식의 스폰일 경우, 적들의 스폰 간격                             
    public float lineOffsetX = 0f;                                  //x좌표 offset
    public float lineOffsetY = 0f;                                  //y좌표 offset
}
[Serializable]
public class WaveData
{   
    public string id = "none";                                      //웨이브 고유 id  

    public float startDelay = 0;                                    //웨이브 선딜레이
    public float nextWaveDelay = 0;                                 //다음 웨이브까지의 딜레이. 해당 시간이 지나면 자동으로 다음 웨이브 시작.
    
    public EnemyGroup[] spawnGroups = new EnemyGroup[0];            //해당 웨이브에서 스폰되는 적 무리'들'을 담은 배열 
}
