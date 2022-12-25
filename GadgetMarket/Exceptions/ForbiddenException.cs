namespace GadgetMarket.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException() : base("Acces denied")
        {

        }
    }
}
