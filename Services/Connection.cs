using Microsoft.AspNetCore.DataProtection;

namespace Jordnaer.Services
{
    public abstract class Connection
    {
        protected string connectionString;
        public IConfiguration Configuration { get; set; }
        public Connection(IConfiguration configuration)
        {
            Configuration = configuration;
            connectionString = Secret.MyConnectionString; /*Configuration["ConnectionStrings:DefaultConnection"];*/
        }
        public Connection(string connectionString)
        {
            Configuration = null;
            this.connectionString = connectionString;
        }
    }
}
