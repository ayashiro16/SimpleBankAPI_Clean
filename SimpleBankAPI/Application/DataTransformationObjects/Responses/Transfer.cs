namespace SimpleBankAPI.Application.DataTransformationObjects.Responses;

public record Transfer(Domain.Entities.Account? Sender, Domain.Entities.Account? Recipient);