using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //싱글톤 초기화 
    public static GameManager Instance;

    //임시
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] prefab;
    [SerializeField] Transform characterRoot;
    //외부 참조
    [SerializeField] private InputActionAsset inputActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //싱글톤 초기화
        if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        GameObject go = Instantiate(prefab[0], characterRoot.position, Quaternion.identity);
        go.SetActive(false);
        PlayerPartyManager.Instance.AddCharacter(go);
        player.GetComponent<PlayerController>().SwitchPlayerCharacter(1);

        go = Instantiate(prefab[1], characterRoot.position, Quaternion.identity);
        go.SetActive(false);
        PlayerPartyManager.Instance.AddCharacter(go);

        go = Instantiate(prefab[2], characterRoot.position, Quaternion.identity);
        go.SetActive(false);
        PlayerPartyManager.Instance.AddCharacter(go);

        WaveManager.Instance.StartWaveManager();
    }

    //공개 함수
    public InputActionAsset GetInputActionAsset()
    {
        if(inputActions == null)
        {
            Debug.Log("Error: InputActionAsset is null");
        }
        return inputActions;
    }
}
