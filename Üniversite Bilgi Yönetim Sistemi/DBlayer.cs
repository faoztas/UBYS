using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.Collections;

namespace WindowsFormsApplication1
{
public class DBLayer
{
        
    public static string connectionstring;


    private static SqlConnection Bağlan()
    {

        SqlConnection baglanti = new SqlConnection("Data Source=mssql3.gear.host;Initial Catalog=ubys;Persist Security Info=True;User ID=ubys;Password=Ks61?S8YQT?4");
        if (baglanti.State != System.Data.ConnectionState.Open)
        {
            try
            {
                baglanti.Open();
            }
            catch
            {
                MessageBox.Show("Bağlantı Kurulamadı", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        return baglanti;
    }

    public static bool IslemYap(string sql)
    {
        SqlCommand kmt = new SqlCommand(sql, Bağlan());
        if (kmt.ExecuteNonQuery() == 1)
            return true;
        else
            return false;
    }

    public static object FunctionÇağır(string sql)
    {
        SqlCommand kmt = new SqlCommand(sql, Bağlan());
        return kmt.ExecuteScalar();
    }

    public static DataTable TabloSorgula(string sql)
    {
        SqlDataAdapter adap = new SqlDataAdapter(sql, Bağlan());
        DataSet data = new DataSet();
        adap.Fill(data);
        return data.Tables[0];
    }

        public static DataRow SatırSorgula(string sql)
    {
        SqlDataAdapter adap = new SqlDataAdapter(sql, Bağlan());
        DataSet data = new DataSet();
        adap.Fill(data);
        return data.Tables[0].Rows[0];
    }
}
}