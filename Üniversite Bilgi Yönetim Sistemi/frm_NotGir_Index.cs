using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace WindowsFormsApplication1
{
    public partial class frm_NotGir_Index : Form
    {
        public frm_NotGir_Index()
        {
            InitializeComponent();
        }

        private void btn_Ara_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Lütfen aranacak bir bilgi giriniz!", "Uyarı");
            }
            else
            {
                str = @"SELECT Ogrenci_Not.Not_ID AS 'ID',
                        Ogrenci_Kaydi.No AS 'Ogr. No',
                        Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                        Ders_Bilgi.DersNo AS 'Ders Kodu',
                        Ders_Bilgi.DersAdi AS 'Ders Adı',
                        Ogrenci_Not.Vize AS '1. Vize',
                        Ogrenci_Not.Vizee AS '2. Vize',
                        Ogrenci_Not.Final AS 'Final',
                        Ogrenci_Not.Butunleme AS 'Büt.'
                        FROM Ogrenci_Not
                        INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                        INNER JOIN Ogrenci_Ders ON Ogrenci_Not.Ogr_Ders_ID = Ogrenci_Ders.Ogr_Ders_ID
                        INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID
                        WHERE Ogrenci_Kaydi.No = '" + textBox1.Text + @"'
                        OR Ogrenci_Kaydi.AdSoyad LIKE '%" + textBox1.Text + @"%'
                        OR Ders_Bilgi.DersNo = '" + textBox1.Text + @"'
                        OR Ders_Bilgi.DersAdi LIKE '%" + textBox1.Text + "%'";
                dataGridView1.DataSource = DBLayer.TabloSorgula(str);
            }
        }

        private void frm_NotGir_Index_Load(object sender, EventArgs e)
        {
            str = @"SELECT Ogrenci_Not.Not_ID AS 'ID',
                    Ogrenci_Kaydi.No AS 'Ogr. No',
                    Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                    Ders_Bilgi.DersNo AS 'Ders Kodu',
                    Ders_Bilgi.DersAdi AS 'Ders Adı',
                    Ogrenci_Not.Vize AS '1. Vize',
                    Ogrenci_Not.Vizee AS '2. Vize',
                    Ogrenci_Not.Final AS 'Final',
                    Ogrenci_Not.Butunleme AS 'Büt.'
                    FROM Ogrenci_Not
                    INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                    INNER JOIN Ogrenci_Ders ON Ogrenci_Not.Ogr_Ders_ID = Ogrenci_Ders.Ogr_Ders_ID
                    INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void btn_NotGir_Click(object sender, EventArgs e)
        {
            frm_Not_Ekle_Duzenle frm = new frm_Not_Ekle_Duzenle();
            frm.ShowDialog();
            frm.Close();
        }

        private void btn_Guncelle_Click(object sender, EventArgs e)
        {
            frm_Not_Ekle_Duzenle frm2 = new frm_Not_Ekle_Duzenle();
            frm2.btn_Kaydet.Text = "Not Güncelle";
            frm2.ShowDialog();
            frm2.Close();
        }

        private void btn_Sil_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ID.ToString()))
            {
                MessageBox.Show("Lütfen silinecek bir kayıt seçiniz!", "Uyarı");
            }
            else
            {
                str = @"Update Ogrenci_Not SET Vize = NULL,
                        Vizee = NULL, Final = NULL,
                        Butunleme = NULL
                        WHERE Ogrenci_Not.Not_ID=( " + ID.ToString() + " )";
                DBLayer.IslemYap(str);

                str = @"SELECT Ogrenci_Not.Not_ID AS 'ID',
                        Ogrenci_Kaydi.No AS 'Ogr. No',
                        Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                        Ders_Bilgi.DersNo AS 'Ders Kodu',
                        Ders_Bilgi.DersAdi AS 'Ders Adı',
                        Ogrenci_Not.Vize AS '1. Vize',
                        Ogrenci_Not.Vizee AS '2. Vize',
                        Ogrenci_Not.Final AS 'Final',
                        Ogrenci_Not.Butunleme AS 'Büt.'
                        FROM Ogrenci_Not
                        INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                        INNER JOIN Ogrenci_Ders ON Ogrenci_Not.Ogr_Ders_ID = Ogrenci_Ders.Ogr_Ders_ID
                        INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID";
                dataGridView1.DataSource = DBLayer.TabloSorgula(str);
                ID = "0";
            }
        }

        public static string ID = "0",str;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int num = dataGridView1.SelectedCells[0].RowIndex;
            ID = dataGridView1.Rows[num].Cells[0].Value.ToString();
        }

        private void frm_NotGir_Index_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
        }

        private void btn_Hesapla_Click(object sender, EventArgs e)
        {
            frm_Not_Hesapla frm2 = new frm_Not_Hesapla();
            frm2.ShowDialog();
            frm2.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                str = @"SELECT Ogrenci_Not.Not_ID AS 'ID',
                        Ogrenci_Kaydi.No AS 'Ogr. No',
                        Ogrenci_Kaydi.AdSoyad AS 'Ad Soyad',
                        Ders_Bilgi.DersNo AS 'Ders Kodu',
                        Ders_Bilgi.DersAdi AS 'Ders Adı',
                        Ogrenci_Not.Vize AS '1. Vize',
                        Ogrenci_Not.Vizee AS '2. Vize',
                        Ogrenci_Not.Final AS 'Final',
                        Ogrenci_Not.Butunleme AS 'Büt.'
                        FROM Ogrenci_Not
                        INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                        INNER JOIN Ogrenci_Ders ON Ogrenci_Not.Ogr_Ders_ID = Ogrenci_Ders.Ogr_Ders_ID
                        INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID";
                dataGridView1.DataSource = DBLayer.TabloSorgula(str);
            }
        }

        private void btn_raporla_Click(object sender, EventArgs e)
        {
            //PDF ayarları
            BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("Helvetica", "CP1254", BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fontNormal = new iTextSharp.text.Font(STF_Helvetica_Turkish, 12, iTextSharp.text.Font.NORMAL);
            PdfPTable pdfTable = new PdfPTable(dataGridView1.ColumnCount);
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.DefaultCell.Phrase = new Phrase() { Font = fontNormal };
            pdfTable.WidthPercentage = 100;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTable.DefaultCell.BorderWidth = 1;

            //Başlık eklendi
            for (int j = 0; j < dataGridView1.Columns.Count; j++)
            {
                PdfPCell cell = new PdfPCell(new Phrase(dataGridView1.Columns[j].HeaderText, fontNormal));
                cell.BackgroundColor = new iTextSharp.text.Color(240, 240, 240);
                pdfTable.AddCell(cell);
            }

            //alt satırlar
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int k = 0; k < dataGridView1.Columns.Count; k++)
                {
                    if (dataGridView1[k, i].Value != null)
                    {
                        pdfTable.AddCell(new Phrase(dataGridView1[k, i].Value.ToString(), fontNormal));
                    }
                }
            }

            //PDF oluştur
            string folderPath = "Rapor\\";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (FileStream stream = new FileStream(folderPath + "Not Listesi.pdf", FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(pdfTable);
                pdfDoc.Close();
                stream.Close();
            }
            System.Diagnostics.Process.Start(@"Rapor\\Not Listesi.pdf");


        }
    }
}
