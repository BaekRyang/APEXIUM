using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

public class ObjectPoolManager : MonoBehaviour
{
    private interface IComponentPool
    {
        void ReturnObject(Component _obj);

        Component GetObject(bool _isActive);

        GameObject GetOriginalPrefab();
    }

    private class ObjectPool<T> : IComponentPool where T : Component
    {
        private readonly Transform  _parentTransform;
        private readonly GameObject _prefab;
        private readonly Queue<T>   _pool = new();

        public ObjectPool(Transform _parentTransform, GameObject _prefab, int _poolSize)
        {
            this._parentTransform = _parentTransform;
            this._prefab          = _prefab;

            for (int _i = 0; _i < _poolSize; _i++)
            {
                GameObject _obj = Instantiate(_prefab, this._parentTransform);
                _obj.SetActive(false);
                _pool.Enqueue(_obj.GetComponent<T>());
            }
        }

        public void ReturnObject(Component obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_parentTransform);
            _pool.Enqueue(obj as T);
        }

        public GameObject GetOriginalPrefab()
        {
            return _prefab;
        }

        public Component GetObject(bool _isActive)
        {
            if (_pool.Count == 0)
            {
                GameObject _gameObject = Instantiate(_prefab, _parentTransform);
                _gameObject.SetActive(_isActive);
                return _gameObject.GetComponent<T>();
            }

            T _obj = _pool.Dequeue();
            _obj.gameObject.SetActive(_isActive);
            return _obj;
        }
    }

    private readonly Dictionary<string, IComponentPool> _pools       = new();
    private readonly Dictionary<Component, string>      _objPath     = new();
    private readonly Dictionary<Type, string>           _defaultPath = new();

    public GameObject GetPrefab<T>() where T : Component => _pools[_defaultPath[typeof(T)]].GetOriginalPrefab();

    public void MakePool<T>(string _path, int _size) where T : Component
    {
        if (_pools.ContainsKey(_path))
            return;

        GameObject _originalPrefab = Addressables.LoadAssetAsync<GameObject>(_path).WaitForCompletion();

        GameObject _poolObj = new(_originalPrefab.name + "Pool");
        _poolObj.transform.SetParent(transform);

        var _pool = new ObjectPool<T>(_poolObj.transform, _originalPrefab, _size);
        _pools.Add(_path, _pool);
        _defaultPath[typeof(T)] = _path;
    }

    public T GetObject<T>(bool _isActive = true) where T : Component => GetObject<T>(_defaultPath[typeof(T)], _isActive);

    public T GetObject<T>(string _path, bool _isActive = true) where T : Component
    {
        if (_pools.ContainsKey(_path) == false)
            throw new Exception("Not Exist Pool " + _path);

        T _obj = _pools[_path].GetObject(_isActive) as T;

        if (_obj == null)
            return null;
        
        _objPath.Add(_obj, _path);
        return _obj;
    }

    public void ReturnObject<T>(T _object) where T : Component
    {
        if (_objPath.ContainsKey(_object) == false)
            throw new Exception("Not from ObjPool:  " + _object.name);

        string _path = _objPath[_object];
        _pools[_path].ReturnObject(_object);
        _objPath.Remove(_object);
    }

    private void OnDestroy()
    {
        foreach (var _pool in _pools)
            Addressables.Release(_pool.Value.GetOriginalPrefab());
    }
}