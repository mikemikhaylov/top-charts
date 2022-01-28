using System;

namespace TopCharts.Domain.Model.Api
{
    public class Data
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public Author Author { get; set; }
        public SubSite SubSite { get; set; }
        public int Date { get; set; }
        public Counters Counters { get; set; }
        public int HitsCount { get; set; }
        public Likes Likes { get; set; }
        public Block[] Blocks { get; set; }
        public string Title { get; set; }
    }
}