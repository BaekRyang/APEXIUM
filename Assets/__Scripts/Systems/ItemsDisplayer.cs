using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject                         itemPrefab;
    private readonly         Dictionary<int, (Image, TMP_Text)> _items         = new();
    private const            int                                MAX_ITEM_COUNT = 50;

    [Inject] private ItemManager _itemManager;

    private void OnEnable()  => EventBus.Subscribe<UpdateItemEvent>(OnItemUpdated);
    private void OnDisable() => EventBus.Unsubscribe<UpdateItemEvent>(OnItemUpdated);
    private void Start()     => DIContainer.Inject(this);

    private void OnItemUpdated(UpdateItemEvent _e)
    {
        int _itemID = _e.ItemID;
        Debug.Log($"<color=blue>ItemsDisplayer</color> : {_itemID} is updated.");
        Items _updatedItems = _e.Item;

        int _targetItemAmount = _updatedItems.GetItemAmount(_itemID);
        if (_e.ChangeAmount < 0) //아이템이 삭제되었을 때
        {
            if (_targetItemAmount == 0) //아이템이 모두 삭제되었을 때
            {
                //오브젝트 삭제후 딕셔너리에서도 삭제
                Destroy(_items[_itemID].Item1.gameObject);
                _items.Remove(_itemID);
            }
            else //개수만 업데이트
                _items[_itemID].Item2.text = _updatedItems.GetItemAmount(_itemID).ToString();

            return;
        }

        //아이템이 추가되었을 때
        Debug.Log($"<color=blue>ItemsDisplayer</color> : {_itemID} is added. {_targetItemAmount}");
        if (_targetItemAmount == 1) //해당 아이템이 새로 추가되었을 때
        {
            if (_items.Count >= MAX_ITEM_COUNT) //최대 개수를 넘어섰을 때
            {
                Debug.Log("<color=red>ItemsDisplayer</color> : Item count is over the limit.");
                return;
            }

            //새로 만들고 이미지 지정
            GameObject _itemObject = Instantiate(itemPrefab, transform);

            _items.Add(_itemID, (_itemObject.GetComponent<Image>(), _itemObject.GetComponentInChildren<TMP_Text>()));
            _items[_itemID].Item1.sprite = _itemManager.GetItem(_itemID).sprite;
        }
        else //이미 있었으면 개수 업데이트
            _items[_itemID].Item2.text = "x" + _updatedItems.GetItemAmount(_itemID);
    }
}