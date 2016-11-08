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
    public partial class frm_Devam_Ekle : Form
    {
        public frm_Devam_Ekle()
        {
            InitializeComponent();
        }

        public static string str;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = "";
            if (String.IsNullOrEmpty(textBox1.Text))
            {
            }
            else
            {
                str = @"Select AdSoyad from Ogrenci_Kaydi
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
                str = @"SELECT DersAdi FROM Ders_Bilgi
                        WHERE DersNo LIKE '%" + textBox3.Text + "%'";
                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    textBox4.Text = DBLayer.FunctionÇağır(str).ToString();
                }
            }
        }

        private void btn_Kaydet_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox4.Text) || String.IsNullOrEmpty(textBox5.Text) || String.IsNullOrEmpty(textBox6.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı");
            }
            else
            {
                str = @"Update Ogrenci_Ders SET DersToplam = '" + textBox5.Text + @"',
                        DersDevamsiz = '" + textBox6.Text + @"'
                        FROM Ogrenci_Kaydi OK
                        Inner Join Ogrenci_Ders OD ON OK.Ogr_ID = OD.Ogr_ID
                        Inner Join Ders_Bilgi DB ON OD.Ders_ID = DB.Ders_ID
                        WHERE OK.No = '" + textBox1.Text + @"'
                        and DB.DersNo = '" + textBox3.Text + "'";
                DBLayer.IslemYap(str);

                str = @"SELECT Ogrenci_Ders.Ogr_Ders_ID AS 'ID',
                        Ogrenci_Kaydi.No AS 'Ogr No',
                        Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                        Ders_Bilgi.DersNo AS 'Ders Kodu',
                        Ders_Bilgi.DersAdi AS 'Ders Adı',
                        Ders_Bilgi.DersHoca AS 'Öğretim Görevlisi',
                        Ogrenci_Ders.DersToplam AS 'Toplam Ders',
                        Ogrenci_Ders.DersDevamsiz AS 'Devamsız'
                        FROM Ogrenci_Ders
                        INNER JOIN Ogrenci_Kaydi ON Ogrenci_Ders.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                        INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Ders.Ders_ID";
                dataGridView1.DataSource = DBLayer.TabloSorgula(str);
            }
        }

        private void frm_Devam_Ekle_Load(object sender, EventArgs e)
        {
            str = @"SELECT Ogrenci_Ders.Ogr_Ders_ID AS 'ID',
                        Ogrenci_Kaydi.No AS 'Ogr No',
                        Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                        Ders_Bilgi.DersNo AS 'Ders Kodu',
                        Ders_Bilgi.DersAdi AS 'Ders Adı',
                        Ders_Bilgi.DersHoca AS 'Öğretim Görevlisi',
                        Ogrenci_Ders.DersToplam AS 'Toplam Ders',
                        Ogrenci_Ders.DersDevamsiz AS 'Devamsız'
                        FROM Ogrenci_Ders
                        INNER JOIN Ogrenci_Kaydi ON Ogrenci_Ders.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                        INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Ders.Ders_ID";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int num = dataGridView1.SelectedCells[0].RowIndex;
            textBox1.Text = dataGridView1.Rows[num].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[num].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.Rows[num].Cells[3].Value.ToString();
            textBox4.Text = dataGridView1.Rows[num].Cells[4].Value.ToString();
            textBox5.Text = dataGridView1.Rows[num].Cells[6].Value.ToString();
            textBox6.Text = dataGridView1.Rows[num].Cells[7].Value.ToString();
        }

        private void frm_Devam_Ekle_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
            frm_Devam_Index frm = (frm_Devam_Index)Application.OpenForms["frm_Devam_Index"];
            str = @"SELECT Ogrenci_Ders.Ogr_Ders_ID AS 'ID',
                           Ogrenci_Kaydi.No AS 'Ogr No',
                           Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                           Ders_Bilgi.DersNo AS 'Ders Kodu',
                           Ders_Bilgi.DersAdi AS 'Ders Adı',
                           Ders_Bilgi.DersHoca AS 'Öğretim Görevlisi',
                           Ogrenci_Ders.DersToplam AS 'Toplam Ders',
                           Ogrenci_Ders.DersDevamsiz AS 'Devamsız'
                           FROM Ogrenci_Ders
                           INNER JOIN Ogrenci_Kaydi ON Ogrenci_Ders.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                           INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Ders.Ders_ID";
            frm.dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar); //Yalnızca Harf Girişi
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi
        }
    }
}
