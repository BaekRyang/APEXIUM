using System.Collections.Generic;

public static class UpdatableUIElements
{
    private static Dictionary<string, UIElementUpdater> UIElementUpdaters = new();

    public static void AssignToGlobal(this UIElementUpdater _updater) =>
        UIElementUpdaters[_updater.gameObject.name] = _updater;

    public static void UpdateValue(string _name, object _current, object _max = null) =>
        UIElementUpdaters[_name].UpdateValue(_current, _max);
}