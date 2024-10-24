using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using top.ebiz.service.Models.Create_Trip;

namespace top.ebiz.service.Service.Create_trip
{
    public class masterService
    {
        // WBS หรือ IO
        public List<WBSOutModel> getWBS(WBSInputModel value)
        {
            var data = new List<WBSOutModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                string sql = "";
                sql = " select      IO wbs,  COST_CENTER_RESP cost_center ";
                sql += " from BZ_MASTER_IO ";
                sql += " where ROWNUM < 1000 ";
                if (!string.IsNullOrEmpty(value.text))
                {
                    sql += " and upper(IO) like '%" + value.text.Trim().ToUpper().Replace("'", "''") + "%' ";
                }
                sql += " order by IO ";

                data = context.Database.SqlQuery<WBSOutModel>(sql).ToList();

            }

            return data;
        }

        public List<CCOutModel> getCostCenter(CCInputModel value)
        {
            var data = new List<CCOutModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                string sql = "";
                sql = " select     distinct COST_CENTER_RESP code ";
                sql += " from BZ_MASTER_IO ";
                sql += " where ROWNUM < 1000 ";
                if (!string.IsNullOrEmpty(value.text))
                {
                    sql += " and upper(COST_CENTER_RESP) like '%" + value.text.Trim().ToUpper().Replace("'", "''") + "%' ";
                }
                sql += " order by COST_CENTER_RESP ";

                data = context.Database.SqlQuery<CCOutModel>(sql).ToList();

            }

