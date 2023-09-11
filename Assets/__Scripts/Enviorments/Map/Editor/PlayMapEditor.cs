using UnityEditor;
using UnityEngine;

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
