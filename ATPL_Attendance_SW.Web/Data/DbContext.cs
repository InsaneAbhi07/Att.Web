namespace ATPL_Attendance_SW.Web.Data
{
    public class DbContext
    {
        public string ConnectionString { get; set; }
        public DbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
