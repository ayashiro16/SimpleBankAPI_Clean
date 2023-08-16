using System.Collections.Immutable;
using SimpleBankAPI.Application.Interfaces;
using IFormatter = SimpleBankAPI.API.Formatters.Interfaces.IFormatter;

namespace SimpleBankAPI.API.Formatters;

public class FormatterFactory : IFactory<IFormatter>
{
    private readonly IReadOnlyDictionary<string, IFormatter> _formatters;
    
    public IFormatter? this[string key] => _formatters!.GetValueOrDefault(key);

    public FormatterFactory()
    {
        var formatterType = typeof(IFormatter);
        _formatters = formatterType.Assembly.ExportedTypes
            .Where(x => formatterType.IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
            .Select(x => Activator.CreateInstance(x))
            .Cast<IFormatter>()
            .ToImmutableDictionary(x => x.GetType().Name, x => x);
    }

    public bool ContainsKey(string key) => _formatters.ContainsKey(key);
}