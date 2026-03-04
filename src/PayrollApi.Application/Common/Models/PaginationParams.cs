using System.ComponentModel.DataAnnotations;

namespace PayrollApi.Application.Common.Models;

public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 10 : value;
    }

    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
    public string? Search { get; set; }
}
