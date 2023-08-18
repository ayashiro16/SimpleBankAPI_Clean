using System.Text;

namespace SimpleBankAPI.API.Formatters.CsvFormatters;

public class Transfer : Interfaces.IFormatter
{
    public void Format(StringBuilder buffer, object item)
    {
        var transfer = (Application.DataTransformationObjects.Responses.Transfer)item;
        buffer.AppendLine($"\"{transfer.Sender?.Id}\",\"{transfer.Sender?.Name}\",{transfer.Sender?.Balance}");
        buffer.AppendLine($"\"{transfer.Recipient?.Id}\",\"{transfer.Recipient?.Name}\",{transfer.Recipient?.Balance}");
    }
}