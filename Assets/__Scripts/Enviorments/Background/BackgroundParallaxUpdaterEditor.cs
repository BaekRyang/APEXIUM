using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BackgroundParallaxUpdater))]
public class BackgroundParallaxUpdaterEditor : Editor
{
    private BackgroundParallaxUpdater _parallaxBackground;

    private void OnEnable()
    {
        _parallaxBackground = target as BackgroundParallaxUpdater;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Initialize"))
        {
            foreach (Transform _child in _parallaxBackground.transform)
            {
                SpriteRenderer _spriteRenderer = _child.GetComponent<SpriteRenderer>();
                _spriteRenderer.sortingOrder = _child.GetSiblingIndex() - _parallaxBackground.transform.childCount - 99;
                _child.name                  = $"Layer {_child.GetSiblingIndex()}";

                if (!_child.TryGetComponent(out BackgroundParallaxObject _parallaxObject))
                    _child.AddComponent<BackgroundParallaxObject>();

                _parallaxObject.layerIndex = _child.GetSiblingIndex();
                _parallaxObject.spriteRenderer = _parallaxObject.GetComponent<SpriteRenderer>();
            }
        }
    }
}