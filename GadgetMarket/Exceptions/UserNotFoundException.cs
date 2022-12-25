namespace GadgetMarket.Exceptions
{
    public class UserNotFoundException : Exception
    {

        public int Id { get; }

        public UserNotFoundException(int id) : base($"User with id {id} is not found")
        {
            Id = id;
        }
    }
}
