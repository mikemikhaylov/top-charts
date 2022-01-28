namespace TopCharts.Domain.Model.Api
{
    public class Block
    {
        public string Type { get; set; }
        public bool Hidden { get; set; }
        public BlockData Data { get; set; }
    }
}