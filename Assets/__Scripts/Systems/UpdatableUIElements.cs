using System.Collections.Generic;

public class UpdatableUIElements
{
    private static Dictionary<string, UIElementUpdater> UIElementUpdaters = new();

    public static void Assign(string _name, UIElementUpdater _updater) =>
        UIElementUpdaters[_name] = _updater;

    public static void UpdateValue(string _name, object _current, object _max = null) =>
        UIElementUpdaters[_name].UpdateValue(_current, _max);
}