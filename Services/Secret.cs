namespace Jordnaer.Services
{
    public class Secret
    {

        private static string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ProjektJordnaerDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static string MyConnectionString
        {
            get { return _connectionString; }
        }
    }
}
