namespace GadgetMarket.Exceptions
{
    public class UserEmailConflictException : Exception
    {
        public UserEmailConflictException() : base("Such email is already registered")
        {

        }
    }
}
