using System.Text;
using SimpleBankAPI.API.Formatters.Interfaces;

namespace SimpleBankAPI.API.Formatters.CsvFormatters;

public class ConvertCurrency : IFormatter
{
    public void Format(StringBuilder buffer, object item)
    {
        var currency = (Application.DataTransformationObjects.Responses.ConvertCurrency)item;
        buffer.AppendLine($"\"{currency.CurrencyCode}\",{currency.ConvertedBalance}");
    }
}