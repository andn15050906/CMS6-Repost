using System.ComponentModel.DataAnnotations;

namespace CMSnet6.Models.EntityModels.MetaModels
{
    public class Statistics
    {
        [Key]
        public DateTime Date { get; set; }

        public int VisitCount { get; set; }
    }
}
