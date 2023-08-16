namespace SimpleBankAPI.Domain.Entities;

public class Account
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Balance { get; set; }
}