namespace SimpleBankAPI.API.RequestModels;

public record TransferFunds(Guid SenderId, Guid RecipientId, decimal Amount);