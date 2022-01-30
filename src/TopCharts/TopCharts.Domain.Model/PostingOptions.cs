using System;

namespace TopCharts.Domain.Model
{
    public class PostingOptions
    {
        public PostingType Type { get; set; }
        public DateTime InitialDate { get; set; }
        public Site Site { get; set; }
        public string TelegraphToken { get; set; }
    }
}