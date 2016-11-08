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
    public partial class frm_KayıtDüzenle : Form
    {
        public frm_KayıtDüzenle()
        {
            InitializeComponent();
        }

        private void btn_Duzenle_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox3.Text) || String.IsNullOrEmpty(textBox4.Text) || String.IsNullOrEmpty(textBox5.Text) || String.IsNullOrEmpty(comboBox1.Text) || String.IsNullOrEmpty(comboBox2.Text) || String.IsNullOrEmpty(comboBox3.Text) || string.IsNullOrEmpty(ID.ToString()))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı");
            }

            else
            {
                str = @"SELECT Ogr_ID FROM Ogrenci_Kaydi
                        WHERE Ogr_ID ='" + ID.ToString() + "'";
                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    str = @"Update Ogrenci_Kaydi set No='" + textBox1.Text.ToString() + @"',
                           TC='" + textBox2.Text.ToString() + @"',
                           AdSoyad='" + textBox3.Text.ToString() + @"',
                           Telefon='" + textBox4.Text.ToString() + @"',
                           Sinif='" + comboBox1.Text.ToString() + @"',
                           Email='" + textBox5.Text.ToString() + @"',
                           Fakulte='" + comboBox2.Text.ToString() + @"',
                           Bolum='" + comboBox3.Text.ToString() + @"',
                           Tarih='" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + @"'
                           where Ogr_ID=" + ID.ToString() + "";
                    DBLayer.IslemYap(str);
                    str = @"Select Ogr_ID AS 'ID',
                            No AS 'Ogr No',
                            TC AS 'TC Kimlik No',
                            AdSoyad AS 'Ad Soyad',
                            Telefon AS 'Telefon',
                            Sinif AS 'Sınıf',
                            Email AS 'E-Posta',
                            Fakulte AS 'Fakülte',
                            Bolum AS 'Bölüm',
                            Tarih AS 'Kayıt Tarihi'
                            from Ogrenci_Kaydi";
                    dataGridView1.DataSource = DBLayer.TabloSorgula(str);
                }
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    comboBox1.Text=null;
                    textBox5.Clear();
                    comboBox2.Text = null;
                    comboBox3.Text = null;
                    ID = "0";
            }
        }

        private void frm_KayıtDüzenle_Load(object sender, EventArgs e)
        {
            str = @"Select Ogr_ID AS 'ID',
                            No AS 'Ogr No',
                            TC AS 'TC Kimlik No',
                            AdSoyad AS 'Ad Soyad',
                            Telefon AS 'Telefon',
                            Sinif AS 'Sınıf',
                            Email AS 'E-Posta',
                            Fakulte AS 'Fakülte',
                            Bolum AS 'Bölüm',
                            Tarih AS 'Kayıt Tarihi'
                            from Ogrenci_Kaydi";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void frm_KayıtDüzenle_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
            frm_Ogr_Index frm = (frm_Ogr_Index)Application.OpenForms["frm_Ogr_Index"];
            string str = @"Select Ogr_ID AS 'ID',
                           No AS 'Ogr No',
                           TC AS 'TC Kimlik No',
                           AdSoyad AS 'Ad Soyad',
                           Telefon AS 'Telefon',
                           Sinif AS 'Sınıf',
                           Email AS 'E-Posta',
                           Fakulte AS 'Fakülte',
                           Bolum AS 'Bölüm',
                           Tarih AS 'Kayıt Tarihi'
                           from Ogrenci_Kaydi";
            frm.dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        public static string ID="0",str;

        public void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int num = dataGridView1.SelectedCells[0].RowIndex;
            ID = dataGridView1.Rows[num].Cells[0].Value.ToString();
            textBox1.Text = dataGridView1.Rows[num].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[num].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.Rows[num].Cells[3].Value.ToString();
            textBox4.Text = dataGridView1.Rows[num].Cells[4].Value.ToString();
            comboBox1.Text = dataGridView1.Rows[num].Cells[5].Value.ToString();
            textBox5.Text = dataGridView1.Rows[num].Cells[6].Value.ToString();
            comboBox2.Text = dataGridView1.Rows[num].Cells[7].Value.ToString();
            comboBox3.Text = dataGridView1.Rows[num].Cells[8].Value.ToString();
            dateTimePicker1.Text = dataGridView1.Rows[num].Cells[9].Value.ToString();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar) && !char.IsPunctuation(e.KeyChar); //Yalnızca Harf Girişi
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }
    }
}
