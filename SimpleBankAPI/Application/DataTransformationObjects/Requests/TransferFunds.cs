namespace SimpleBankAPI.Application.DataTransformationObjects.Requests;

public record TransferFunds(Guid SenderId, Guid RecipientId, decimal Amount);