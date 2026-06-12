using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    //싱글톤
    public static WaveManager Instance;
    
    //웨이브 데이터
    private List<WaveData> waveDataList;                            //웨이브 데이터 묶음 
    private WaveData currentWaveData;                               //현재 웨이브 정보 
    private int currentWaveIdx;                                     //현재 웨이브 번호. waveDataList 탐색할 때 활용.

    private int totalActiveEnemyNum;                                //현재 씬에 살아있는 오브젝트 수. 여러 웨이브 밀리더라도 총 오브젝트 수가 0이면 스테이지 클리어 검사에 활용 가능. 
    private List<EnemyController> currentWaveActiveEnemyList;       //현재 진행 중인 웨이브에서 살아있는 오브젝트 리스트. 스폰하면 추가하고 죽으면 빼는 방식. 
    private bool isWaveSpawnStart;                                  //현재 웨이브 스폰 시작 했는지. Update문에서 중복 시작 방지용.
    private bool isWaveSpawnEnd;                                    //현재 웨이브에서 모든 오브젝트를 스폰완료 했는지. 완료한 시점부터 모든 적 죽였는지 검사 가능.

    private int tempEnemyGroupCount;                                //웨이브가 모두 스폰 완료 했는지를 실제로 검사하기 위한 숫자. 

    //내부 필드
    private float nextWaveDelayTimer;                               //다음 웨이브까지 딜레이 타이머

    //GameManager에서 관리하는 WaveManager on/off 변수
    private bool isWaveManagerActive = false;

    void Start()
    {
        //싱글톤 초기화
        if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        //waveDataList 받아오기
        waveDataList = new List<WaveData>();
        waveDataList = DataManager.Instance.GetWaveDataList();

        if(waveDataList != null)
        {
            currentWaveData = waveDataList[0];
            currentWaveIdx = 0;
        }
        else
        {
            Debug.Log("Error: Failed to initialize WaveDataList");
        }

        //필드 초기화
        totalActiveEnemyNum = 0;
        isWaveSpawnStart = false;
        isWaveSpawnEnd = true;

        tempEnemyGroupCount = 0;

        nextWaveDelayTimer = 0f;
    }
    void Update()
    {
        if(!isWaveManagerActive)
            return;
        if(!isWaveSpawnStart)
        {
            isWaveSpawnStart = true;
            isWaveSpawnEnd = false;
            currentWaveActiveEnemyList = new List<EnemyController>();
            StartCoroutine(StartWave(currentWaveData));
            tempEnemyGroupCount = currentWaveData.spawnGroups.Length;
        }
        if(tempEnemyGroupCount <= 0)
        {
            isWaveSpawnEnd = true;
        }
        if(isWaveSpawnStart && isWaveSpawnEnd)
        {
            nextWaveDelayTimer += Time.deltaTime;
            if(CheckNextWaveCondition())
            {
                nextWaveDelayTimer = 0f;

                //currentWave를 다음 웨이브로 갱신
                currentWaveIdx++;
                if(currentWaveIdx >= waveDataList.Count)        //웨이브를 끝까지 돌았을 경우 웨이브 스폰 그만. 
                {
                    StopWaveManager();          
                    return;
                }
                currentWaveData = waveDataList[currentWaveIdx];
                isWaveSpawnStart = false;
            }
        }
    }

    //외부에서 호출하여 WaveManager를 On/Off 하는 함수 
    public void StartWaveManager()
    {
        isWaveManagerActive = true;
    }
    public void StopWaveManager()
    {
        isWaveManagerActive = false;
    }
    //웨이브 코루틴을 실행. 웨이브 코루틴 내에서도 여러개의 StartEnemyGroupSpawn 코루틴이 실행된다. 
    public IEnumerator StartWave(WaveData data)
    {   
        yield return new WaitForSeconds(data.startDelay);       //선딜레이 반영 
        //스폰
        foreach(var enemyGroup in data.spawnGroups)
        {
            StartCoroutine(StartEnemyGroupSpawn(enemyGroup));
        }
    }
    //EnemyGroup에 있는 적들을 모두 스폰하는 코루틴 
    public IEnumerator StartEnemyGroupSpawn(EnemyGroup groupData)
    {
        //선딜레이 반영 
        yield return new WaitForSeconds(groupData.startDelay);

        for(int i=0; i<groupData.count; i++)
        {
            //특정 위치에 스폰
            GameObject enemyObj = DataManager.Instance.GetEnemyPrefab(groupData.enemyId);
            var enemy = Instantiate(enemyObj);                                              /////////////////스폰 위치값 반영 하는 걸로 나중에 바꿔주세요
            //현재 화면 정보 값들 업데이트 
            totalActiveEnemyNum++;
            currentWaveActiveEnemyList.Add(enemy.GetComponent<EnemyController>());
            //특정 시간 텀을 두기
            yield return new WaitForSeconds(groupData.spawnInterval);
        }
        tempEnemyGroupCount--;
    }
    //다음 웨이브로 진행할 지 체크하는 함수 
    private bool CheckNextWaveCondition()
    {   
        //웨이브 스폰 끝난 후 일정 시간이 지나면 다음 웨이브 실행 (=웨이브 밀릴 수 있음)
        if(nextWaveDelayTimer >= currentWaveData.nextWaveDelay)
            return true;
        
        //웨이브 스폰 끝난 후 해당 웨이브를 포함한 화면 내 모든 적을 처치하면 다음 웨이브 실행.
        if(currentWaveActiveEnemyList.Count <= 0 && totalActiveEnemyNum <= 0)
            return true;
        
        return false;
    }
    
    public void RemoveEnemyFromList(EnemyController e)
    {
        if(currentWaveActiveEnemyList.Contains(e))
        {
            currentWaveActiveEnemyList.Remove(e);
            totalActiveEnemyNum--;
        }
        else
        {
            Debug.Log($"Error: Can't find enemy in currentWaveActiveEnemyList.");
        }
    }
}
