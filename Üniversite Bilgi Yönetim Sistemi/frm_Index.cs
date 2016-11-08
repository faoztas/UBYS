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
    public partial class frm_Index : Form
    {
        public frm_Index()
        {
            InitializeComponent();
        }

        private void frm_Index_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btn_ÖğrenciGirişi_Click(object sender, EventArgs e)
        {
            frm_Ogr_Index frm1 = new frm_Ogr_Index();
            frm1.ShowDialog();
            frm1.Close();
        }

        private void btn_DersGirişi_Click(object sender, EventArgs e)
        {
            frm_Ders_Index frm2 = new frm_Ders_Index();
            frm2.ShowDialog();
            frm2.Close();
        }

        private void btn_NotGirişi_Click(object sender, EventArgs e)
        {
            frm_NotGir_Index frm6 = new frm_NotGir_Index();
            frm6.ShowDialog();
            frm6.Close();
        }

        private void btn_Devamsızlık_Click(object sender, EventArgs e)
        {
            frm_Devam_Index frm3 = new frm_Devam_Index();
            frm3.ShowDialog();
            frm3.Close();
        }

        private void btn_Hakkında_Click(object sender, EventArgs e)
        {
            Hakkımızda frm6 = new Hakkımızda();
            frm6.ShowDialog();
            frm6.Close();
        }

        private void btn_Kullanici_Click(object sender, EventArgs e)
        {
            frm_Kullanici_Kaydi frm4 = new frm_Kullanici_Kaydi();
            frm4.ShowDialog();
            frm4.Close();
        }

        private void btn_Ogr_Ders_Click(object sender, EventArgs e)
        {
            frm_Ogr_Ders_Index frm5 = new frm_Ogr_Ders_Index();
            frm5.ShowDialog();
            frm5.Close();
        }
    }
}
