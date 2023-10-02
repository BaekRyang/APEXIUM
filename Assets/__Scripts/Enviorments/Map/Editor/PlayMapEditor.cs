using UnityEditor;
using UnityEngine;

//TODO : 두 개다 동일한 기능인데 합칠 수 없나?
[CustomEditor(typeof(PlayMap))]
public class PlayMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(50);
        if (GUILayout.Button("LoadData")) 
            LoadData();
    }

    private void LoadData()
    {
        PlayMap _playMap = (PlayMap)target;
        
        _playMap.Initialize();
    }
}

[CustomEditor(typeof(BossPlayMap))]
public class BossPlayMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(50);
        if (GUILayout.Button("LoadData")) 
            LoadData();
    }

    private void LoadData()
    {
        PlayMap _playMap = (PlayMap)target;
        
        _playMap.Initialize();
    }
}
