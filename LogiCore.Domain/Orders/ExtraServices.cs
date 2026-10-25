namespace LogiCore.Domain.Orders;

[Flags]
public enum ExtraServices
{
    None = 0,
    Insurance = 1,
    Urgent = 2,
    FragilePackaging = 4
}
