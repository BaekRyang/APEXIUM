using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Events;

public class EV : MonoBehaviour
{
    [MenuItem("CONTEXT/TextMeshPro/Localize")]
    static void LocalizeTMProText(MenuCommand command)
    {
        var target = command.context as TextMeshPro;
        SetupForLocalization(target);
    }

    public static MonoBehaviour SetupForLocalization(TextMeshPro target)
    {
        LocalizeStringEvent comp            = Undo.AddComponent(target.gameObject, typeof(LocalizeStringEvent)) as LocalizeStringEvent;
        MethodInfo          setStringMethod = target.GetType().GetProperty("text").GetSetMethod();
        UnityAction<string> methodDelegate  = System.Delegate.CreateDelegate(typeof(UnityAction<string>), target, setStringMethod) as UnityAction<string>;
        comp.OnUpdateString.AddListener(methodDelegate);
        comp.OnUpdateString.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
        return comp;
    }
}