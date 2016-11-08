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
    public partial class frm_Ders_Ekle : Form
    {
        public frm_Ders_Ekle()
        {
            InitializeComponent();
        }

        private void btn_Kaydet_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox11.Text) || String.IsNullOrEmpty(textBox22.Text) || String.IsNullOrEmpty(textBox33.Text) || String.IsNullOrEmpty(textBox44.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı");
            }

            else
            {
                string str = @"Insert into Ders_Bilgi(DersNo,DersAdi,DersHoca,DersKredi)
                               Values('" + textBox11.Text + @"',
                               '" + textBox22.Text +  @"',
                               '" + textBox33.Text + @"',
                               '" + textBox44.Text + "')";
                DBLayer.IslemYap(str);

                int i = dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = textBox11.Text;
                dataGridView1.Rows[i].Cells[1].Value = textBox22.Text;
                dataGridView1.Rows[i].Cells[2].Value = textBox33.Text;
                dataGridView1.Rows[i].Cells[3].Value = textBox44.Text;
                textBox11.Clear();
                textBox22.Clear();
                textBox33.Clear();
                textBox44.Clear();
                i = i + 1;
            }
    }

        private void frm_Ders_Ekle_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = (4);

            dataGridView1.Columns[0].Name = "Ders Kodu";
            dataGridView1.Columns[1].Name = "Ders Adı";
            dataGridView1.Columns[2].Name = "Kredi";
            dataGridView1.Columns[3].Name = "Öğretim Görevlisi";
        }

        private void frm_Ders_Ekle_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
            frm_Ders_Index frm = (frm_Ders_Index)Application.OpenForms["frm_Ders_Index"];
            string str = @"Select Ders_ID AS 'ID' ,
                    DersNo AS 'Ders Kodu' ,
                    DersAdi AS 'Dersin Adı' ,
                    DersKredi AS 'Kredi' ,
                    DersHoca AS 'Öğretim Görevlisi'
                    from Ders_Bilgi";
            frm.dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }

        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar); //Yalnızca Harf Girişi
        }

        private void textBox33_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar) && !char.IsPunctuation(e.KeyChar); //Yalnızca Harf Girişi
        }

        private void textBox44_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar); //Yalnızca Sayı Girişi 
        }
    }
}
