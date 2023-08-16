using System.Text;
using SimpleBankAPI.API.Formatters.Interfaces;

namespace SimpleBankAPI.API.Formatters.CsvFormatters;

public class AccountBalance : IFormatter
{
    public void Format(StringBuilder buffer, object item) => buffer.Append($"{((Application.DataTransformationObjects.Responses.AccountBalance)item).Balance}");
}