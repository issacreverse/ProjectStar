using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    //싱글톤 
    public static DataManager Instance;

    //모든 적들의 정보들이 담겨있는 테이블 
    private Dictionary<string, EnemyData> enemyTable;
    private Dictionary<string, EnemyBulletPatternData> enemyBulletPatternTable;

    void Awake()
    {
        //싱글톤 초기화 
        if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        enemyTable = new Dictionary<string, EnemyData>();
        enemyBulletPatternTable = new Dictionary<string, EnemyBulletPatternData>();
        
        LoadAllEnemyJsonFiles();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void LoadAllEnemyJsonFiles()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("EnemyData");
        foreach(TextAsset jsonFile in jsonFiles)
        {
            EnemyData enemyData = JsonUtility.FromJson<EnemyData>(jsonFile.text);

            if(enemyTable.ContainsKey(enemyData.id))
            {
                Debug.Log("Error: multiple key values");
                continue;
            }
            Debug.Log("ki");
            enemyTable.Add(enemyData.id, enemyData);
        }
        
        TextAsset[] jsonFiles2 = Resources.LoadAll<TextAsset>("EnemyBulletPatternData");
        foreach(TextAsset jsonFile2 in jsonFiles2)
        {
            EnemyBulletPatternData enemyBulletPatternData = JsonUtility.FromJson<EnemyBulletPatternData>(jsonFile2.text);

            if(enemyBulletPatternTable.ContainsKey(enemyBulletPatternData.id))
            {
                Debug.Log("Error: multiple key values");
                continue;
            }
            Debug.Log("Wi");
            enemyBulletPatternTable.Add(enemyBulletPatternData.id, enemyBulletPatternData);
        }
    }
    public EnemyData GetEnemyData(string enemyId)
    {
        if(enemyTable == null)
        {
            Debug.Log("hihi");
        }
        if(enemyTable.TryGetValue(enemyId, out EnemyData data))
        {
            return data;
        }

        Debug.Log($"Error: Can't Find values for key: {enemyId}");
        return null;
    }
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

}
