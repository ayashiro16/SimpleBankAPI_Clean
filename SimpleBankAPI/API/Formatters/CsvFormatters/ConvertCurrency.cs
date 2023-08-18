using System.Text;

namespace SimpleBankAPI.API.Formatters.CsvFormatters;

public class ConvertCurrency : Interfaces.IFormatter
{
    public void Format(StringBuilder buffer, object item)
    {
        var currency = (Application.DataTransformationObjects.Responses.ConvertCurrency)item;
        buffer.AppendLine($"\"{currency.CurrencyCode}\",{currency.ConvertedBalance}");
    }
}