using Microsoft.Data.SqlClient;
using System.Data;


public class DataUtility
{
    private readonly string _con;

    public DataUtility(IConfiguration config)
    {
        _con = config.GetConnectionString("DefaultConnection");
    }
    
    public DataTable GetDataTable(string spName, SqlParameter[] prms)
    {
        using SqlConnection con = new SqlConnection(_con);
        using SqlCommand cmd = new SqlCommand(spName, con);

        cmd.CommandType = CommandType.StoredProcedure;
        if (prms != null && prms.Length > 0)
        {
            cmd.Parameters.AddRange(prms);
        }
        using SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    public DataTable GetDataTableByQuery(string query, SqlParameter[] prms)
    {
        using SqlConnection con = new SqlConnection(_con);
        using SqlCommand cmd = new SqlCommand(query, con);

        cmd.CommandType = CommandType.Text;

        if (prms != null && prms.Length > 0)
        {
            cmd.Parameters.AddRange(prms);
        }

        using SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    public int Execute(string spName, SqlParameter[] prms)
    {
        using SqlConnection con = new SqlConnection(_con);
        using SqlCommand cmd = new SqlCommand(spName, con);

        cmd.CommandType = CommandType.StoredProcedure;

        if (prms != null && prms.Length > 0)
        {
            cmd.Parameters.AddRange(prms);
        }


        con.Open();
        return cmd.ExecuteNonQuery();
    }

    public object ExecuteScalar(string query)
    {
        using (SqlConnection con = new SqlConnection(_con))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
 
}
