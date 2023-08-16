using System.Collections.Immutable;
using SimpleBankAPI.Application.Interfaces;
using IValidator = SimpleBankAPI.Application.Validators.Interfaces.IValidator;

namespace SimpleBankAPI.API.Factories;

public class ValidatorFactory : IFactory<IValidator?>
{
    private readonly IReadOnlyDictionary<string, IValidator> _validators;
    
    public IValidator? this[string key] => _validators!.GetValueOrDefault(key);

    public ValidatorFactory()
    {
        var validatorType = typeof(IValidator);
        _validators = validatorType.Assembly.ExportedTypes
            .Where(x => validatorType.IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
            .Select(x => Activator.CreateInstance(x))
            .Cast<IValidator>()
            .ToImmutableDictionary(x => x.GetType().Name, x => x);
    }
    
    public bool ContainsKey(string key) => _validators.ContainsKey(key);
}