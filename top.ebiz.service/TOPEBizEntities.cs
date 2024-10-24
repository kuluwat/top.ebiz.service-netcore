using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using top.ebiz.service.Models.Create_Trip;

namespace top.ebiz.service
{
    public partial class TOPEBizEntities : DbContext
    {
        // สร้าง constructor ที่ไม่มีพารามิเตอร์และดึง connection string จาก appsettings.json
        public TOPEBizEntities()
            : base(GetOptions(GetConnectionStringFromAppSettings()))
        {
        }

        // Constructor ที่รับ connection string เป็นพารามิเตอร์
        public TOPEBizEntities(string connectionString)
            : base(GetOptions(connectionString))
        {
        }

        // Constructor ที่รับ DbContextOptions สำหรับการเชื่อมต่อฐานข้อมูล (ใช้กับ DI)
        public TOPEBizEntities(DbContextOptions<TOPEBizEntities> options)
            : base(options)
        {
        }

        // ใช้ IConfiguration เพื่อดึง connection string จาก appsettings.json
        private static string GetConnectionStringFromAppSettings()
        {
            // สร้าง IConfiguration เพื่ออ่านไฟล์ appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // ดึง connection string จาก appsettings.json
            return config.GetConnectionString("eBizConnection");
        }

        // กำหนด options สำหรับการสร้าง context ด้วย connection string สำหรับ Oracle
        private static DbContextOptions GetOptions(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TOPEBizEntities>();
            optionsBuilder.UseOracle(connectionString);
            return optionsBuilder.Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // กำหนดค่าหรือข้อจำกัดเพิ่มเติม (ถ้ามี)
            modelBuilder.Entity<BZ_USERS>().HasNoKey();
            modelBuilder.Entity<DocDetail2Model>().HasNoKey(); 
        }


        // เพิ่มฟังก์ชัน ConvertTypeParameter ในคลาสนี้
        public OracleParameter ConvertTypeParameter(string paramName, object value, string type= "char", int defLength = 1)
        {
            OracleParameter param = new OracleParameter();
            param.ParameterName = paramName ;

            switch (type.ToLower())
            {
                case "char":
                    param.OracleDbType = OracleDbType.Char;
                    param.Size = value != null ? value.ToString().Length : defLength; // กำหนดขนาด
                    param.Value = value ?? DBNull.Value; // ถ้าไม่มีค่าให้เป็น DBNull
                    break;

                case "int":
                    param.OracleDbType = OracleDbType.Int32;
                    param.Value = value != null && int.TryParse(value.ToString(), out int intValue) ? intValue : DBNull.Value;
                    break;

                case "number":
                    param.OracleDbType = OracleDbType.Decimal;
                    param.Value = value != null && decimal.TryParse(value.ToString(), out decimal decimalValue) ? decimalValue : DBNull.Value;
                    break;

                case "date":
                    param.OracleDbType = OracleDbType.Date;
                    param.Value = value != null && DateTime.TryParse(value.ToString(), out DateTime dateValue) ? dateValue : DBNull.Value;
                    break;

                default:
                    param.OracleDbType = OracleDbType.Varchar2; // Default เป็น string
                    param.Value = value ?? DBNull.Value;
                    break;
            }

            return param;
        }


        // รายการ DbSet สำหรับแต่ละ entity (ตารางในฐานข้อมูล)
        public DbSet<BZ_USERS> BZ_USERS { get; set; }
        public DbSet<BZ_DOC_TRAVEL_TYPE> BZ_DOC_TRAVEL_TYPE { get; set; }
        public DbSet<BZ_DOC_CONTIENT> BZ_DOC_CONTIENT { get; set; }
        public DbSet<BZ_DOC_COUNTRY> BZ_DOC_COUNTRY { get; set; }
        public DbSet<BZ_DOC_PROVINCE> BZ_DOC_PROVINCE { get; set; }
        public DbSet<BZ_MASTER_COUNTRY> BZ_MASTER_COUNTRY { get; set; }
        public DbSet<BZ_MASTER_CONTINENT> BZ_MASTER_CONTINENT { get; set; }
        public DbSet<BZ_DOC_RUNNING> BZ_DOC_RUNNING { get; set; }
        public DbSet<BZ_DOC_HEAD> BZ_DOC_HEAD { get; set; }
        public DbSet<BZ_DOC_ACTION> BZ_DOC_ACTION { get; set; }
        public DbSet<BZ_DOC_TRAVELER_APPROVER> BZ_DOC_TRAVELER_APPROVER { get; set; }
        public DbSet<BZ_DOC_TRAVELER_EXPENSE> BZ_DOC_TRAVELER_EXPENSE { get; set; }
        public DbSet<BZ_DOC_TRAVELER> BZ_DOC_TRAVELER { get; set; }

