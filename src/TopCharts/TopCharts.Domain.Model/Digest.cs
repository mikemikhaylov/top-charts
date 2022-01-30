using TopCharts.Domain.Model.Api;

namespace TopCharts.Domain.Model
{
    public class Digest
    {
        public string Name { get; set; }
        public SubSiteType SubSiteType { get; set; }
        public int TopSize { get; set; }
        public Item[] ByLikes { get; set; }
        public Item[] ByViews { get; set; }
        public Item[] ByComments { get; set; }
        public Item[] ByBookmarks { get; set; }
        public Item[] ByReposts { get; set; }
        public int TotalLikes { get; set; }
        public int TotalViews { get; set; }
        public int TotalComments { get; set; }
        public int TotalReposts { get; set; }
        public int TotalBookmarks { get; set; }
    }
}