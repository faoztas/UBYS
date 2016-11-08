using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace WindowsFormsApplication1
{
    public partial class frm_Ogr_Ders_Duzenle : Form
    {
        public frm_Ogr_Ders_Duzenle()
        {
            InitializeComponent();
        }

        private void btn_Kaydet_Click(object sender, EventArgs e)
        {
            str = @"UPDATE Ogrenci_Ders
                    SET Ders_ID = '" + ID.ToString() + @"'
                    FROM Ogrenci_Kaydi OK
                    Inner Join Ogrenci_Ders OD ON OK.Ogr_ID = OD.Ogr_ID
                    Inner Join Ders_Bilgi DB ON OD.Ders_ID = DB.Ders_ID
                    WHERE OK.No = '" + textBox1.Text + @"'
                    AND DB.DersNo = '" + DERSNO.ToString() + "'";
            DBLayer.IslemYap(str);
            str = @"UPDATE Ogrenci_Not
                    SET Ders_ID = '" + ID.ToString() + @"'
                    FROM Ogrenci_Kaydi OK
                    Inner Join Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID Inner
                    Join Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID
                    WHERE OK.No = '" + textBox1.Text + @"'
                    AND DB.DersNo = '" + DERSNO.ToString() + "'";
            DBLayer.IslemYap(str);
            ID = "0"; DERSNO = "0";

            str = @"SELECT Ogrenci_Kaydi.No AS 'Ogr. No',
                    Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                    Ders_Bilgi.DersNo AS 'Ders Kodu',
                    Ders_Bilgi.DersAdi AS 'Ders Adi',
                    Ders_Bilgi.DersHoca AS 'Öğretim Görevlisi'
                    FROM Ogrenci_Ders
                    INNER JOIN Ogrenci_Kaydi ON Ogrenci_Ders.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                    INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Ders.Ders_ID";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = "";
            if (String.IsNullOrEmpty(textBox1.Text))
            {
            }
            else
            {
                str = @"Select AdSoyad
                        from Ogrenci_Kaydi
                        where No Like '%" + textBox1.Text + "%'";
                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    textBox2.Text = DBLayer.FunctionÇağır(str).ToString();
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox4.Text = "";
            if (String.IsNullOrEmpty(textBox3.Text))
            {
            }
            else
            {
                str = @"SELECT DersAdi
                        FROM Ders_Bilgi
                        WHERE DersNo LIKE '%" + textBox3.Text + "%'";
                string sql = @"SELECT Ders_ID
                               FROM Ders_Bilgi
                               WHERE DersNo LIKE '%" + textBox3.Text + "%'";
                if (DBLayer.TabloSorgula(str).Rows.Count > 0 && DBLayer.TabloSorgula(sql).Rows.Count > 0)
                {
                    textBox4.Text = DBLayer.FunctionÇağır(str).ToString();
                    ID = DBLayer.FunctionÇağır(sql).ToString();
                }
            }
        }

        private void frm_Ogr_Ders_Duzenle_Load(object sender, EventArgs e)
        {
            str = @"SELECT Ogrenci_Kaydi.No AS 'Ogr. No',
                    Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                    Ders_Bilgi.DersNo AS 'Ders Kodu',
                    Ders_Bilgi.DersAdi AS 'Ders Adi',
                    Ders_Bilgi.DersHoca AS 'Öğretim Görevlisi'
                    FROM Ogrenci_Ders
                    INNER JOIN Ogrenci_Kaydi ON Ogrenci_Ders.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                    INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Ders.Ders_ID";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        public static string ID="0",DERSNO="0",str;

        private void frm_Ogr_Ders_Duzenle_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
            frm_Ogr_Ders_Index frm = (frm_Ogr_Ders_Index)Application.OpenForms["frm_Ogr_Ders_Index"];
            str = @"SELECT Ogrenci_Kaydi.No AS 'Ogr. No',
                    Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                    Ders_Bilgi.DersNo AS 'Ders Kodu',
                    Ders_Bilgi.DersAdi AS 'Ders Adi',
                    Ders_Bilgi.DersHoca AS 'Öğretim Görevlisi'
                    FROM Ogrenci_Ders
                    INNER JOIN Ogrenci_Kaydi ON Ogrenci_Ders.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                    INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Ders.Ders_ID";
            frm.dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int num = dataGridView1.SelectedCells[0].RowIndex;
            textBox1.Text = dataGridView1.Rows[num].Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.Rows[num].Cells[1].Value.ToString();
            DERSNO = dataGridView1.Rows[num].Cells[2].Value.ToString();
        }
    }
}
