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
    public partial class frm_Ders_Düzenle : Form
    {
        public frm_Ders_Düzenle()
        {
            InitializeComponent();
        }

        public static string ID="0",str;

        private void btn_Kaydet_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox3.Text) || String.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(ID.ToString()))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı");
            }

            else
            {
                str =  "SELECT Ders_ID FROM Ders_Bilgi WHERE Ders_ID ='" + ID.ToString() + "'";

                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    str = @"Update Ders_Bilgi
                    set DersNo='" + textBox1.Text.ToString() +
                    "',DersAdi='" + textBox2.Text.ToString() +
                    "',DersHoca='" + textBox3.Text.ToString() +
                    "',DersKredi='" + textBox4.Text.ToString() +
                    "'where Ders_ID='" + ID.ToString() + "'";
                    DBLayer.IslemYap(str);

                    str = @"Select Ders_ID AS 'ID' ,
                    DersNo AS 'Ders Kodu' ,
                    DersAdi AS 'Dersin Adı' ,
                    DersKredi AS 'Kredi' ,
                    DersHoca AS 'Öğretim Görevlisi'
                    from Ders_Bilgi";
                    dataGridView1.DataSource = DBLayer.TabloSorgula(str);

                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    ID = "0";
                }
            }
        }

        private void frm_Ders_Düzenle_Load(object sender, EventArgs e)
        {
            str = @"Select Ders_ID AS 'ID' ,
                    DersNo AS 'Ders Kodu' ,
                    DersAdi AS 'Dersin Adı' ,
                    DersKredi AS 'Kredi' ,
                    DersHoca AS 'Öğretim Görevlisi'
                    from Ders_Bilgi";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int num = dataGridView1.SelectedCells[0].RowIndex;
            ID = dataGridView1.Rows[num].Cells[0].Value.ToString();
            textBox1.Text = dataGridView1.Rows[num].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[num].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.Rows[num].Cells[4].Value.ToString();
            textBox4.Text = dataGridView1.Rows[num].Cells[3].Value.ToString();
        }

        private void frm_Ders_Düzenle_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
            frm_Ders_Index frm = (frm_Ders_Index)Application.OpenForms["frm_Ders_Index"];
            str = @"Select Ders_ID AS 'ID' ,
                    DersNo AS 'Ders Kodu' ,
                    DersAdi AS 'Dersin Adı' ,
                    DersKredi AS 'Kredi' ,
                    DersHoca AS 'Öğretim Görevlisi'
                    from Ders_Bilgi";
            frm.dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); //Yalnızca Sayı Girişi 
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar) && !char.IsPunctuation(e.KeyChar); //Yalnızca Harf Girişi

        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar) && !char.IsPunctuation(e.KeyChar); //Yalnızca Harf Girişi

        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar); //Yalnızca Sayı Girişi ve noktalama
        }
    }
}
