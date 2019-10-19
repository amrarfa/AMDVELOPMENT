using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Security.AccessControl;
using Microsoft.Win32;
using MicroVisionTrailMaker;

namespace ROSESONLY.forms
{
    public partial class frm_splashscreen : DevExpress.XtraEditors.XtraForm
    {
        MicroVisionTrailMaker.TrailRegister trial = new TrailRegister();
        string str = "";

        public frm_splashscreen()
        {
            InitializeComponent();
            lb_number.Text = Properties.Settings.Default.open_number.ToString();
            Properties.Settings.Default.open_number += 1;
            Properties.Settings.Default.Save();

        }
        string getkey()
        {
            try
            {
                RegistryKey reg;
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Rose_reg", true);
                string x;
                x = reg.GetValue("prog_key").ToString();
                return x;
            }
            catch (Exception)
            {
                return "";
            }
        }
        int loadingdatat()
        {
            open_connections();
            if (getkey() == string.Empty)
            {
                trayagain:
                str = trial.TrialPeriod(7, "SOFTWARE", "windowslog");
                if (string.IsNullOrEmpty(str))
                {
                    goto trayagain;
                }
                else
                {
                    if (str.StartsWith("XXX") == true)
                    {
                        if (Properties.Settings.Default.open_number > 100)
                        {
                            frm_activations frm2 = new frm_activations();
                            frm2.ShowDialog();
                        }
                    }
                    else
                    {
                        frm_activations frm2 = new frm_activations();
                        frm2.ShowDialog();
                    }
                }
            }
            else
            {
                if (MicroVisionSerial.Encryption.Activation.CheckActivationKey(Security.FingerPrint.Value(), getkey()) == false)
                {
                    MessageBox.Show("error in activations code");
                    frm_activations frm = new frm_activations();
                    frm.ShowDialog();
                    Application.Exit();
                }
            }
            int x =0;
            return x;
        }
        private void frm_splashscreen_Load(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            });
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = loadingdatat();
        }
        void open_connections()
        {
            ROSESONLY.DLL.connectiondata cd = new DLL.connectiondata();
            DataTable dt = new DataTable();
            try
            {
                dt = cd.getdata("check_connections", null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                frm_repair frm = new frm_repair();
                frm.ShowDialog();
            }

        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Hide();
            frm_login frm = new frm_login();
            frm.ShowDialog();
            this.Close();
        }
    }
}