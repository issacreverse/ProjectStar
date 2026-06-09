using UnityEngine;
using UnityEngine.Pool;

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
    }
    private void OnGetFromPool(EnemyBullet pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }
    private void OnDestroyPooledObject(EnemyBullet pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }
}
