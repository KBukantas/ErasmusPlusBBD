namespace ErasmusPlus.Models
{
    public class UserError
    {
        public string Reason { get; set; }

        public UserError(string reason)
        {
            Reason = reason;
        }
    }
}