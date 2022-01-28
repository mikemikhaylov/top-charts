namespace TopCharts.Domain.Model.Api
{
    public class Result
    {
        public Item[] Type { get; set; }
        public int LastId { get; set; }
        public int LastSortingValue { get; set; }
    }
}