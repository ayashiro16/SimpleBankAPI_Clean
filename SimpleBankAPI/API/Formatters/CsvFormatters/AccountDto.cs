using System.Text;

namespace SimpleBankAPI.API.Formatters.CsvFormatters;

public class AccountDto : Interfaces.IFormatter
{
    public void Format(StringBuilder buffer, object item)
    {
        var account = (Application.DataTransformationObjects.Responses.AccountDto)item;
        buffer.AppendLine($"\"{account.Id}\",\"{account.Name}\",{account.Balance}");
    }
}