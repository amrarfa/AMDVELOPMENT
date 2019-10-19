using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace ROSESONLY.DLL
{
    class Cl_purchase
    {
        connectiondata cd = new connectiondata();
        public DataSet binding_productsandnavigations(string move_type,int store_id)
        {
            SqlParameter[] p = new SqlParameter[3];
            p[0] = new SqlParameter("@user_code", connectiondata.user_id);
            p[1] = new SqlParameter("@move_type", move_type);
            p[2] = new SqlParameter("@store_id", store_id);
            DataSet dts = new DataSet();
            dts = cd.get_dataset("sp_updateaftersave", p);
            return dts;
        }
        
    }
}
