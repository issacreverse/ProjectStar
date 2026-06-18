using UnityEngine;

public class PlayerPartyManager : MonoBehaviour
{
    public static PlayerPartyManager Instance; 

    private const int MAX_PARTY_CHARACTERS = 3;

    [SerializeField] private Transform characterRoot;

    //내부 필드
    private GameObject currentPlayerObject;
    private GameObject[] party;  
    private int idx;
    

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        party = new GameObject[MAX_PARTY_CHARACTERS];
        idx = 0;
    }
    public void AddCharacter(GameObject character)
    {
        if(idx >= 3)
            return;
        
        party[idx++] = character;
    }

    public void SwitchCharacter(int num)
    {   
        if(currentPlayerObject != null)
        {
            currentPlayerObject.transform.SetParent(null);
            currentPlayerObject.SetActive(false);
        }
        
        currentPlayerObject = party[num-1];

        currentPlayerObject.SetActive(true);
        currentPlayerObject.transform.SetParent(characterRoot);
    }
    public GameObject GetCurrentPlayerCharacter()
    {
        return currentPlayerObject;
    }
}
