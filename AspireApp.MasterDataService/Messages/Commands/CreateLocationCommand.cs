namespace AspireApp.MasterDataService.Messages.Commands;

public record CreateLocationCommand(string Name, string Type, int? ParentId); 