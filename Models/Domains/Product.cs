namespace GeekStore.API.Models.Domains
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public Guid TierId { get; set; }
        public Guid CategoryId {  get; set; }

        //Navigation props
        public Tier Tier { get; set; }
        public Category Category { get; set; }
    }
}
