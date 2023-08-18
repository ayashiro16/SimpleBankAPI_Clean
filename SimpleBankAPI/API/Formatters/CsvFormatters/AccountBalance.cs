using System.Text;

namespace SimpleBankAPI.API.Formatters.CsvFormatters;

public class AccountBalance : Interfaces.IFormatter
{
    public void Format(StringBuilder buffer, object item) => buffer.Append($"{((Application.DataTransformationObjects.Responses.AccountBalance)item).Balance}");
}