using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using top.ebiz.service.Models.Create_Trip;
using static top.ebiz.service.TOPEBizCreateTripEntities;

namespace top.ebiz.service
{
    public partial class TOPEBizCreateTripEntities : DbContext
    {
        #region function main
        // สร้าง constructor ที่ไม่มีพารามิเตอร์และดึง connection string จาก appsettings.json
        public TOPEBizCreateTripEntities()
            : base(GetOptions(GetConnectionStringFromAppSettings()))
        {
        }

        // Constructor ที่รับ connection string เป็นพารามิเตอร์
        public TOPEBizCreateTripEntities(string connectionString)
            : base(GetOptions(connectionString))
        {
        }

        // Constructor ที่รับ DbContextOptions สำหรับการเชื่อมต่อฐานข้อมูล (ใช้กับ DI)
        public TOPEBizCreateTripEntities(DbContextOptions<TOPEBizCreateTripEntities> options)
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
            return config.GetConnectionString("eBizConnection") ?? "";
        }

        // กำหนด options สำหรับการสร้าง context ด้วย connection string สำหรับ Oracle
        private static DbContextOptions GetOptions(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TOPEBizCreateTripEntities>();
            optionsBuilder.UseOracle(connectionString);
            return optionsBuilder.Options;
        }
        // เพิ่มฟังก์ชัน ConvertTypeParameter ในคลาสนี้
        public OracleParameter ConvertTypeParameter(string paramName, object value, string type = "char", int defLength = 4000)
        {
            OracleParameter param = new OracleParameter();
            param.ParameterName = paramName;

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

        #endregion function main

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //DbSet => Insert Update Delete 
            // โมเดลที่ต้องระบุว่าไม่มี Primary Key 
            modelBuilder.Entity<BZ_USERS>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_TRAVEL_TYPE>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_CONTIENT>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_COUNTRY>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_PROVINCE>().HasNoKey();
            modelBuilder.Entity<BZ_MASTER_COUNTRY>().HasNoKey();
            modelBuilder.Entity<BZ_MASTER_CONTINENT>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_RUNNING>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_HEAD>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_TRAVELER_APPROVER>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_TRAVELER_EXPENSE>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_TRAVELER>().HasNoKey();

            modelBuilder.Entity<BZ_DOC_ACTION>().HasNoKey();
            modelBuilder.Entity<VW_BZ_USERS>().HasNoKey();

            // โมเดลที่เป็น View หรือ Stored Procedure (ไม่มี Primary Key)
            modelBuilder.Entity<DocHeadModel>().HasNoKey();
            modelBuilder.Entity<DocDetail3HeadModel>().HasNoKey();
            modelBuilder.Entity<DocList2Model>().HasNoKey();
            modelBuilder.Entity<DocList3Model>().HasNoKey();
            modelBuilder.Entity<docFlow2_travel>().HasNoKey();
            modelBuilder.Entity<doc2ApproverModel>().HasNoKey();
            modelBuilder.Entity<DocFileListModel>().HasNoKey();
            modelBuilder.Entity<allApproveModel>().HasNoKey();
            modelBuilder.Entity<SearchCAP_TraverlerModel>().HasNoKey();
            modelBuilder.Entity<SearchUserModel>().HasNoKey();
            modelBuilder.Entity<SearchCAPModel>().HasNoKey();
            modelBuilder.Entity<CountryDocModel>().HasNoKey();
            modelBuilder.Entity<ProvinceDocModel>().HasNoKey();
            modelBuilder.Entity<TelephoneModel>().HasNoKey();
            modelBuilder.Entity<costcenter_io>().HasNoKey();
            modelBuilder.Entity<gl_account>().HasNoKey();
            modelBuilder.Entity<tempModel>().HasNoKey();
            modelBuilder.Entity<ExchangeRatesModel>().HasNoKey();
            modelBuilder.Entity<ContinentDocModel>().HasNoKey();
            modelBuilder.Entity<employeeDoc2Model>().HasNoKey();
            modelBuilder.Entity<approverModel>().HasNoKey();
            modelBuilder.Entity<MasterCostCenter>().HasNoKey();
            modelBuilder.Entity<TravelerUsers>().HasNoKey();
            modelBuilder.Entity<travelerDoc2Model>().HasNoKey();
            modelBuilder.Entity<BZ_DOC_TRAVELER_APPROVER_V2>().HasNoKey();
            modelBuilder.Entity<TravelerDocModel>().HasNoKey();
            modelBuilder.Entity<StatusDocModel>().HasNoKey();

            // โมเดล master ที่เป็น View หรือ Stored Procedure
            modelBuilder.Entity<WBSOutModel>().HasNoKey();
            modelBuilder.Entity<CCOutModel>().HasNoKey();
            modelBuilder.Entity<GLOutModel>().HasNoKey();
            modelBuilder.Entity<CompanyResultModel>().HasNoKey();
            modelBuilder.Entity<CountryResultModel>().HasNoKey();

            // traveler summary service ที่เป็น View หรือ Stored Procedure
            modelBuilder.Entity<TravelerApproverConditionModel>().HasNoKey();
            modelBuilder.Entity<TravelerDocHead>().HasNoKey();
            modelBuilder.Entity<RestApproverListModel>().HasNoKey();
            modelBuilder.Entity<ApproverConditionModel>().HasNoKey();
            modelBuilder.Entity<TravelerExpense>().HasNoKey();

            // estimate expense service
            modelBuilder.Entity<EstExpTravelDateModel>().HasNoKey();
            modelBuilder.Entity<EstExpSAPModel>().HasNoKey();

        }

