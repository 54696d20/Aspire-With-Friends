namespace AspireApp.MasterDataService.Messages.Commands;

public record UpdateLocationCommand(int Id, string Name, string Type, int? ParentId); 