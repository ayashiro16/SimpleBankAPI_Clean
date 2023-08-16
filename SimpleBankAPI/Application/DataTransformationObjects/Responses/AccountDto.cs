namespace SimpleBankAPI.Application.DataTransformationObjects.Responses;

public record AccountDto(Guid Id, string Name, decimal Balance);