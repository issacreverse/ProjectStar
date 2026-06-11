using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    //Json 파일들을 불러와 안에 있는 데이터를 Dictionary 형태로 정리해놓는 스크립트 
    //다른 스크립트 들은 Id 값을 이용해 이 스크립트로부터 직접 Data를 받아갑니다. 

    //싱글톤 
    public static DataManager Instance;

    //모든 적들의 정보들이 담겨있는 테이블 
    private Dictionary<string, EnemyData> enemyTable;
    private Dictionary<string, EnemyBulletPatternData> enemyBulletPatternTable;
    private Dictionary<string, EnemyBossPhaseActionData> enemyBossPhaseActionTable;                 //페이즈 데이터만을 저장해둔 것
    private Dictionary<string, List<EnemyBossPhaseActionData>> bossPhaseTable;                      //보스별로 해당 보스가 사용하는 페이즈들을 같이 저장해놓은 것
    //모든 플레이어들의 정보들이 담겨있는 테이블
    private Dictionary<string, PlayerData> playerTable;

    void Awake()
    {
        //싱글톤 초기화 
        if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        //테이블 초기화
        enemyTable = new Dictionary<string, EnemyData>();
        enemyBulletPatternTable = new Dictionary<string, EnemyBulletPatternData>();
        enemyBossPhaseActionTable = new Dictionary<string, EnemyBossPhaseActionData>();
        bossPhaseTable = new Dictionary<string, List<EnemyBossPhaseActionData>>();
        playerTable = new Dictionary<string, PlayerData>();
        //테이블 초기화 후 읽어온다.
        LoadAllEnemyJsonFiles();
        LoadAllPlayerJsonFiles();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //Json 파일에서 EnemyData, EnemyBulletPatternData값을 모두 읽어와서 Dictionary에 저장하는 함수
    private void LoadAllEnemyJsonFiles()
    {
        //EnemyData 읽어오기
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("EnemyData");
        foreach(TextAsset jsonFile in jsonFiles)
        {
            EnemyData enemyData = JsonUtility.FromJson<EnemyData>(jsonFile.text);

            if(enemyTable.ContainsKey(enemyData.id))
            {
                Debug.Log("Error: multiple key values");
                continue;
            }
            enemyTable.Add(enemyData.id, enemyData);
        }
        
        //EnemyBulletPatternData 읽어오기
        TextAsset[] jsonFiles2 = Resources.LoadAll<TextAsset>("EnemyBulletPatternData");
        foreach(TextAsset jsonFile2 in jsonFiles2)
        {
            EnemyBulletPatternData enemyBulletPatternData = JsonUtility.FromJson<EnemyBulletPatternData>(jsonFile2.text);

            if(enemyBulletPatternTable.ContainsKey(enemyBulletPatternData.id))
            {
                Debug.Log("Error: multiple key values");
                continue;
            }
            enemyBulletPatternTable.Add(enemyBulletPatternData.id, enemyBulletPatternData);
        }

        //EnemyBossPhaseActionData들을 읽어와서 딕셔너리 enemyBossPhaseActionTable에 저장하기
        TextAsset[] jsonFiles3 = Resources.LoadAll<TextAsset>("EnemyBossPhaseActionData");
        foreach(TextAsset jsonFile3 in jsonFiles3)
        {
           EnemyBossPhaseActionData enemyBossPhaseActionData = JsonUtility.FromJson<EnemyBossPhaseActionData>(jsonFile3.text);

            if(enemyBulletPatternTable.ContainsKey(enemyBossPhaseActionData.id))
            {
                Debug.Log("Error: multiple key values");
                continue;
            }
            enemyBossPhaseActionTable.Add(enemyBossPhaseActionData.id, enemyBossPhaseActionData);
        }
        
        //enemyBossPhaseActionData를 보스별로 분류해서 정리함. 
        foreach(var pair in enemyTable)
        {
            
            string[] phaseActionIds = pair.Value.phaseActionIds;
            //phaseAction이 있을 경우 => 즉 페이즈별 행동패턴이 다른 보스 같은 몹일 경우
            if(phaseActionIds != null && phaseActionIds.Length > 0)
            {
                
                foreach(string phaseActionId in phaseActionIds)
                {
                    if(enemyBossPhaseActionTable.TryGetValue(phaseActionId, out EnemyBossPhaseActionData data))
                    {
                        if(!bossPhaseTable.ContainsKey(pair.Key))
                        {
                            List<EnemyBossPhaseActionData> list = new List<EnemyBossPhaseActionData>();
                            bossPhaseTable.Add(pair.Key, list);
                        }
                        bossPhaseTable[pair.Key].Add(data);
                    }
                    else
                    {
                        Debug.Log($"Error: Can't find phase action data named {phaseActionId}");
                    }
                }
            }
        }
    }
    //Json 파일에서 playerData 값을 모두 읽어와서 Dictionary에 저장하는 함수
    private void LoadAllPlayerJsonFiles()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("PlayerData");
        foreach(TextAsset jsonFile in jsonFiles)
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonFile.text);

            if(playerTable.ContainsKey(playerData.id))
            {
                Debug.Log("Error: multiple key values");
                continue;
            }
            playerTable.Add(playerData.id, playerData);
        }
    }
    //외부에서 호출하는, EnemyData 가져오는 함수. 
    public EnemyData GetEnemyData(string enemyId)
    {
        if(enemyTable.TryGetValue(enemyId, out EnemyData data))
        {
            return data;
        }

        Debug.Log($"Error: Can't Find values for key: {enemyId}");
        return null;
    }
    //외부에서 호출하는, EnemyBulletPatternData 가져오는 함수.
    public EnemyBulletPatternData GetEnemyBulletPatternData(string enemyId)
    {
        if(enemyTable.TryGetValue(enemyId, out EnemyData data))
        {
            string bulletPatternId = data.bulletPatternId;
            if(enemyBulletPatternTable.TryGetValue(bulletPatternId, out EnemyBulletPatternData bulletPatternData))
            {
                return bulletPatternData;
            }
        }
        Debug.Log($"Error: Can't Find values for key: {enemyId}");
        return null;
    }
    public List<EnemyBossPhaseActionData> GetEnemyBossPhaseActionData(string enemyId)
    {
        if(bossPhaseTable.TryGetValue(enemyId, out List<EnemyBossPhaseActionData> list))
        {
            return list;
        }
        Debug.Log($"Error: Can't Find values for key: {enemyId}");
        return null;
    }
    //외부에서 호출하는, PlayerData 가져오는 함수.
    public PlayerData GetPlayerData(string playerId)
    {
        if(playerTable.TryGetValue(playerId, out PlayerData data))
        {
            return data;
        }
        Debug.Log($"Error: Can't Find values for key: {playerId}");
        return null;
    }
}
