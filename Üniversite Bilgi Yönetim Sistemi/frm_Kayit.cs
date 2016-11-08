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
    public partial class frm_Kayit : Form
    {
        public frm_Kayit()
        {
            InitializeComponent();
        }

        private void frm_Kayit_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = (9);
            dataGridView1.Columns[0].Name = "Ogr No";
            dataGridView1.Columns[1].Name = "TC Kimlik No";
            dataGridView1.Columns[2].Name = "Ad Soyad";
            dataGridView1.Columns[3].Name = "Telefon";
            dataGridView1.Columns[4].Name = "Sınıf";
            dataGridView1.Columns[5].Name = "E-Posta";
            dataGridView1.Columns[6].Name = "Fakulte";
            dataGridView1.Columns[7].Name = "Bölüm";
            dataGridView1.Columns[8].Name = "Kayıt Tarihi";
        }

        private void btn_Ara_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox11.Text) || String.IsNullOrEmpty(textBox22.Text) || String.IsNullOrEmpty(textBox33.Text) || String.IsNullOrEmpty(textBox44.Text) || string.IsNullOrEmpty(textBox55.Text) || String.IsNullOrEmpty(comboBox11.Text) || string.IsNullOrEmpty(comboBox22.Text) || string.IsNullOrEmpty(comboBox33.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı");
            }

            else
            {
                string str = @"Insert into Ogrenci_Kaydi(No,TC,AdSoyad,Telefon,Sinif,Email,Fakulte,Bolum,Tarih)
                               Values('" + textBox11.Text + @"',
                               '" + textBox22.Text + @"',
                               '" + textBox33.Text + @"',
                               '" + textBox44.Text + @"',
                               '" + comboBox11.SelectedItem + @"',
                               '" + textBox55.Text + @"',
                               '" + comboBox22.SelectedItem + @"',
                               '" + comboBox33.SelectedItem + @"',
                               '" + dateTimePicker11.Value.ToString("yyyy-MM-dd") + "')";
                DBLayer.IslemYap(str);

                int i = dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = textBox11.Text;
                dataGridView1.Rows[i].Cells[1].Value = textBox22.Text;
                dataGridView1.Rows[i].Cells[2].Value = textBox33.Text;
                dataGridView1.Rows[i].Cells[3].Value = textBox44.Text;
                dataGridView1.Rows[i].Cells[4].Value = comboBox11.Text;
                dataGridView1.Rows[i].Cells[5].Value = textBox55.Text;
                dataGridView1.Rows[i].Cells[6].Value = comboBox22.Text;
                dataGridView1.Rows[i].Cells[7].Value = comboBox33.Text;
                dataGridView1.Rows[i].Cells[8].Value = dateTimePicker11.Text;

                textBox11.Clear();
                textBox22.Clear();
                textBox33.Clear();
                textBox44.Clear();
                comboBox11.Text=null;
                textBox55.Clear();
                comboBox22.Text = null;
                comboBox33.Text = null;

                i = i + 1;
            }
        }

        private void frm_Kayit_FormClosed(object sender, FormClosedEventArgs e)
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

        public void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int num = dataGridView1.SelectedCells[0].RowIndex;
            string ogrno = dataGridView1.Rows[num].Cells[0].Value.ToString();
            string tcno = dataGridView1.Rows[num].Cells[1].Value.ToString();
            string adsoyad = dataGridView1.Rows[num].Cells[2].Value.ToString();
            string tel = dataGridView1.Rows[num].Cells[3].Value.ToString();
            string snf = dataGridView1.Rows[num].Cells[4].Value.ToString();
            string mail = dataGridView1.Rows[num].Cells[5].Value.ToString();
            string fklte = dataGridView1.Rows[num].Cells[6].Value.ToString();
            string bolum = dataGridView1.Rows[num].Cells[7].Value.ToString();
            string tarih = dataGridView1.Rows[num].Cells[8].Value.ToString();   
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }

        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }

        private void textBox33_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar); //Yalnızca Harf Girişi
        }

        private void textBox44_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }
    }
}
