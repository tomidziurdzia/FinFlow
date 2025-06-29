using FinFlow.Domain.Enum;

namespace FinFlow.Application.Contracts.Categories.Response;

public record CategoryResponse(
    string Id,
    string Name,
    CategoryType Type,
    string Color,
    string Icon
);