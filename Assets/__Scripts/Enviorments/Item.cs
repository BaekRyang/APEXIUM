using System;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic
    }
    
    public int id;
    public static Item ToItem(int _id)
    {
        return new Item().SetID(_id);
    }

    private Item SetID(int _id)
    {
        id = _id;
        return this;
    }
}
