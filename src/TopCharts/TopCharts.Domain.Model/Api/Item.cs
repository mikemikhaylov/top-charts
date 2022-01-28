namespace TopCharts.Domain.Model.Api
{
    public class Item
    {
        public string Id { get; set; }
        public Site Site { get; set; }
        public string Type { get; set; }
        public Data Data { get; set; }
    }
}