using Dotland.FileSyncHub.Domain.Enums;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = Enum.GetValues<DocumentCategory>()
            .Select(c => new CategoryDto
            {
                Id = c.ToString(),
                Name = c.ToString(),
                // Map colors/icons if needed, for now defaults
                Color = GetColor(c),
                Icon = GetIcon(c)
            })
            .ToList();

        return Task.FromResult(categories);
    }

    private string GetColor(DocumentCategory category) => category switch
    {
        DocumentCategory.Contracts => "orange",
        DocumentCategory.Invoices => "blue",
        DocumentCategory.HumanResources => "purple",
        DocumentCategory.Legal => "red",
        _ => "gray"
    };

    private string GetIcon(DocumentCategory category) => category switch
    {
        DocumentCategory.Contracts => "pi pi-file",
        DocumentCategory.Invoices => "pi pi-dollar",
        DocumentCategory.HumanResources => "pi pi-users",
        _ => "pi pi-folder"
    };
}
