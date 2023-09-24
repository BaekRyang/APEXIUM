using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private Color         selectedColor, unSelectedColor, lockedColor;
    [SerializeField] private TMP_Text      readyButtonText;
    [SerializeField] private RectTransform charactersTransform;
    [SerializeField] private RectTransform readyGaugeTransform;
    [SerializeField] private bool          isMultiplayer;

    private bool IsLocalReady => lockOnIndex != -2;

    [SerializeField] private CharacterCell[] characters;
    [SerializeField] private int             lastSelectedIndex = -1;
    [SerializeField] private int             lockOnIndex       = -2;

    private void Start()
    {
        EventBus.Subscribe<CharacterSelectEvent>(OnCharacterSelect);
        EventBus.Subscribe<ButtonPressedAction>(OnButtonPressed);

        if (characters.Length == 0)
            GetCharacterCells();
    }

    private void OnButtonPressed(ButtonPressedAction _obj)
    {
        if (_obj.buttonName == "Back")
            ClearCharacterCells();

        if (_obj.buttonName != "Ready") return;

        switch (IsLocalReady)
        {
            case true:
                UnReady(Client.ClientID);
                break;

            case false:
                Ready(Client.ClientID);
                if (!isMultiplayer)
                    WaitToStart();
                break;
        }
    }

    [SerializeField] private float waitTime = 5f;

    private async void WaitToStart()
    {
        Slider _gauge = readyGaugeTransform.GetComponentInParent<Slider>();
        Image  _fill  = _gauge.fillRect.GetComponent<Image>();
        _gauge.value = 0;
        while (_gauge.value < 1)
        {
            if (!IsLocalReady)
            {
                _gauge.value = 0;
                return;
            }

            _gauge.value += Time.deltaTime / waitTime;
            _fill.color  =  Color.Lerp(Color.black, new Color(0, .55f, 1), _gauge.value);

            await UniTask.Yield();
        }

        StartGame(_gauge);
    }

    private async void StartGame(Slider _gauge)
    {
        Image _image              = _gauge.fillRect.GetComponent<Image>();
        UniTask _textAnimationTask = readyGaugeTransform.GetComponent<MMF_Player>().PlayFeedbacksUniTask(transform.position);
        UniTask  _gaugeAnimationTask = LerpColor(_image, _image.color, Color.green, .5f);
        
        await UniTask.WhenAll(_textAnimationTask, _gaugeAnimationTask);
        Debug.Log("START GAME");
        
        //입력을 막는다.
        EventSystem.current.enabled = false;
        
        //로딩
        EventBus.Publish(new ButtonPressedAction("LoadGame"));
    }

    private async UniTask LerpColor(Image _target, Color _from, Color _to, float _duration)
    {
        float _time = 0;
        while (_time < _duration)
        {
            _time += Time.deltaTime;
            _target.color = Color.Lerp(_from, _to, _time / _duration);
            await Task.Yield();
        }
    }

    private void Ready(int _selectorID)
    {
        if (lastSelectedIndex == -1)
            return;

        readyButtonText.text = "Cancel";

        lockOnIndex = lastSelectedIndex;

        characters[lockOnIndex].lockedID.Add(_selectorID);

        characters[lockOnIndex].ready.SetActive(true);
        characters[lockOnIndex].lockOnImage.color = lockedColor;

        characters[lockOnIndex].readyText.text = isMultiplayer ?
            $"P{_selectorID} Ready" :
            "Ready";
    }

    private void UnReady(int _selectorID)
    {
        if (lockOnIndex == -1)
            return;

        readyButtonText.text = "Ready";

        characters[lockOnIndex].lockedID.Remove(_selectorID);

        characters[lockOnIndex].ready.SetActive(false);
        characters[lockOnIndex].lockOnImage.color = unSelectedColor;

        lockOnIndex = -2;

        if (isMultiplayer)
            characters[lastSelectedIndex].readyText.text =
                $"P{GetLockedPlayersString(characters[lastSelectedIndex].lockedID)}Select";
    }

    private string GetLockedPlayersString(List<int> _lockedID)
    {
        StringBuilder _lockedPlayersString = new();

        foreach (int _id in _lockedID)
            _lockedPlayersString.Append(_id);

        return _lockedPlayersString.ToString();
    }

    private async void OnCharacterSelect(CharacterSelectEvent _obj)
    {
        await UniTask.Yield();
        int _clickedCellIndex = _obj.index;
        Debug.Log($"CharacterSelectEvent: {_clickedCellIndex}, {lastSelectedIndex}");
        if (IsLocalReady)
        {
            characters[_clickedCellIndex].lockOnImage.color = selectedColor;
            if (lastSelectedIndex != _clickedCellIndex)
                characters[lastSelectedIndex].lockOnImage.color = unSelectedColor;
            characters[lockOnIndex].lockOnImage.color = lockedColor;
        }
        else
        {
            Debug.Log($"lastSelectedIndex: {lastSelectedIndex}, _obj.index: {_clickedCellIndex}");
            characters[_clickedCellIndex].lockOnImage.color = selectedColor;
            if (lastSelectedIndex != -1)
                characters[lastSelectedIndex].lockOnImage.color = unSelectedColor;
        }


        lastSelectedIndex = _clickedCellIndex;
    }

    private void GetCharacterCells()
    {
        characters = charactersTransform.GetComponentsInChildren<CharacterCell>();
    }

    private void ClearCharacterCells()
    {
        // foreach (CharacterCell _characterCell in characters)
        // {
        //     _characterCell.selectedID = -1;
        //     _characterCell.ready.SetActive(false);
        //     _characterCell.lockOnImage.color = unSelectedColor;
        //     _characterCell.lockedID.Clear();
        // }
        lastSelectedIndex = -1;
        UnReady(Client.ClientID);
    }

    private void SelectCharacter(CharacterCell _cell)
    {
    }
}