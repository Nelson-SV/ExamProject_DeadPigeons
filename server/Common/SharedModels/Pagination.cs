namespace Common.SharedModels;

public class Pagination
{
    public int CurrentPageItems { get; set; } = 10;
    public int CurrentPage { get; set; }
    public Int32? NextPage { get; set; }
    public bool HasNext { get; set; }
    public int TotalItems { get; set; }
}