using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class EnergyCrystal : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueText;

    private bool IsEmphasis => remainingEmphasisTime > 0 || remainingReturnTime > 0;

    private void Awake()
    {
        GetComponent<UIElementUpdater>().OnUpdateValue += (_, _) => SetValue();
    }
    

    private UniTask _task;

    private void SetValue()
    {
        Debug.Log("ACT" + _task.Status);
        remainingEmphasisTime = EMPHASIS_TIME;
        remainingReturnTime   = RETURN_TIME;
        if (_task.Status == UniTaskStatus.Pending) return;
        _task = EmphasisIndexText();
    }

    private const float EMPHASIS_TIME = 1.2f;
    private const float RETURN_TIME   = .4f;

    [SerializeField] [Range(0, EMPHASIS_TIME)]
    private float remainingEmphasisTime;

    [SerializeField] [Range(0, RETURN_TIME)]
    private float remainingReturnTime;

    private async UniTask EmphasisIndexText()
    {
        Debug.Log("EmphasisIndexText");
        while (IsEmphasis)
        {
            try
            {
                if (remainingEmphasisTime > 0)
                {
                    remainingEmphasisTime -= Time.deltaTime;

                    _valueText.transform.localScale = Vector3.one * 1.5f;
                    _valueText.color                = Color.green;
                }
                else if (remainingReturnTime > 0)
                {
                    remainingReturnTime -= Time.deltaTime;

                    _valueText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one                    * 1.5f, remainingReturnTime / RETURN_TIME);
                    _valueText.color                = Color.Lerp(Color.white, Color.green, remainingReturnTime / RETURN_TIME);
                }

                await UniTask.Yield();
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e);
                throw;
            }
        }
    }
}