            return data;
        }

        public List<GLOutModel> getGLAccount(GLInputModel value)
        {
            var data = new List<GLOutModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                string sql = "";
                sql = " select      GL_NO code ";
                sql += " from BZ_MASTER_GL ";
                sql += " where ROWNUM < 1000 ";
                if (!string.IsNullOrEmpty(value.text))
                {
                    sql += " and upper(GL_NO) like '%" + value.text.Trim().ToUpper().Replace("'", "''") + "%' ";
                }
                sql += " order by GL_NO ";

                data = context.Database.SqlQuery<GLOutModel>(sql).ToList();

            }

            return data;
        }



        public List<RequestTypeResultModel> getRequestType(RequestTypeModel value)
        {
            var data = new List<RequestTypeResultModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "bz_sp_getRequestType";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_token", value.token_login));

                    OracleParameter oraP = new OracleParameter();
                    oraP.ParameterName = "mycursor";
                    oraP.OracleDbType = OracleDbType.RefCursor;
                    oraP.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oraP);

                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            var schema = reader.GetSchemaTable();
                            data = reader.MapToList<RequestTypeResultModel>() ?? new List<RequestTypeResultModel>();

                        }
                        catch (Exception ex) { }
                    }

                }
            }

            return data;
        }

        public List<CompanyResultModel> getCompany(CompanyModel value)
        {
            var data = new List<CompanyResultModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    //connection.Open();
                    //DbCommand cmd = connection.CreateCommand();
                    //cmd.CommandText = "bz_sp_get_company";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add(new OracleParameter("p_token", value.token_login));

                    //OracleParameter oraP = new OracleParameter();
                    //oraP.ParameterName = "mycursor";
                    //oraP.OracleDbType = OracleDbType.RefCursor;
                    //oraP.Direction = ParameterDirection.Output;
                    //cmd.Parameters.Add(oraP); 
                    //using (var reader = cmd.ExecuteReader())
                    //{
                    //    try
                    //    {
                    //        var schema = reader.GetSchemaTable();
                    //        data = reader.MapToList<CompanyResultModel>() ?? new List<CompanyResultModel>();

                    //    }
                    //    catch (Exception ex) { }
                    //} 
                    var sql = @" select com_code as com_id, com_name as com_name, sort_by as com_sort_by
                                 from bz_master_company order by sort_by";
                    data = context.Database.SqlQuery<CompanyResultModel>(sql).ToList();
                    if (data != null && data.Count() > 0)
                    {
                    }
                }
            }

            return data;
        }

        public List<ContinentResultModel> getContinuent(ContinentModel value)
        {
            var data = new List<ContinentResultModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "bz_sp_get_continent";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_token", value.token_login));

                    OracleParameter oraP = new OracleParameter();
                    oraP.ParameterName = "mycursor";
                    oraP.OracleDbType = OracleDbType.RefCursor;
                    oraP.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oraP);

                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            var schema = reader.GetSchemaTable();
                            data = reader.MapToList<ContinentResultModel>() ?? new List<ContinentResultModel>();
                        }
                        catch (Exception ex) { }
                    }

                }
            }

            return data;
        }

        public List<CountryResultModel> getCountry(CountryModel value)
        {
            var data = new List<CountryResultModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                string sql = "";
                sql = " select      to_char(a.ctn_id) continent_id ";
                sql += "            , a.ctn_name continent";
                sql += "            , to_char(b.ct_id)country_id";
                sql += "            , b.ct_name country";
                sql += " from       bz_master_continent a inner";
                sql += " join       bz_master_country b on a.ctn_id = b.ctn_id";
                if (value.continent != null && value.continent.Count() > 0)
                {
                    string subsql = "";
                    foreach (var item in value.continent)
                    {
                        subsql += ", " + item.id ?? "";
                    }

                    if (subsql.Length > 0)
                    {
                        subsql = subsql.Substring(1);
                    }

                    sql += " where a.ctn_id in (" + subsql + ") ";
                }
                sql += " order by  b.ct_name ";
                data = context.Database.SqlQuery<CountryResultModel>(sql).ToList();
            }

            return data;
        }

        public List<ProvinceResultModel> getProvince(ProvinceModel value)
        {
            var data = new List<ProvinceResultModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "bz_sp_get_province";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_token", value.token_login));
                    cmd.Parameters.Add(new OracleParameter("p_country", value.country_id));

                    OracleParameter oraP = new OracleParameter();
                    oraP.ParameterName = "mycursor";
                    oraP.OracleDbType = OracleDbType.RefCursor;
                    oraP.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oraP);

                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            var schema = reader.GetSchemaTable();
                            data = reader.MapToList<ProvinceResultModel>() ?? new List<ProvinceResultModel>();
                        }
                        catch (Exception ex) { }
                    }

                }
            }

            return data;
        }

        public List<EmpSearchResultModel> getEmployee(EmpSearchModel value)
        {
            var data = new List<EmpSearchResultModel>();

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "bz_sp_get_employee";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_token", value.token_login));
                    cmd.Parameters.Add(new OracleParameter("p_empid", value.emp_id));
                    cmd.Parameters.Add(new OracleParameter("p_empname", value.emp_name));

                    OracleParameter oraP = new OracleParameter();
                    oraP.ParameterName = "mycursor";
                    oraP.OracleDbType = OracleDbType.RefCursor;
                    oraP.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oraP);

                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            var schema = reader.GetSchemaTable();
                            data = reader.MapToList<EmpSearchResultModel>() ?? new List<EmpSearchResultModel>();
                        }
                        catch (Exception ex) { }
                    }

                }
            }

            return data;
        }

        public void getTest()
        {
            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                var data = new List<CompanyModel>();
                var data2 = new List<CompanyModel>();
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "bz_sp_getCompany";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_token", ""));

                    OracleParameter oraP = new OracleParameter();
                    oraP.ParameterName = "mycursor";
                    oraP.OracleDbType = OracleDbType.RefCursor;
                    oraP.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oraP);

                    OracleParameter oraP2 = new OracleParameter();
                    oraP2.ParameterName = "mycursor2";
                    oraP2.OracleDbType = OracleDbType.RefCursor;
                    oraP2.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oraP2);

                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {

                            var schema = reader.GetSchemaTable();
                            data = reader.MapToList<CompanyModel>() ?? new List<CompanyModel>();
                            reader.NextResult();
                            data2 = reader.MapToList<CompanyModel>() ?? new List<CompanyModel>();
                        }
                        catch (Exception ex) { }
                    }

                }
            }
        }


    }
}