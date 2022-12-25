namespace GadgetMarket.Exceptions
{
    public class CategoryNotFoundException : Exception
    {
        public int Id { get; }

        public CategoryNotFoundException(int id) : base($"Category with id {id} is not found")
        {
            Id = id;
        }
    }
}
