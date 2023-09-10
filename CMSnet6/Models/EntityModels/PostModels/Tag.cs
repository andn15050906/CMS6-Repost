using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSnet6.Models.EntityModels.PostModels
{
    public class Tag
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR(15)")]
        public string Name { get; set; }






        public ICollection<PostTag> PostTags { get; set; }
    }
}
