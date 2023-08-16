using System.Text;

namespace SimpleBankAPI.API.Formatters.Interfaces;

public interface IFormatter
{
    void Format(StringBuilder buffer, object item);
}