using System;
using TopCharts.Domain.Model;

namespace TopCharts.Domain.Services
{
    public class PostingOptions
    {
        public PostingType Type { get; set; }
        public DateTime InitialDate { get; set; }
        public Site Site { get; set; }
    }
}