        public DbSet<approverModel> ApproverModels { get; set; }
        public DbSet<ContinentDocModel> ContinentDocModels { get; set; }
        public DbSet<CountryDocModel> CountryDocModels { get; set; }
        public DbSet<doc2ApproverModel> Doc2ApproverModels {get; set;}
        public DbSet<DocHeadModel> DocHeadModels { get; set; }
        
        public DbSet<DocDetailModel> DocDetailModels { get; set; }
        public DbSet<DocDetail2Model> DocDetail2Models { get; set; }
        public DbSet<DocDetail3HeadModel> DocDetail3HeadModels { get; set; }
        public DbSet<DocFileListModel> DocFileListModels { get; set; }
        public DbSet<DocList2Model> DocList2Models { get; set; }
        public DbSet<DocList3Model> DocList3Models { get; set; }
        // public class DocHeadModel
        // {
        //     public string document_status { get; set; }
        //     public string DH_CODE { get; set; }
        //     public Nullable<decimal> DH_VERSION { get; set; }
        //     public Nullable<decimal> DH_DOC_STATUS { get; set; }
        //     public Nullable<System.DateTime> DH_DATE { get; set; }
        //     public string DH_COM_CODE { get; set; }
        //     public string DH_TYPE { get; set; }
        //     public string DH_BEHALF_EMP_ID { get; set; }
        //     public string ENNAME { get; set; }
        //     public string COMPANYCODE { get; set; }
        //     public string COMPANYNAME { get; set; }
        //     public string DH_TOPIC { get; set; }
        //     public string DH_TRAVEL { get; set; }
        //     public Nullable<System.DateTime> DH_BUS_FROMDATE { get; set; }
        //     public string bus_start { get; set; }
        //     public Nullable<System.DateTime> DH_BUS_TODATE { get; set; }
        //     public string bus_stop { get; set; }
        //     public Nullable<System.DateTime> DH_TRAVEL_FROMDATE { get; set; }
        //     public string travel_start { get; set; }
        //     public Nullable<System.DateTime> DH_TRAVEL_TODATE { get; set; }
        //     public string travel_stop { get; set; }
        //     public Nullable<decimal> DH_TRAVEL_DAYS { get; set; }
        //     public string DH_TRAVEL_OBJECT { get; set; }
        //     public string DH_INITIATOR_EMPID { get; set; }
        //     public string INITIATOR_NAME { get; set; }
        //     public string INITIATOR_COM { get; set; }
        //     public string DH_INITIATOR_REMARK { get; set; }
        //     public string DH_AFTER_TRIP_OPT1 { get; set; }
        //     public string DH_AFTER_TRIP_OPT2 { get; set; }
        //     public string DH_AFTER_TRIP_OPT3 { get; set; }
        //     public string DH_AFTER_TRIP_OPT2_REMARK { get; set; }
        //     public string DH_REMARK { get; set; }
        //     public string DH_AFTER_TRIP_OPT3_REMARK { get; set; }
        //     public string DH_CITY { get; set; }
        //     public Nullable<decimal> DH_TOTAL_PERSON { get; set; }
        //     public Nullable<System.DateTime> DH_CREATE_DATE { get; set; }


        //     //DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:not flow, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW
        //     public string DH_TYPE_FLOW { get; set; }

        //     //DevFix 20210622 0000 เพิ่มข้อมูล ประเภทพนักงาน 1:Employee, 2:Contract
        //     public string REQUEST_USER_TYPE { get; set; }
        //     public string REQUEST_USER_NAME { get; set; }
        // }

        public DbSet<EmployeeDoc2Model> EmployeeDoc2Models { get; set; }
       
        public DbSet<ProvinceDocModel> ProvinceDocModels { get; set; }
        public DbSet<SearchCAPModel> SearchCAPModels { get; set; }
        public DbSet<SearchUserModel> SearchUserModels { get; set; }
        public DbSet<StatusDocModel> StatusDocModels { get; set; }

        public DbSet<TravelerApproverConditionModel> TravelerApproverConditionModels { get; set; }
        public DbSet<TravelerDocModel> TravelerDocModels { get; set; }
        public DbSet<TravelerDoc2Model> TravelerDoc2Models { get; set; }


        public DbSet<TypeModel> TypeModels { get; set; }

    }
}
