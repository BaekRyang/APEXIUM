using UnityEngine;

public class Item
{
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
