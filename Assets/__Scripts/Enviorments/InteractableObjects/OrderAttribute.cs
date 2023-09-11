public class OrderAttribute : System.Attribute
{
    public readonly int order;

    public OrderAttribute(int _order)
    {
        order = _order;
    }
}
