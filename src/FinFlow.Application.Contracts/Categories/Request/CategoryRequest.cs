using FinFlow.Domain.Enum;

namespace FinFlow.Application.Contracts.Categories.Request;

public record CategoryRequest(
    string Name, 
    CategoryType Type, 
    string Color, 
    string Icon
);