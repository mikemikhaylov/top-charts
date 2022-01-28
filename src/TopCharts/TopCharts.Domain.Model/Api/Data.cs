using System;

namespace TopCharts.Domain.Model.Api
{
    public class Data
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public Author Author { get; set; }
        public SubSite SubSite { get; set; }
        public DateTime Date { get; set; }
    }
}