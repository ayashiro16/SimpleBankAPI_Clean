using System.Text;
using SimpleBankAPI.API.Formatters.Interfaces;

namespace SimpleBankAPI.API.Formatters.CsvFormatters;

public class Transfer : IFormatter
{
    public void Format(StringBuilder buffer, object item)
    {
        var transfer = (Application.DataTransformationObjects.Responses.Transfer)item;
        buffer.AppendLine($"\"{transfer.Sender?.Id}\",\"{transfer.Sender?.Name}\",{transfer.Sender?.Balance}");
        buffer.AppendLine($"\"{transfer.Recipient?.Id}\",\"{transfer.Recipient?.Name}\",{transfer.Recipient?.Balance}");
    }
}