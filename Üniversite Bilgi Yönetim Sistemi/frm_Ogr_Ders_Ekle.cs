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
    public partial class frm_Ogr_Ders_Ekle : Form
    {
        public frm_Ogr_Ders_Ekle()
        {
            InitializeComponent();
        }

        public static string str;

        private void frm_Ogr_Ders_Ekle_Load(object sender, EventArgs e)
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
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox4.Text = "";
            if (String.IsNullOrEmpty(textBox3.Text)) {}
            else
            {
                str = @"SELECT DersAdi
                        FROM Ders_Bilgi
                        WHERE DersNo LIKE '%" + textBox3.Text + "%'";
                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    textBox4.Text = DBLayer.FunctionÇağır(str).ToString();
                }
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = "";
            if (String.IsNullOrEmpty(textBox1.Text)){}
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

        private void btn_Kaydet_Click(object sender, EventArgs e)
        {
            str = @"INSERT INTO Ogrenci_Ders(Ogr_ID,Ders_ID)
                    (SELECT Ogr_ID,Ders_ID
                    FROM Ogrenci_Kaydi,Ders_Bilgi
                    WHERE No='" + textBox1.Text + @"'
                    AND DersNo='" + textBox3.Text + "')";
            DBLayer.IslemYap(str);
            str = @"INSERT INTO Ogrenci_Not(Ogr_Ders_ID, Ogr_ID, Ders_ID)
                    (SELECT Ogr_Ders_ID, Ogr_ID, Ders_ID
                    FROM Ogrenci_Ders
                    WHERE Ogr_Ders_ID =
                    (SELECT Ogr_Ders_ID
                    FROM Ogrenci_Ders
                    WHERE Ogr_ID =
                    (SELECT Ogr_ID
                    FROM Ogrenci_Kaydi
                    WHERE No = '" + textBox1.Text + @"')
                    AND Ders_ID =
                    (SELECT Ders_ID
                    FROM Ders_Bilgi
                    WHERE DersNo = '" + textBox3.Text + "')))";
            DBLayer.IslemYap(str);
            textBox1.Text = ""; textBox3.Text = "";
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

        private void frm_Ogr_Ders_Ekle_FormClosed(object sender, FormClosedEventArgs e)
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
    }
}
