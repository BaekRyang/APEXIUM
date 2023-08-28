using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Resources : MonoBehaviour
{
    public static Resources Instance;

    [SerializeField] private TMP_Text _resourceValue;

    private bool IsEmphasis => _remainingEmphasisTime > 0 || _remainingReturnTime > 0;

    private void Awake()
    {
        Instance ??= this;
    }

    public async void SetResource(int p_currentResource)
    {
        _resourceValue.text = $"{p_currentResource:0}";

        if (IsEmphasis) return;
        EmphasisResourceIndex();
    }

    private const float EMPHASIS_TIME = 1.2f;
    private const float RETURN_TIME   = .4f;

    [SerializeField] [Range(0, EMPHASIS_TIME)]
    private float _remainingEmphasisTime;

    [SerializeField] [Range(0, RETURN_TIME)]
    private float _remainingReturnTime;

    private async void EmphasisResourceIndex()
    {
        Debug.Log("EmphasisResourceIndex");
        _remainingEmphasisTime = EMPHASIS_TIME;
        _remainingReturnTime   = RETURN_TIME;

        while (IsEmphasis)
        {
            if (_remainingEmphasisTime > 0)
            {
                _remainingEmphasisTime -= Time.deltaTime;

                _resourceValue.transform.localScale = Vector3.one * 1.5f;
                _resourceValue.color                = Color.green;
            }
            else if (_remainingReturnTime > 0)
            {
                _remainingReturnTime -= Time.deltaTime;

                _resourceValue.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one                     * 1.5f, _remainingReturnTime / RETURN_TIME);
                _resourceValue.color                = Color.Lerp(Color.white, Color.green, _remainingReturnTime / RETURN_TIME);
            }

            await UniTask.Yield();
        }
    }
}