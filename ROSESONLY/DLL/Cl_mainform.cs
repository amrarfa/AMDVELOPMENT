using ROSESONLY.forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROSESONLY.DLL
{
    class Cl_mainform
    {
        public static DataTable dt_invoicedata;
        connectiondata cd = new connectiondata();
        public DataTable binding_permission(int user_id)
        {
            SqlParameter[] p = new SqlParameter[1];
            p[0] = new SqlParameter("@user_id", user_id);
            DataTable dt = new DataTable();
            dt = cd.getdata("sp_checkpermisions", p);
            return dt;
        }
        public bool checkpermissions(int user_id, string screen_name, string col, int type)
        {
            DataRow[] _findrows;
            _findrows = frm_othermain.getmain.dt_userpermissions.Select("screen_name='" + screen_name + "'");
            if (Convert.ToBoolean(_findrows[0][col].ToString()) == true)
            {
                return true;
            }
            else
            {
                if (type == 1)
                {
                    MSg.showmsg("لا يمكنك الدخول الى هذه الشاشه ", MSg.msgbutton.ok, MSg.msgicon.information);
                }
                else
                {

                }
                return false;
            }
        }
        public void create_datatable(int invoice_no,DateTime invoice_dte,string account,string store,decimal discount,
        decimal net_value,string user_name)
    {
        dt_invoicedata = new DataTable();
        dt_invoicedata.Columns.Add("invoice_no");
        dt_invoicedata.Columns.Add("invoice_date");
        dt_invoicedata.Columns.Add("accounts_name");
        dt_invoicedata.Columns.Add("store_name");
        dt_invoicedata.Columns.Add("discount");
        dt_invoicedata.Columns.Add("net_value");
        dt_invoicedata.Columns.Add("user_name");
        //insert rows
        DataRow _newrow;
        _newrow = dt_invoicedata.NewRow();
        _newrow["invoice_no"] =invoice_no;
        _newrow["invoice_date"] =invoice_dte;
        _newrow["accounts_name"] =account;
        _newrow["store_name"] =store;
        _newrow["discount"] =discount;
        _newrow["net_value"] =net_value;
        _newrow["user_name"] =user_name;
        dt_invoicedata.Rows.Add(_newrow);
    }
        public  void backup_datatbase()
        {
            string path = Properties.Settings.Default.backup_path + "\\Rose" + DateTime.Now.ToShortDateString().Replace('/', '-')
     + " - " + DateTime.Now.ToShortTimeString().Replace(':', '-');
            string StrQuery = "Backup Database DB_ROSES to Disk='" + path + ".bak'";
            connectiondata cd = new connectiondata();
            SqlCommand cmd = new SqlCommand(StrQuery, cd.con);
            cd.opencnnection();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                FolderBrowserDialog op = new FolderBrowserDialog();
                if (op.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.backup_path = op.SelectedPath;
                    Properties.Settings.Default.Save();
                }
            }
            cd.closconnection();
            cmd.Dispose();

        }

    }
}
