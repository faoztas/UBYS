using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApplication1
{
    public partial class frm_Not_Ekle_Duzenle : Form
    {
        public frm_Not_Ekle_Duzenle()
        {
            InitializeComponent();
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
                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    textBox4.Text = DBLayer.FunctionÇağır(str).ToString();
                }
            }
        }

        public static string str;

        public static double vize=0, vizee = 0, final = 0, butunleme = 0, vort = 0, ortalama1 = 0, ortalama2 = 0;

        private void btn_Kaydet_Click(object sender, EventArgs e)
        {
            if (textBox5.Text.Trim() == "")
            {
                textBox5.Text = "0";
            }
            if (textBox6.Text.Trim() == "")
            {
                textBox6.Text = "0";
            }
            if (textBox7.Text.Trim() == "")
            {
                textBox7.Text = "0";
            }
            if (textBox8.Text.Trim() == "")
            {
                textBox8.Text = "0";
            }
            vize = double.Parse(textBox5.Text);
            vizee = double.Parse(textBox6.Text);
            final = double.Parse(textBox7.Text);
            butunleme = double.Parse(textBox8.Text);

            if (vize != 0)
            {
                if (vizee != 0)
	            {
                    vort = (vize + vizee)/2;
                }
	            else
	            {
                    vort = vize;
                }
            }
            else
            {
                //MessageBox.Show("Vize notu girmediniz!", "Uyarı");
            }
            if (final != 0)
            {
                 if ( butunleme != 0)
	             {
                    ortalama1 = (vort * 0.5)+(final * 0.5);
                    ortalama2 = (vort * 0.5)+(butunleme * 0.5);
                 }
	             else
	             {
                    ortalama1 = (vort * 0.5)+(final * 0.5);
                 }
            }
            else
            {
                if (butunleme != 0)
	            {
                    ortalama2 = (vort * 0.5)+(butunleme * 0.5);
                }
	            else
	            {
                    //MessageBox.Show("Final/Bütünleme notu girmediniz!", "Uyarı");
                }
            }

            str = @"UPDATE Ogrenci_Not
                    SET Vize='" + textBox5.Text + @"',
                    Vizee='" + textBox6.Text + @"' ,
                    Final='" + textBox7.Text + @"' ,
                    Butunleme='" + textBox8.Text + @"',
                    Ortalama1=" + ortalama1 + @",
                    Ortalama2=" + ortalama2 + @"
                    FROM Ogrenci_Kaydi OK
                    INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID
                    INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID
                    WHERE OK.No = '" + textBox1.Text + @"'
                    AND DB.DersNo = '" + textBox3.Text + "'";
            DBLayer.IslemYap(str);
            str = @"SELECT Ogrenci_Not.Not_ID AS 'ID',
                    Ogrenci_Kaydi.No AS 'Ogr No',
                    Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                    Ders_Bilgi.DersNo AS 'Ders Kodu',
                    Ders_Bilgi.DersAdi AS 'Ders Adı',
                    Ogrenci_Not.Vize AS '1. Vize',
                    Ogrenci_Not.Vizee AS '2.Vize',
                    Ogrenci_Not.Final AS 'Final',
                    Ogrenci_Not.Butunleme AS 'Büt.'
                    FROM Ogrenci_Not
                    INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                    INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
            textBox5.Text = ""; textBox6.Text = ""; textBox7.Text = ""; textBox8.Text = "";
        }

        private void frm_Not_Ekle_Duzenle_Load(object sender, EventArgs e)
        {
            str = @"SELECT Ogrenci_Not.Not_ID AS 'ID',
                    Ogrenci_Kaydi.No AS 'Ogr No',
                    Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                    Ders_Bilgi.DersNo AS 'Ders Kodu',
                    Ders_Bilgi.DersAdi AS 'Ders Adı',
                    Ogrenci_Not.Vize AS '1. Vize',
                    Ogrenci_Not.Vizee AS '2.Vize',
                    Ogrenci_Not.Final AS 'Final',
                    Ogrenci_Not.Butunleme AS 'Büt.'
                    FROM Ogrenci_Not
                    INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                    INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int num = dataGridView1.SelectedCells[0].RowIndex;
            textBox1.Text = dataGridView1.Rows[num].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[num].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.Rows[num].Cells[3].Value.ToString();
            textBox4.Text = dataGridView1.Rows[num].Cells[4].Value.ToString();
            textBox5.Text = dataGridView1.Rows[num].Cells[5].Value.ToString();
            textBox6.Text = dataGridView1.Rows[num].Cells[6].Value.ToString();
            textBox7.Text = dataGridView1.Rows[num].Cells[7].Value.ToString();
            textBox8.Text = dataGridView1.Rows[num].Cells[8].Value.ToString();
        }

        private void frm_Not_Ekle_Duzenle_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
            frm_NotGir_Index frm = (frm_NotGir_Index)Application.OpenForms["frm_NotGir_Index"];
            str = @"SELECT Ogrenci_Not.Not_ID AS 'ID',
                    Ogrenci_Kaydi.No AS 'Ogr No',
                    Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                    Ders_Bilgi.DersNo AS 'Ders Kodu',
                    Ders_Bilgi.DersAdi AS 'Ders Adı',
                    Ogrenci_Not.Vize AS '1. Vize',
                    Ogrenci_Not.Vizee AS '2.Vize',
                    Ogrenci_Not.Final AS 'Final',
                    Ogrenci_Not.Butunleme AS 'Büt.'
                    FROM Ogrenci_Not
                    INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                    INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID";
            frm.dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }
    }
}
