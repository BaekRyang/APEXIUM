interface Subject
{
    void RegisterObserver(IObserver p_o);
    void RemoveObserver(IObserver p_o);
    void NotifyObservers();
}

interface IObserver
{
    void Notified(NotifyTypes p_type, object p_value);
}

public enum NotifyTypes
{
    Health,
    MaxHealth,
    Exp,
    Level,
    CoolDown,
    Resource
}