        #region DbSet
        public DbSet<NormalModel> NormalModelList { get; set; }

        // รายการ DbSet สำหรับแต่ละ entity (ตารางในฐานข้อมูล ???ต้องเป็น field ทั้งหมดนะ เดียวเช็คอีกทีนะ)
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

        public DbSet<VW_BZ_USERS> VW_BZ_USERS { get; set; }

        //********** set doc service ???เป็น View / Stored Procedure 
        public DbSet<DocHeadModel> DocHeadModelList { get; set; }
        public DbSet<DocDetail3HeadModel> DocDetail3HeadModelList { get; set; }
        public DbSet<DocList2Model> DocList2ModelList { get; set; }
        public DbSet<DocList3Model> DocList3ModelList { get; set; }

        public DbSet<docFlow2_travel> DocFlow2TravelList { get; set; }
        public DbSet<doc2ApproverModel> Doc2ApproverModelList { get; set; }

        public DbSet<DocFileListModel> DocFileListModelList { get; set; }


        public DbSet<allApproveModel> AllApproveModelList { get; set; }
        public DbSet<SearchCAP_TraverlerModel> SearchCAP_TraverlerModelList { get; set; }
        public DbSet<SearchUserModel> SearchUserModelList { get; set; }
        public DbSet<SearchCAPModel> SearchCAPModelList { get; set; }

        public DbSet<CountryDocModel> CountryDocModelList { get; set; }
        public DbSet<ProvinceDocModel> ProvinceDocModelList { get; set; }
        public DbSet<TelephoneModel> TelephoneModelList { get; set; }
        public DbSet<costcenter_io> CostcenterIOList { get; set; }
        public DbSet<gl_account> GLAccountList { get; set; }
        public DbSet<tempModel> TempModelList { get; set; }
        public DbSet<ExchangeRatesModel> ExchangeRatesModelList { get; set; }
        public DbSet<ContinentDocModel> ContinentDocModelList { get; set; }

        public DbSet<employeeDoc2Model> EmployeeDoc2ModelList { get; set; }
        public DbSet<approverModel> ApproverModelList { get; set; }
        public DbSet<MasterCostCenter> MasterCostCenterList { get; set; }
        public DbSet<TravelerUsers> TravelerUsersModelList { get; set; }
        public DbSet<travelerDoc2Model> TravelerDoc2ModelList { get; set; }
        public DbSet<BZ_DOC_TRAVELER_APPROVER_V2> TravelApproveList { get; set; }

        public DbSet<TravelerDocModel> TravelerDocModelList { get; set; }
        public DbSet<StatusDocModel> StatusDocModelList { get; set; }


        //********** master ???เป็น View / Stored Procedure 
        public DbSet<WBSOutModel> WBSOutModelList { get; set; }
        public DbSet<CCOutModel> CCOutModelList { get; set; }
        public DbSet<GLOutModel> GLOutModelList { get; set; }
        public DbSet<CompanyResultModel> CompanyResultModelList { get; set; }
        public DbSet<CountryResultModel> CountryResultModelList { get; set; }


        //********** traveler summary service ???เป็น View / Stored Procedure 
        public DbSet<TravelerApproverConditionModel> TravelerApproverConditionModelList { get; set; }
        public DbSet<TravelerDocHead> TravelerDocHeadList { get; set; }
        public DbSet<RestApproverListModel> RestApproverListModelList { get; set; }
        public DbSet<ApproverConditionModel> ApproverConditionModelList { get; set; }
        public DbSet<TravelerExpense> TravelerExpenseList { get; set; }


        //********** estimate expense service
        public DbSet<EstExpTravelDateModel> EstExpTravelDateModelList { get; set; }
        public DbSet<EstExpSAPModel> EstExpSAPModelList { get; set; }

        #endregion DbSet

        #region Model
        public class NormalModel
        {
            public string id { get; set; }
            public string name { get; set; }
            public string text { get; set; }
        }

        #endregion Model
    }


}
