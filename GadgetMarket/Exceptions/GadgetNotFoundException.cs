namespace GadgetMarket.Exceptions
{
    public class GadgetNotFoundException : Exception
    {
        public int Id { get; }

        public GadgetNotFoundException(int id) : base($"Gadget with id {id} is not found")
        {
            Id = id;
        }
    }
}
