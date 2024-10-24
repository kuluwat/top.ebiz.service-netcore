
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using top.ebiz.service.Models.Create_Trip;

namespace top.ebiz.service.Service.Create_Trip
{
    public class masterService
    {
        string sql = "";
        // WBS หรือ IO
        public List<WBSOutModel> getWBS(WBSInputModel value)
        {
            var data = new List<WBSOutModel>();
            var value_text = value.text ?? "";

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
            {
                sql = " select distinct IO wbs,  COST_CENTER_RESP cost_center from BZ_MASTER_IO where ROWNUM < 1000 ";
                if (!string.IsNullOrEmpty(value_text))
                {
                    sql += " and upper(IO) like upper('%' || :value_text  ||'%') ";
                }
                sql += " order by IO ";

                var parameters = new List<OracleParameter>();
                parameters.Add(context.ConvertTypeParameter("value_text", value_text, "char"));
                data = context.WBSOutModelList.FromSqlRaw(sql, parameters.ToArray()).ToList();

            }

            return data;
        }

        public List<CCOutModel> getCostCenter(CCInputModel value)
        {
            var data = new List<CCOutModel>();
            var value_text = value.text ?? "";

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
            {
                sql = " select distinct COST_CENTER_RESP code from BZ_MASTER_IO where ROWNUM < 1000 ";
                if (!string.IsNullOrEmpty(value_text))
                {
                    sql += " and upper(COST_CENTER_RESP) like upper('%'|| :value_text ||'%') ";
                }
                sql += " order by COST_CENTER_RESP ";

                var parameters = new List<OracleParameter>();
                parameters.Add(context.ConvertTypeParameter("value_text", value_text, "char"));
                data = context.CCOutModelList.FromSqlRaw(sql, parameters.ToArray()).ToList();

            }

            return data;
        }

        public List<GLOutModel> getGLAccount(GLInputModel value)
        {
            var data = new List<GLOutModel>();
            var value_text = value.text ?? "";

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
            {
                sql = " select distinct GL_NO code from BZ_MASTER_GL where ROWNUM < 1000 ";
                if (!string.IsNullOrEmpty(value.text))
                {
                    sql += " and upper(GL_NO) like '%'|| :value_text ||'%') ";
                }
                sql += " order by GL_NO ";

                var parameters = new List<OracleParameter>();
                parameters.Add(context.ConvertTypeParameter("value_text", value_text, "char"));
                data = context.GLOutModelList.FromSqlRaw(sql, parameters.ToArray()).ToList();

            }

            return data;
        }



        public List<RequestTypeResultModel> getRequestType(RequestTypeModel value)
        {
            var data = new List<RequestTypeResultModel>();

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
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

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
            {
                sql = @" select com_code as com_id, com_name as com_name, sort_by as com_sort_by from bz_master_company order by sort_by";

                var parameters = new List<OracleParameter>();
                data = context.CompanyResultModelList.FromSqlRaw(sql, parameters.ToArray()).ToList();

                //if (data != null && data.Count() > 0) { }
            }

            return data;
        }

        public List<ContinentResultModel> getContinuent(ContinentModel value)
        {
            var data = new List<ContinentResultModel>();

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
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

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
            {
                var parameters = new List<OracleParameter>();
                sql = @" select to_char(a.ctn_id) continent_id, a.ctn_name continent, to_char(b.ct_id)country_id, b.ct_name country
                         from bz_master_continent a inner join bz_master_country b on a.ctn_id = b.ctn_id
                         where a.ctn_id is not null ";

                if (value.continent != null && value.continent.Count() > 0)
                {
                    var index = 0;
                    sql += " and (";
                    foreach (var item in value.continent)
                    {
                        var item_value = item.id ?? "";
                        var param_name = $"p{index}";

                        sql += $"{(index > 0 ? " or " : "")} a.ctn_id = :{param_name} ";
                        parameters.Add(context.ConvertTypeParameter(param_name, item_value, "char"));
                        index++;
                    }
                    sql += " ) ";
                }
                sql += " order by  b.ct_name ";

                data = context.CountryResultModelList.FromSqlRaw(sql, parameters.ToArray()).ToList();

            }

            return data;
        }

        public List<ProvinceResultModel> getProvince(ProvinceModel value)
        {
            var data = new List<ProvinceResultModel>();

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
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

            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
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
            using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
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