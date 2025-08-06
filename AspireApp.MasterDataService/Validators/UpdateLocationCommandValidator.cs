using FluentValidation;
using AspireApp.MasterDataService.Messages.Commands;

namespace AspireApp.MasterDataService.Validators;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Location ID must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Location name is required")
            .MaximumLength(100).WithMessage("Location name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Location name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Location type is required")
            .MaximumLength(50).WithMessage("Location type cannot exceed 50 characters")
            .Must(BeValidLocationType).WithMessage("Location type must be one of: Site, Building, Floor, Room, Area");

        RuleFor(x => x.ParentId)
            .GreaterThan(0).When(x => x.ParentId.HasValue)
            .WithMessage("Parent ID must be greater than 0 when specified")
            .NotEqual(x => x.Id).WithMessage("Location cannot be its own parent");
    }

    private static bool BeValidLocationType(string type)
    {
        var validTypes = new[] { "Site", "Building", "Floor", "Room", "Area" };
        return validTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
    }
} 