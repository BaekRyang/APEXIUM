using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    // ReSharper disable once HeapView.ObjectAllocation
    private readonly JsonConverter[] _converters =
    {
        new SpriteConverter(),
        new EnumTypeConverter<ChangeableStatsTypes>(),
        new EnumTypeConverter<CalculationType>()
    };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemData _itemData = target as ItemData;

        if (_itemData == null) return;

        if (GUILayout.Button("Save"))
        {
            string _location = AssetDatabase.GetAssetPath(_itemData);

            _location = _location.Substring(0, _location.LastIndexOf("/", StringComparison.Ordinal));

            if (Directory.Exists(_location + "/ItemData"))
                Directory.Delete(_location + "/ItemData", true);

            Directory.CreateDirectory(_location + "/ItemData");

            _location += "/ItemData";

            foreach (Item _item in _itemData.items)
            {
                ConvertToJson(_item, _location);
            }
        }

        if (GUILayout.Button("Load"))
        {
            _itemData.items.Clear();
            string _location = AssetDatabase.GetAssetPath(_itemData);

            _location = _location.Substring(0, _location.LastIndexOf("/", StringComparison.Ordinal));

            _location += "/ItemData";

            foreach (string _file in Directory.GetFiles(_location))
            {
                //안에 있는 모든 파일 전부 탐색하므로 json 파일만 탐색
                if (!_file.EndsWith(".json"))
                    continue;
                
                string _json = File.ReadAllText(_file);

                try //Serialize가 가능한 경우만 추가
                {
                    Item _item = JsonConvert.DeserializeObject<Item>(_json, _converters);
                    _itemData.items.Add(_item);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    Debug.Log($"<color=red>ItemDataLoader</color> : {_file} is not valid json file!");
                }
            }
        }
    }

    private void ConvertToJson(Item _itemData, string _targetLocation)
    {
        string _json = JsonConvert.SerializeObject(_itemData, _converters);

        File.WriteAllText(_targetLocation + "/" + _itemData.id + ". " + _itemData.name + ".json", _json);
    }
}

public class SpriteConverter : JsonConverter<Sprite>
{
    public override void WriteJson(JsonWriter _writer, Sprite _value, JsonSerializer _serializer)
    {
        _writer.WriteValue(AssetDatabase.GetAssetPath(_value));
    }

    public override Sprite ReadJson(JsonReader _reader, Type _objectType, Sprite _existingValue, bool _hasExistingValue, JsonSerializer _serializer)
    {
        Sprite _sprite = Addressables.LoadAssetAsync<Sprite>(_reader.Value).WaitForCompletion();
        return _sprite;
    }
}

public class EnumTypeConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override void WriteJson(JsonWriter _writer, T _value, JsonSerializer _serializer)
    {
        _writer.WriteValue(_value.ToString());
    }

    public override T ReadJson(JsonReader _reader, Type _objectType, T _existingValue, bool _hasExistingValue, JsonSerializer _serializer)
    {
        if (_reader.Value is null ||
            !Enum.TryParse(_reader.Value.ToString(), out T _result))
            return Enum.GetValues(typeof(T)).Cast<T>().First();

        return _result;
    }
}