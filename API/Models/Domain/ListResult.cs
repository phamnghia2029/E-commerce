using API.Utils;

namespace API.Models.Domain;

public class ListResult<T>
{
    public ListResult(int total, int page, int pages, int size, bool ascending, string? orderBy, int lowerPage, int upperPage, bool hasNextPageSection, bool hasPrevPageSection)
    {
        Total = total;
        Page = page;
        Pages = pages;
        Size = size;
        Ascending = ascending;
        OrderBy = orderBy;
        LowerPage = lowerPage;
        UpperPage = upperPage;
        HasNextPageSection = hasNextPageSection;
        HasPrevPageSection = hasPrevPageSection;
    }

    public List<T> Content { get; set; } = new();
    public int Total { get; set; } = 0;
    public int Page { get; set; } = 1;
    public int Pages { get; set; } = 1;
    public int Size { get; set; } = 10;
    public bool Ascending { get; set; } = false;
    public string? OrderBy { get; set; }

    public int LowerPage { get; set; }
    public int UpperPage { get; set; }
    public bool HasNextPageSection { get; set; } = false;
    public bool HasPrevPageSection { get; set; } = false;

    private int PageRange { get; set; } = 10;


    public ListResult()
    {
    }

    public ListResult(List<T> content, int total, int page, int size, bool asc, string? orderBy = null, int pageRange = 10)
    {
        Content = content;
        Total = total;
        Page = page;
        Size = size;
        OrderBy = orderBy;
        Ascending = asc;
        Pages = Numbers.GetCeilOnDivide(total, size);
        PageRange = pageRange;

        int currentPageSection = (page - 1) / pageRange + 1;
        int start = (currentPageSection - 1) * pageRange + 1;
        int end = Pages;

        if (pageRange < Pages)
        {
            end = pageRange * currentPageSection;
            if (end > Pages)
            {
                end = Pages;
            }
        }
        LowerPage = start;
        UpperPage = end;

        HasPrevPageSection = LowerPage > 1;
        HasNextPageSection = UpperPage < Pages;
    }

    public ListResult<TR> WithContent<TR>(List<TR> content)
    {
        return new ListResult<TR>(content, this.Total, this.Page, this.Size, this.Ascending, this.OrderBy, this.PageRange);
    }

}