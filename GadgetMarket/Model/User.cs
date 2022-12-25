namespace GadgetMarket.Model
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public List<Gadget> Gadgets { get; } = new List<Gadget>();
    }
}
