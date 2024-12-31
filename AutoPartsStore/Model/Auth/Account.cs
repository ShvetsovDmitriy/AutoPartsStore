namespace AutoPartsStore.Model.Auth
{
    public class Account
    {
        public Guid ID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string PasswordHash { get; set; }


    }
}
