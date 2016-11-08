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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        public static string str;
        
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Kullanıcı Adı ve/veya Şifre boş bırakılamaz!", "!.Uyarı.!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                i++;
            }

            try
            {
                    str = @"SELECT * FROM Kullanici_Kaydi
                            WHERE Email='" + textBox1.Text.ToString() + @"' COLLATE Turkish_CS_AS AND
                            Sifre='" + HashLayer.SHA512(textBox2.Text.ToString()) + "'";
                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    this.Hide();
                    frm_Index frm = new frm_Index();
                    frm.ShowDialog();
                    frm.Close();
                }
                if(i==0)
                {
                     MessageBox.Show("Yanlış Kullanıcı Adı ve/veya Şifre girdiniz!", "!.Uyarı.!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            }
        }
    }
