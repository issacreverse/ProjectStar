using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class PoolingManager : MonoBehaviour
{
    //싱글톤 
    public static PoolingManager Instance;

    //오브젝트 풀링 관련 필드 
    public IObjectPool<EnemyBullet> objectPool;
    [SerializeField] private EnemyBullet EnemyBulletPrefab;
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;

    //내부 필드
    //폭탄에 사용. 현재 활성화된 탄알을 모두 추적합니다.
    private List<EnemyBullet> activeBullets;        

    void Awake()
    {
        //오브젝트 풀링 풀 초기 생성
        objectPool = new ObjectPool<EnemyBullet>(CreateObject,
            OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject,
            collectionCheck, defaultCapacity, maxSize);
        
        //싱글톤 초기화
        if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        activeBullets = new List<EnemyBullet>();
    }
    //오브젝트 풀링 내부 함수. 풀 생성 시 매개변수로 전달함.
    private EnemyBullet CreateObject()
    {
        EnemyBullet EnemyBulletInstance = Instantiate(EnemyBulletPrefab);
        return EnemyBulletInstance;
    }
    private void OnReleaseToPool(EnemyBullet pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
        activeBullets.Remove(pooledObject);
    }
    private void OnGetFromPool(EnemyBullet pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
        activeBullets.Add(pooledObject);
    }
    private void OnDestroyPooledObject(EnemyBullet pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }
    public void EraseAllActiveBullets()
    {
        //폭탄이 터질 때 활성화된 모든 탄알을 제거하는 함수. 
        while(activeBullets.Count > 0)
        {
            objectPool.Release(activeBullets[0]);
        }
    }
}
