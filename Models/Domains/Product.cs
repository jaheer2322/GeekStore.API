using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace GeekStore.API.Models.Domains
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? Review { get; set; }
        public Guid TierId { get; set; }
        public Guid CategoryId {  get; set; }

        [Column(TypeName = "vector(384)")]
        public Vector? Embedding { get; set; }

        //Navigation props
        public Tier Tier { get; set; }
        public Category Category { get; set; }
    }
}
