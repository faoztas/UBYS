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
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class frm_Devam_Index : Form
    {
        public frm_Devam_Index()
        {
            InitializeComponent();
        }

        private void frm_Devam_Index_Load(object sender, EventArgs e)
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

        private void frm_Devam_Index_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
        }

        private void btn_Ara_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Lütfen aranacak bir bilgi giriniz!", "Uyarı");
            }
            else
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
        }

        private void btn_Devam_Sil_Click(object sender, EventArgs e)
        {
            str = @"Update Ogrenci_Ders SET DersToplam = NULL,
                DersDevamsiz = NULL
                FROM Ogrenci_Kaydi OK
                Inner Join Ogrenci_Ders OD ON OK.Ogr_ID = OD.Ogr_ID
                Inner Join Ders_Bilgi DB ON OD.Ders_ID = DB.Ders_ID
                WHERE OK.No = '"+ID.ToString()+@"'
                and DB.DersNo = '"+DERSNO.ToString()+"'";
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
            ID = "0"; DERSNO = "0";
        }

        public static string ID="0", DERSNO="0",str;
        
        private void btn_Devam_Ekle_Click(object sender, EventArgs e)
        {
            frm_Devam_Ekle frm1 = new frm_Devam_Ekle();
            frm1.ShowDialog();
            frm1.Close();
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
            using (FileStream stream = new FileStream(folderPath + "Devam Listesi.pdf", FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(pdfTable);
                pdfDoc.Close();
                stream.Close();
            }
            System.Diagnostics.Process.Start(@"Rapor\\Devam Listesi.pdf");
        }

        private void btn_Devam_Duzenle_Click(object sender, EventArgs e)
        {
            frm_Devam_Ekle frm2 = new frm_Devam_Ekle();
            frm2.btn_Kaydet.Text = "Devam Güncelle";
            frm2.ShowDialog();
            frm2.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int num = dataGridView1.SelectedCells[0].RowIndex;
            ID = dataGridView1.Rows[num].Cells[1].Value.ToString();
            DERSNO = dataGridView1.Rows[num].Cells[3].Value.ToString();
        }
    }
}
