using System.Collections;
using UnityEngine;
using System;

public class PlayerPartyManager : MonoBehaviour
{
    public static PlayerPartyManager Instance; 

    private const float SWITCH_COOLDOWN = 3;

    [SerializeField] private Transform characterRoot;

    //내부 필드
    private GameObject currentPlayerObject;
    private GameObject[] party;  
    private int idx;
    private bool isSwitchReady;

    //플레이어 교체 이벤트
    private Action[] OnSwitchCharacters;
    

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        party = new GameObject[GameConstants.MAX_PARTY_CHARACTERS];

        idx = 0;
        isSwitchReady = true;

        OnSwitchCharacters = new Action[3];
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
        int idx = num-1;
        //교체 쿨타임이 돌아야 진행
        if(!isSwitchReady)
            return;
        //현재 캐릭터랑 같은 캐릭터로 교체 요구인지 판단
        //아닐 경우에만 교체 진행
        if(currentPlayerObject != null)
        {
            if(currentPlayerObject == party[idx])
                return;
        }
        //실제 교체 구현부
        StartCoroutine(SwitchCoroutine(num));
    }
    private IEnumerator SwitchCoroutine(int num)
    {
        int idx = num-1;
        isSwitchReady = false;

        if(currentPlayerObject != null)
        {
            currentPlayerObject.SetActive(false);
        }
        
        currentPlayerObject = party[idx];

        currentPlayerObject.SetActive(true);
        //외부에서 등록한 캐릭터 교체 이벤트 호출
        OnSwitchCharacters[idx]?.Invoke();
        //캐릭터 고유의 교체 콜백 함수 호출 
        currentPlayerObject.GetComponent<PlayerCharacterBase>().OnSwitchCharacter();

        yield return new WaitForSeconds(SWITCH_COOLDOWN);

        isSwitchReady = true;
    }
    public GameObject GetCurrentPlayerCharacter()
    {
        return currentPlayerObject;
    }
    public void PartyHeal(float amount)
    {
        foreach(GameObject character in party)
        {
            character.GetComponent<PlayerCharacterBase>().Heal(amount);
        }
    }
    public GameObject[] GetParty()
    {
        return party;
    }

    public void Subscribe(int idx, Action handler)
    {
        OnSwitchCharacters[idx] += handler;
    }
    public void UnSubscribe(int idx, Action handler)
    {
        OnSwitchCharacters[idx] -= handler;
    }
}
