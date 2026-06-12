using System;

public enum SpawnPoint
{
    None
}
[Serializable]
public class EnemyGroup
{
    public string enemyId = "none";                                 //스폰되는 적의 id
    public int count = 0;                                           //해당 적을 몇 마리 스폰 할 것인지
    public float startDelay = 0;                                    //웨이브 시작 후 각 EnemyGroup이 몇 초 뒤에 스폰될 건지. 동시에 여러 그룹이 스폰될 수 있다. 
    public float spawnInterval = 0;                                 //한 EnemyGroup내에서 적들을 몇 초 간격으로 스폰할 건지
    public string spawnPointId = "none";                            //적의 스폰 위치. Enum SpawnPoints 타입으로 제어.
}
[Serializable]
public class WaveData
{   
    public string id = "none";                                      //웨이브 고유 id   
    public float startDelay = 0;                                    //웨이브 선딜레이
    public float nextWaveDelay = 0;                                 //다음 웨이브까지의 딜레이. 해당 시간이 지나면 자동으로 다음 웨이브 시작.
    public EnemyGroup[] spawnGroups = new EnemyGroup[0];            //해당 웨이브에서 스폰되는 적 무리'들'을 담은 배열 
}
