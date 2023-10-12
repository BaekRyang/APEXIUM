using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPoolManager : MonoBehaviour
{
    public interface IComponentPool
    {
        void      ReturnObj(Component obj);
        Component GetObject(bool      _isActive);

        GameObject GetPrefab();
    }

    public class ObjPool<T> : IComponentPool where T : Component
    {
        public          Transform  parentTransform;
        public          GameObject prefab;
        public readonly Queue<T>   pool = new();

        public ObjPool(Transform _parentTransform, GameObject _prefab, int _poolSize)
        {
            parentTransform = _parentTransform;
            prefab          = _prefab;

            for (int i = 0; i < _poolSize; i++)
            {
                var _obj = Instantiate(_prefab, parentTransform);
                _obj.SetActive(false);
                pool.Enqueue(_obj.GetComponent<T>());
            }
        }

        public void ReturnObj(Component obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(this.parentTransform);
            pool.Enqueue(obj as T);
        }

        public GameObject GetPrefab()
        {
            return prefab;
        }

        public Component GetObject(bool _isActive)
        {
            T obj;

            if (pool.Count == 0)
            {
                var gameObj = Instantiate(prefab, parentTransform);
                obj = gameObj.GetComponent<T>();
                obj.gameObject.SetActive(_isActive);

                return obj;
            }

            obj = pool.Dequeue();
            obj.gameObject.SetActive(_isActive);
            return obj;
        }
    }

    private readonly Dictionary<string, IComponentPool> _pools = new();
    private readonly Dictionary<Component, string> _objPath = new();
    private readonly Dictionary<Type, string> _defaultPath = new();
    
    public GameObject GetPrefab<T>() where T : Component => _pools[_defaultPath[typeof(T)]].GetPrefab();

    public void MakePool<T>(string path, int size) where T : Component
    {
        if (_pools.ContainsKey(path))
        {
            return;
        }

        var _obj = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();

        var _poolObj = new GameObject();
        _poolObj.name = _obj.name + "Pool";

        _poolObj.transform.SetParent(transform);

        var pool = new ObjPool<T>(_poolObj.transform, _obj, size);

        _pools.Add(path, pool);

        _defaultPath[typeof(T)] = path;
    }

    public T GetObj<T>(bool _isActive = true) where T : Component => GetObj<T>(_defaultPath[typeof(T)], _isActive);

    public T GetObj<T>(string _path, bool _isActive = true) where T : Component
    {
        if (_pools.ContainsKey(_path) == false) 
            throw new Exception("Not Exist Pool " + _path);

        var obj = _pools[_path].GetObject(_isActive) as T;

        _objPath.Add(obj, _path);

        return obj;
    }

    public void ReturnObj<T>(T obj) where T : Component
    {
        if (_objPath.ContainsKey(obj) == false)
        {
            throw new Exception("Not from ObjPool:  " + obj.name);
        }

        var path = _objPath[obj];
        _pools[path].ReturnObj(obj);
        _objPath.Remove(obj);
    }

    private void OnDestroy()
    {
        foreach (var pool in _pools)
        {
            Addressables.Release(pool.Value.GetPrefab());
        }
    }
}