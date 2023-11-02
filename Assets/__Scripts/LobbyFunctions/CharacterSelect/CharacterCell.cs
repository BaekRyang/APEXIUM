using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCell : MonoBehaviour
{
    [SerializeField] public Button button;
    [SerializeField] public int    index;
    [SerializeField] public int    selectedID;

    [SerializeField] public Image      lockOnImage;
    [SerializeField] public GameObject ready;
    [SerializeField] public TMP_Text   readyText;

    [SerializeField] public List<int> lockedID = new();
    
    [SerializeField] public PlayerData playerData;

    private void Start()
    {
        GetData();
        
        ready.SetActive(false);
    }

    private void GetData()
    {
        button      = GetComponent<Button>();
        index       = transform.GetSiblingIndex();
        selectedID  = -1;
        lockOnImage = GetComponent<Image>();
        ready       = transform.Find("Ready").gameObject;
        readyText   = ready.GetComponentInChildren<TMP_Text>();

        button.onClick.AddListener(() => SelectCharacter(this));
    }

    private void SelectCharacter(CharacterCell _cell)
    {
        //TODO: 우선은 로컬 플레이어만 고려
        EventBus.Publish(new CharacterSelectEvent(_cell.index, 0)); 
    }
}

public class CharacterSelectEvent
{
    public int index;
    public int selectorID; //멀티플레이 기준사용
    
    public CharacterSelectEvent(int _index, int _selectorID)
    {
        index = _index;
        selectorID = _selectorID;
    }
}