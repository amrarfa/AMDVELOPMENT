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
using System.Data.SqlClient;
using System.IO;
using ROSESONLY.DLL;
using DevExpress.XtraCharts;
using System.Threading;
namespace ROSESONLY.forms
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        Boolean first_open = true;
        connectiondata cd = new connectiondata();
        public  DataTable dt_copmany = new DataTable();
         public DataTable dt_setting = new DataTable();

        public static bool formclosing;
        private static XtraForm1 frm;
        static void form_closed(object sender, FormClosedEventArgs e)
        {
            frm = null;
        }
        public static XtraForm1 getmain
        {
            get
            {
                if (frm == null)
                {
                    frm = new XtraForm1();
                    frm.FormClosed += new FormClosedEventHandler(form_closed);
                }
                return frm;
            }
        }
        public XtraForm1()
        {
            if (frm == null)
                frm = this;
            InitializeComponent();

        }
        private  void XtraForm1_Load(object sender, EventArgs e)
        {
            progressPanel1.Visible = true;
            progressPanel2.Visible = true;
            progressPanel3.Visible = true;

            backgroundWorker1.RunWorkerAsync();
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }
        string   loading_data()
        {
            DateTime dt1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime dt2 = dt1.AddMonths(1).AddDays(-1);
            SqlParameter[] p = new SqlParameter[2];
            p[0] = new SqlParameter("@first", dt1);
            p[1] = new SqlParameter("@last", dt2);
            DataSet ds = new DataSet();
            ds = cd.get_dataset("sp_loadingmaindata", p);
            //binding company data............
            dt_setting = ds.Tables[5];
            dt_copmany = ds.Tables[0];
            lb_companyname.Text = Properties.Settings.Default.company_name;
            if(string.IsNullOrEmpty(ds.Tables[0].Rows[0]["logo"].ToString()))
            {
                logo.Image = Properties.Resources.product_image;
            }
            else
            {
                byte[] imagebyt = (byte[])ds.Tables[0].Rows[0]["logo"];
                MemoryStream ms = new MemoryStream(imagebyt);
                logo.Image = Image.FromStream(ms);
            }
            //binding totals for today only...........
            lb_salescash.Text = ds.Tables[1].Rows[0]["sales_cash"].ToString();
            lb_salesdebit.Text = ds.Tables[1].Rows[0]["sales_debit"].ToString();
            lb_salesreturn.Text = ds.Tables[1].Rows[0]["sales_return"].ToString();
            lb_purchasecash.Text = ds.Tables[1].Rows[0]["purchase_cash"].ToString();
            lb_purachasedebit.Text = ds.Tables[1].Rows[0]["purchase_debit"].ToString();
            lb_purchasereturn.Text = ds.Tables[1].Rows[0]["purchase_return"].ToString();
            //binding monthly sales chart............
            chartControl1.Series.Clear();
            chartControl1.DataSource = ds.Tables[2];
            DevExpress.XtraCharts.Series sales = new DevExpress.XtraCharts.Series("", ViewType.StackedLine);
            sales.ArgumentDataMember = "اليوم";
            sales.ValueDataMembersSerializable = "المبيعات";
            sales.ShowInLegend = true;
            sales.Name = "";
            chartControl1.Series.Add(sales);
            //binding top 10 customer monthly sales...........
            chartControl3.Series.Clear();
            DevExpress.XtraCharts.Series customer = new DevExpress.XtraCharts.Series("Customer Sales", ViewType.Pie);
            customer.DataSource = ds.Tables[3];
            customer.ArgumentDataMember = "العميل";
            customer.ValueDataMembersSerializable = "المبيعات";
            customer.ValueDataMembers.AddRange(new string[] { "المبيعات" });
            customer.ShowInLegend = true;
            chartControl3.Series.Add(customer);
            customer.Label.TextPattern = "{VP:p0} ({V:.##}LE)";
            customer.LegendTextPattern = "{A}";
            // Adjust the position of series labels.  
            ((PieSeriesLabel)customer.Label).Position = PieSeriesLabelPosition.TwoColumns;

            // Detect overlapping of series labels. 
            ((PieSeriesLabel)customer.Label).ResolveOverlappingMode = ResolveOverlappingMode.Default;

            // Access the view-type-specific options of the series. 
            PieSeriesView myView = (PieSeriesView)customer.View;

            // Specify a data filter to explode points. 
            myView.ExplodedPointsFilters.Add(new SeriesPointFilter(SeriesPointKey.Value_1,
                DataFilterCondition.GreaterThanOrEqual, 9));
            myView.ExplodedPointsFilters.Add(new SeriesPointFilter(SeriesPointKey.Argument,
                DataFilterCondition.NotEqual, "Others"));
            myView.ExplodeMode = PieExplodeMode.UseFilters;
            myView.ExplodedDistancePercentage = 5;
            myView.RuntimeExploding = true;
            // Customize the legend. 
            chartControl3.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;

            //binding top10 products sales monthly.................
            chartControl2.Series.Clear();
            chartControl2.DataSource = ds.Tables[4];
            DevExpress.XtraCharts.Series products = new DevExpress.XtraCharts.Series();
            products.ArgumentDataMember = "الصنف";
            products.ValueDataMembersSerializable = "المبيعات";
            products.ShowInLegend = true;
            products.Name = "";
            chartControl2.Series.Add(products);
            first_open = false;
            string x = "";
            return x;
        }
        private void XtraForm1_Activated(object sender, EventArgs e)
        {
            if (first_open == true) { return; }
            loading_data();
        }
        private void XtraForm1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (formclosing == false)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }

        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate{
                e.Result = loading_data();
            });
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressPanel1.Visible = false;
            progressPanel2.Visible = false;
            progressPanel3.Visible = false;

        }
    }
}