namespace SimpleBankAPI.Application.Interfaces;

public interface IFactory<out T>
{
    T? this[string key] { get; }

    bool ContainsKey(string key);
}