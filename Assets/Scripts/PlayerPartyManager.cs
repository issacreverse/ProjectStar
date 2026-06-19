using System.Collections;
using UnityEngine;

public class PlayerPartyManager : MonoBehaviour
{
    public static PlayerPartyManager Instance; 

    private const int MAX_PARTY_CHARACTERS = 3;
    private const float SWITCH_COOLDOWN = 3;

    [SerializeField] private Transform characterRoot;

    //내부 필드
    private GameObject currentPlayerObject;
    private GameObject[] party;  
    private int idx;
    private bool isSwitchReady;
    

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        party = new GameObject[MAX_PARTY_CHARACTERS];

        idx = 0;
        isSwitchReady = true;
    }
    public void AddCharacter(GameObject character)
    {
        if(idx >= 3)
            return;
        
        party[idx++] = character;
        if(character != null)
            character.transform.SetParent(characterRoot);
    }

    public void SwitchCharacter(int num)
    {   
        //교체 쿨타임이 돌아야 진행
        if(!isSwitchReady)
            return;
        //현재 캐릭터랑 같은 캐릭터로 교체 요구인지 판단
        //아닐 경우에만 교체 진행
        if(currentPlayerObject != null)
        {
            if(currentPlayerObject == party[num-1])
                return;
        }
        //실제 교체 구현부
        StartCoroutine(SwitchCoroutine(num));
    }
    private IEnumerator SwitchCoroutine(int num)
    {
        isSwitchReady = false;

        if(currentPlayerObject != null)
        {
            currentPlayerObject.SetActive(false);
        }
        
        currentPlayerObject = party[num-1];

        currentPlayerObject.SetActive(true);

        yield return new WaitForSeconds(SWITCH_COOLDOWN);

        isSwitchReady = true;
    }
    public GameObject GetCurrentPlayerCharacter()
    {
        return currentPlayerObject;
    }
}
