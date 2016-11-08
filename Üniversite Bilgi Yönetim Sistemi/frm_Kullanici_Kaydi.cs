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
using System.Security.Cryptography;

namespace WindowsFormsApplication1
{
    public partial class frm_Kullanici_Kaydi : Form
    {
        public frm_Kullanici_Kaydi()
        {
            InitializeComponent();
        }

        public static string ID="0",str;

        private void frm_Kullanici_Kaydi_Load(object sender, EventArgs e)
        {
            str = "Select *from Kullanici_Kaydi";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
            ID = "0";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int num = dataGridView1.SelectedCells[0].RowIndex;
            ID = dataGridView1.Rows[num].Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.Rows[num].Cells[1].Value.ToString();
        }

        private void btn_Ara_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Lütfen aranacak bir bilgi giriniz!", "Uyarı");
            }
            else
            {
                str = @"Select *from Kullanici_Kaydi
                        where Email like '%" + textBox1.Text.ToString() + "%'";
                dataGridView1.DataSource = DBLayer.TabloSorgula(str);
            }
        }

        private void btn_Kaydet_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox3.Text))
            {
                //MessageBox.Show("Alanlar boş bırakılamaz!", "Uyarı");
            }
            else
            {
                string str = @"SELECT ID FROM Kullanici_Kaydi
                               WHERE Email='" + textBox2.Text.ToString() + "'";
                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    string ID = DBLayer.FunctionÇağır(str).ToString();
                    str = @"UPDATE Kullanici_Kaydi SET
                           Email = '" + textBox2.Text.ToString() + @"' ,
                           Sifre = '" + HashLayer.SHA512(textBox3.Text.ToString()) + @"'
                           WHERE ID = '" + ID.ToString() + "' ";
                    DBLayer.IslemYap(str);
                    str = "Select *from Kullanici_Kaydi";
                    dataGridView1.DataSource = DBLayer.TabloSorgula(str);
                    textBox1.Text = "";
                    textBox2.Text = "";
                    ID = "0";
                }
                else
                {
                    str = @"INSERT INTO Kullanici_Kaydi (Email,Sifre)
                    Values('" + textBox2.Text.ToString() + @"',
                    '" + HashLayer.SHA512(textBox3.Text.ToString()) + "')";
                    DBLayer.IslemYap(str);
                    str = "Select *from Kullanici_Kaydi";
                    dataGridView1.DataSource = DBLayer.TabloSorgula(str);
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
            }
        }

        private void frm_Kullanici_Kaydi_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
        }

        private void btn_Sil_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ID.ToString()))
            {
                MessageBox.Show("Lütfen silinecek bir kayıt seçiniz!", "Uyarı");
            }
            else
            {
                str = @"Delete from Kullanici_Kaydi
                        where ID=( '" + ID.ToString() + "' )";
                DBLayer.IslemYap(str);
                str = "Select *from Kullanici_Kaydi";
                dataGridView1.DataSource = DBLayer.TabloSorgula(str);
                ID = "0";
            }
        }
    }
}
