namespace HubtelWallets.API.DTOs;

public record WalletResponseDto(Guid Id, string Name, string AccountNumber, string AccountScheme, string Type, string Owner, DateTime CreatedAt);



