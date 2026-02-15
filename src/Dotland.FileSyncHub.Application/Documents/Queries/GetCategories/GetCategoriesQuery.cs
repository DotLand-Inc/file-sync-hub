using Dotland.FileSyncHub.Domain.Enums;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class CategoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "blue"; // Default color
    public string Icon { get; set; } = "pi pi-folder"; // Default icon
}
