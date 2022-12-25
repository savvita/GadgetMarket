namespace GadgetMarket.Model
{
    public class Gadget
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public int? OwnerId { get; set; }

        public User? Owner { get; set; }
    }
}
