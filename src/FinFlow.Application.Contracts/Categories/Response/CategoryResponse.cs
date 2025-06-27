using FinFlow.Domain.Enum;

namespace FinFlow.Application.Contracts.Categories.Response;

public record CategoryResponse(string Name, CategoryType Type);