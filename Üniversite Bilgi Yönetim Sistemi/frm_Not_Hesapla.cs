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
using System.Collections;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class frm_Not_Hesapla : Form
    {
        public frm_Not_Hesapla()
        {
            InitializeComponent();
        }

        SqlConnection Baglan = new SqlConnection("Data Source=mssql3.gear.host;Initial Catalog=ubys;Persist Security Info=True;User ID=ubys;Password=Ks61?S8YQT?4");

        public void VerileriGöster(string kaynak)
        {
            SqlDataAdapter da = new SqlDataAdapter(kaynak, Baglan);
            DataSet ds = new DataSet();

            da.Fill(ds);

            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.ClearSelection();
        }

        public static string ID="0",str;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                label2.Text = "";
            }
            else
            {
                label2.Text = "";
                str = @"SELECT DersAdi FROM Ders_Bilgi
                        WHERE DersNo = '" + textBox1.Text + "'";
                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    label2.Text = DBLayer.FunctionÇağır(str).ToString();
                }
                str = @"SELECT Ders_ID FROM Ders_Bilgi
                        WHERE DersNo = '" + textBox1.Text + "'";
                if (DBLayer.TabloSorgula(str).Rows.Count > 0)
                {
                    ID = DBLayer.FunctionÇağır(str).ToString();
                }
            }
        }

        public static double FO = 0, BO = 0, FS = 0, BS = 0;
        public static double DT=0,DS=0,final=0,but=0, FinOrt = 0, ButOrt = 0, TdegerFinal =0,TdegerBut=0,devamoran=0;

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
            using (FileStream stream = new FileStream(folderPath + "Not Harf Listesi.pdf", FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(pdfTable);
                pdfDoc.Close();
                stream.Close();
            }
            System.Diagnostics.Process.Start(@"Rapor\\Not Harf Listesi.pdf");
        }

        public static string No = "0";

        private void btn_Hesapla_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(label2.Text))
            {
                MessageBox.Show("Lütfen bir Ders Kodu giriniz!", "Uyarı");
            }
            else
            {
                Baglan.Open();
                SqlCommand Komut_SS = new SqlCommand(@"SELECT AVG(Ogrenci_Not.Ortalama1) AS 'FinalOrt',
                                                       AVG(Ogrenci_Not.Ortalama2) AS 'ButOrt',
                                                       STDEV(Ogrenci_Not.Ortalama1) AS 'FinalSapma',
                                                       STDEV(Ogrenci_Not.Ortalama2) AS 'ButSapma'
                                                       FROM Ogrenci_Not
                                                       INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                                                       INNER JOIN Ogrenci_Ders ON Ogrenci_Not.Ogr_Ders_ID = Ogrenci_Ders.Ogr_Ders_ID 
                                                       INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID
                                                       WHERE Ders_Bilgi.DersNo = '" + textBox1.Text + "'", Baglan);
                SqlDataReader Oku_SS = Komut_SS.ExecuteReader();
                while (Oku_SS.Read())
                {
                    FO = Double.Parse(Oku_SS["FinalOrt"].ToString());
                    BO = Double.Parse(Oku_SS["ButOrt"].ToString());
                    FS = Double.Parse(Oku_SS["FinalSapma"].ToString());
                    BS = Double.Parse(Oku_SS["ButSapma"].ToString());

                    textBox2.Text = FO.ToString();
                    textBox5.Text = BO.ToString();
                    textBox3.Text = FS.ToString();
                    textBox4.Text = BS.ToString();
                }
                Baglan.Close();

                Baglan.Open();
                SqlCommand Komut_Not = new SqlCommand(@"SELECT Ogrenci_Kaydi.No,
                                                        Ogrenci_Ders.DersToplam,
                                                        Ogrenci_Ders.DersDevamsiz,
                                                        Ogrenci_Not.Ortalama1,
                                                        Ogrenci_Not.Ortalama2,
                                                        Ogrenci_Not.Final,
                                                        Ogrenci_Not.Butunleme
                                                        FROM Ogrenci_Ders
                                                        INNER JOIN Ogrenci_Kaydi ON Ogrenci_Ders.Ogr_ID = Ogrenci_Kaydi.Ogr_ID
                                                        INNER JOIN Ders_Bilgi ON Ogrenci_Ders.Ders_ID = Ders_Bilgi.Ders_ID
                                                        INNER JOIN Ogrenci_Not ON Ogrenci_Not.Ogr_Ders_ID = Ogrenci_Ders.Ogr_Ders_ID
                                                        WHERE Ders_Bilgi.DersNo = '" + textBox1.Text + "'",Baglan);
                SqlDataReader Oku_Not = Komut_Not.ExecuteReader();                

                ArrayList NotNo = new ArrayList();
                ArrayList NotDT = new ArrayList();
                ArrayList NotDS = new ArrayList();
                ArrayList Final = new ArrayList();
                ArrayList But = new ArrayList();
                ArrayList Fort = new ArrayList();
                ArrayList Bort = new ArrayList();

                while (Oku_Not.Read())
                {
                    NotNo.Add(Oku_Not["No"]);
                    NotDT.Add(Oku_Not["DersToplam"]);
                    NotDS.Add(Oku_Not["DersDevamsiz"]);
                    Final.Add(Oku_Not["Final"]);
                    But.Add(Oku_Not["Butunleme"]);
                    Fort.Add(Oku_Not["Ortalama1"]);
                    Bort.Add(Oku_Not["Ortalama2"]);
                }
                Baglan.Close();

                if (FO == 0)
                {
                    //MessageBox.Show("Final Notları Girilmemiş!");
                }
                if (BO == 0)
                {
                    //MessageBox.Show("Bütünleme Notları Girilmemiş!");
                }

                for (int i=0;i<NotNo.Count;i++)
                {
                    No = NotNo[i].ToString();

                    DT = Double.Parse(NotDT[i].ToString());
                    DS = Double.Parse(NotDS[i].ToString());

                    devamoran = DT / DS;

                    final = Double.Parse(Final[i].ToString());
                    but = Double.Parse(But[i].ToString());

                    FinOrt = Double.Parse(Fort[i].ToString());
                    ButOrt = Double.Parse(Bort[i].ToString());

                    TdegerFinal = ((FinOrt - FO ) / (FS) * 10 ) + 50;
                    TdegerBut = ((ButOrt - BO) / (BS) * 10) + 50;

                    if (FO != 0)
                    {
                        if (devamoran < 3.332)//FF
                        {
                            Baglan.Open();

                            SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                            Komut.ExecuteNonQuery();

                            Baglan.Close();
                        }
                        else
                        {
                            if (final < 45)//Final notu 45 altı ise FF
                            {
                                Baglan.Open();

                                SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                Komut.ExecuteNonQuery();

                                Baglan.Close();
                            }
                            else
                            {
                                if (FO >= 80)//T Değeri Tablosu Başı - Final
                                {
                                    if (FinOrt > 90)//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (FinOrt > 80 && FinOrt < 89)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (FinOrt > 75 && FinOrt < 79)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (FinOrt > 70 && FinOrt < 74)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (FinOrt > 60 && FinOrt < 69)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (FinOrt > 50 && FinOrt < 59)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (FinOrt > 40 && FinOrt < 49)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (FinOrt > 30 && FinOrt < 39)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (FO > 70 && FO < 80)
                                {
                                    if (TdegerFinal < 24)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 24 && TdegerFinal < 28.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 29 && TdegerFinal < 33.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 34 && TdegerFinal < 38.99)
                                    {
                                        Baglan.Open();//DC

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 39 && TdegerFinal < 43.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 44 && TdegerFinal < 48.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 49 && TdegerFinal < 53.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 54 && TdegerFinal < 58.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (FO > 62.5 && FO < 70)
                                {
                                    if (TdegerFinal < 26)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 26 && TdegerFinal < 30.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 31 && TdegerFinal < 35.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 36 && TdegerFinal < 40.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 41 && TdegerFinal < 45.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 46 && TdegerFinal < 50.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 51 && TdegerFinal < 55.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 56 && TdegerFinal < 60.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (FO > 57.5 && FO < 62.5)
                                {
                                    if (TdegerFinal < 28)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 28 && TdegerFinal < 32.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 33 && TdegerFinal < 37.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 38 && TdegerFinal < 42.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 43 && TdegerFinal < 47.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 48 && TdegerFinal < 52.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 53 && TdegerFinal < 57.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 58 && TdegerFinal < 62.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (FO > 52.5 && FO < 57.5)
                                {
                                    if (TdegerFinal < 30)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 30 && TdegerFinal < 34.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 35 && TdegerFinal < 39.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 40 && TdegerFinal < 44.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 45 && TdegerFinal < 49.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 50 && TdegerFinal < 54.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 55 && TdegerFinal < 59.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 60 && TdegerFinal < 64.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (FO > 47.5 && FO < 52.5)
                                {
                                    if (TdegerFinal < 32)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 32 && TdegerFinal < 36.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 37 && TdegerFinal < 41.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 42 && TdegerFinal < 46.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 47 && TdegerFinal < 51.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 52 && TdegerFinal < 56.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 57 && TdegerFinal < 61.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 62 && TdegerFinal < 66.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (FO > 42.5 && FO < 47.5)
                                {
                                    if (TdegerFinal < 34)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 34 && TdegerFinal < 38.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 39 && TdegerFinal < 43.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 44 && TdegerFinal < 48.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 49 && TdegerFinal < 53.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 54 && TdegerFinal < 58.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 59 && TdegerFinal < 63.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 64 && TdegerFinal < 68.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else
                                {
                                    if (TdegerFinal < 36)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 36 && TdegerFinal < 40.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 41 && TdegerFinal < 45.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 46 && TdegerFinal < 50.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 51 && TdegerFinal < 55.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 56 && TdegerFinal < 60.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 61 && TdegerFinal < 65.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerFinal > 66 && TdegerFinal < 70.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                            }
                        }//T Değeri Tablosu Sonu - Final
                    }
                    if(BO!=0)//T Değeri tablosu - Büt
                    {
                        if (devamoran < 3.332)//FF
                        {
                            Baglan.Open();

                            SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf1='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                            Komut.ExecuteNonQuery();

                            Baglan.Close();
                        }
                        else
                        {
                            if (but < 45)//Bütünleme notu 45 altı ise FF
                            {
                                Baglan.Open();

                                SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                Komut.ExecuteNonQuery();

                                Baglan.Close();
                            }
                            else
                            {
                                if (BO >= 80)//T Değeri Tablosu Başı
                                {
                                    if (ButOrt > 90)//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (ButOrt > 80 && ButOrt < 89)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (ButOrt > 75 && ButOrt < 79)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (ButOrt > 70 && ButOrt < 74)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (ButOrt > 60 && ButOrt < 69)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (ButOrt > 50 && ButOrt < 59)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (ButOrt > 40 && ButOrt < 49)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (ButOrt > 30 && ButOrt < 39)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (BO > 70 && BO < 80)
                                {
                                    if (TdegerBut < 24)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 24 && TdegerBut < 28.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 29 && TdegerBut < 33.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 34 && TdegerBut < 38.99)
                                    {
                                        Baglan.Open();//DC

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 39 && TdegerBut < 43.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 44 && TdegerBut < 48.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 49 && TdegerBut < 53.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 54 && TdegerBut < 58.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (BO > 62.5 && BO < 70)
                                {
                                    if (TdegerBut < 26)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 26 && TdegerBut < 30.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 31 && TdegerBut < 35.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 36 && TdegerBut < 40.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 41 && TdegerBut < 45.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 46 && TdegerBut < 50.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 51 && TdegerBut < 55.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 56 && TdegerBut < 60.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (BO > 57.5 && BO < 62.5)
                                {
                                    if (TdegerBut < 28)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 28 && TdegerBut < 32.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 33 && TdegerBut < 37.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 38 && TdegerBut < 42.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 43 && TdegerBut < 47.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 48 && TdegerBut < 52.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 53 && TdegerBut < 57.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 58 && TdegerBut < 62.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (BO > 52.5 && BO < 57.5)
                                {
                                    if (TdegerBut < 30)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 30 && TdegerBut < 34.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 35 && TdegerBut < 39.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 40 && TdegerBut < 44.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 45 && TdegerBut < 49.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 50 && TdegerBut < 54.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 55 && TdegerBut < 59.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 60 && TdegerBut < 64.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (BO > 47.5 && BO < 52.5)
                                {
                                    if (TdegerBut < 32)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 32 && TdegerBut < 36.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 37 && TdegerBut < 41.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 42 && TdegerBut < 46.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 47 && TdegerBut < 51.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 52 && TdegerBut < 56.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 57 && TdegerBut < 61.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 62 && TdegerBut < 66.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else if (BO > 42.5 && BO < 47.5)
                                {
                                    if (TdegerBut < 34)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 34 && TdegerBut < 38.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 39 && TdegerBut < 43.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 44 && TdegerBut < 48.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 49 && TdegerBut < 53.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 54 && TdegerBut < 58.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 59 && TdegerBut < 63.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 64 && TdegerBut < 68.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                                else
                                {
                                    if (TdegerBut < 36)//FF
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FF' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 36 && TdegerBut < 40.99)//FD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='FD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 41 && TdegerBut < 45.99)//DD
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DD' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 46 && TdegerBut < 50.99)//DC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='DC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 51 && TdegerBut < 55.99)//CC
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CC' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 56 && TdegerBut < 60.99)//CB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='CB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 61 && TdegerBut < 65.99)//BB
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BB' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else if (TdegerBut > 66 && TdegerBut < 70.99)//BA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='BA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                    else//AA
                                    {
                                        Baglan.Open();

                                        SqlCommand Komut = new SqlCommand("UPDATE Ogrenci_Not SET Harf2='AA' FROM Ogrenci_Kaydi OK INNER JOIN Ogrenci_Not OgN ON OK.Ogr_ID = OgN.Ogr_ID INNER JOIN Ders_Bilgi DB ON OgN.Ders_ID = DB.Ders_ID WHERE OK.No = '" + No + "' AND DB.DersNo = '" + textBox1.Text + "'", Baglan);
                                        Komut.ExecuteNonQuery();

                                        Baglan.Close();
                                    }
                                }
                            }
                        }//T Değeri Tablosu Sonu -Büt
                    }
                    

                    str = "SELECT Ogrenci_Not.Not_ID AS 'ID',Ogrenci_Kaydi.No AS 'Ogr. No',Ogrenci_Kaydi.AdSoyad AS 'Adı Soyadı', Ders_Bilgi.DersNo AS 'Ders Kodu', Ders_Bilgi.DersAdi AS 'Dersin Adı', Ogrenci_Not.Vize AS '1. Vize', Ogrenci_Not.Vizee AS '2. Vize', Ogrenci_Not.Final AS 'Final', Ogrenci_Not.Ortalama1 AS 'Final Ort.', Ogrenci_Not.Harf1 AS 'Final Harf', Ogrenci_Not.Butunleme AS 'Büt.', Ogrenci_Not.Ortalama2 AS 'Büt. Ort.', Ogrenci_Not.Harf2 AS 'Büt. Harf' FROM Ogrenci_Not INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID INNER JOIN Ogrenci_Ders ON Ogrenci_Not.Ogr_Ders_ID = Ogrenci_Ders.Ogr_Ders_ID INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID";
                    dataGridView1.DataSource = DBLayer.TabloSorgula(str);
                }

            }
        }

        private void btn_Ara_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox6.Text))
            {
                MessageBox.Show("Lütfen aranacak bir bilgi giriniz!", "Uyarı");
            }
            else
            {
                str = "SELECT Ogrenci_Not.Not_ID AS 'ID',Ogrenci_Kaydi.No AS 'Ogr. No',Ogrenci_Kaydi.AdSoyad AS 'Adı Soyadı', Ders_Bilgi.DersNo AS 'Ders Kodu', Ders_Bilgi.DersAdi AS 'Dersin Adı', Ogrenci_Not.Vize AS '1. Vize', Ogrenci_Not.Vizee AS '2. Vize', Ogrenci_Not.Final AS 'Final', Ogrenci_Not.Ortalama1 AS 'Final Ort.', Ogrenci_Not.Harf1 AS 'Final Harf', Ogrenci_Not.Butunleme AS 'Büt.', Ogrenci_Not.Ortalama2 AS 'Büt. Ort.', Ogrenci_Not.Harf2 AS 'Büt. Harf' FROM Ogrenci_Not INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID INNER JOIN Ogrenci_Ders ON Ogrenci_Not.Ogr_Ders_ID = Ogrenci_Ders.Ogr_Ders_ID INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID WHERE Ogrenci_Kaydi.No = '" + textBox6.Text + "' OR Ders_Bilgi.DersNo = '" + textBox6.Text + "'";
                dataGridView1.DataSource = DBLayer.TabloSorgula(str);
            }
        }

        private void frm_Not_Hesapla_Load(object sender, EventArgs e)
        {
            str = "SELECT Ogrenci_Not.Not_ID AS 'ID',Ogrenci_Kaydi.No AS 'Ogr. No',Ogrenci_Kaydi.AdSoyad AS 'Adı Soyadı', Ders_Bilgi.DersNo AS 'Ders Kodu', Ders_Bilgi.DersAdi AS 'Dersin Adı', Ogrenci_Not.Vize AS '1. Vize', Ogrenci_Not.Vizee AS '2. Vize', Ogrenci_Not.Final AS 'Final', Ogrenci_Not.Ortalama1 AS 'Final Ort.', Ogrenci_Not.Harf1 AS 'Final Harf', Ogrenci_Not.Butunleme AS 'Büt.', Ogrenci_Not.Ortalama2 AS 'Büt. Ort.', Ogrenci_Not.Harf2 AS 'Büt. Harf' FROM Ogrenci_Not INNER JOIN Ogrenci_Kaydi ON Ogrenci_Not.Ogr_ID = Ogrenci_Kaydi.Ogr_ID INNER JOIN Ogrenci_Ders ON Ogrenci_Not.Ogr_Ders_ID = Ogrenci_Ders.Ogr_Ders_ID INNER JOIN Ders_Bilgi ON Ders_Bilgi.Ders_ID = Ogrenci_Not.Ders_ID";
            dataGridView1.DataSource = DBLayer.TabloSorgula(str);
        }

        private void frm_Not_Hesapla_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.Hide();
        }
    }
}