using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace top.ebiz.service.Models.Traveler_Profile
{
    public class DocModel
    {
        public string token_login { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public DocBehalf behalf { get; set; }
        public string id_company { get; set; }
        public DocTravelType type_of_travel { get; set; }
        public string topic_of_travel { get; set; }
        public string travel { get; set; }
        public List<idModel> continent { get; set; }
        public List<DocCountry> country { get; set; }
        public List<DocProvince> province { get; set; }
        public string city { get; set; }
        public DateModel business_date { get; set; }
        public DateModel travel_date { get; set; }
        public string travel_objective_expected { get; set; }
        public List<Traveler> summary_table { get; set; }
        public initiatorModel initiator { get; set; }
        public afterTripModel after_trip { get; set; }
        public string remark { get; set; }
        public actionModel action { get; set; }
    }

    public class idModel
    {
        public string id { get; set; }
    }

    public class actionModel
    {
        public string type { get; set; }
        public string remark { get; set; }
    }
    public class afterTripModel    {
        public string opt1 { get; set; }
        public subAfterTripModel opt2 { get; set; } = new subAfterTripModel();
        public subAfterTripModel opt3 { get; set; } = new subAfterTripModel();
        public subAfterTripModel opt4 { get; set; } = new subAfterTripModel();
    }
    public class subAfterTripModel
    {
        public string status { get; set; }
        public string remark { get; set; }
    }
    public class initiatorModel
    {
        public string status { get; set; }
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string emp_organization { get; set; }
        public string remark { get; set; }
    }
    public class Traveler
    {
        public string dh_code { get; set; }
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string emp_organization { get; set; }
        public string continent_id { get; set; }
        public string continent { get; set; }
        public string country_id { get; set; }
        public string country_name { get; set; }
        public string province_id { get; set; }
        public string province_name { get; set; }
        public string city { get; set; }
        public DateModel business_date { get; set; } 
        public DateModel travel_date { get; set; }
        public string travel_duration { get; set; }
        public string gl_account { get; set; }
        public string cost { get; set; }
        public string order { get; set; }
        public string remark { get; set; }

    }
    public class DateModel
    {
        public string start { get; set; }
        public string stop { get; set; }
    }
    public class DocProvince
    {
        public string province_id { get; set; }
        public string province_name { get; set; }
    }
    public class DocCountry
    {
        public string contry_id { get; set; }
        public string country_id { get; set; }
        public string country_name { get; set; }
        public string continent_id { get; set; }
        public string continent_name { get; set; }
    }

    public class DocBehalf
    {
        public string status { get; set; }
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string emp_organization { get; set; }
    }
    public class DocTravelType
    {
        public string meeting { get; set; }
        public string siteVisite { get; set; }
        public string workshop { get; set; }
        public string roadshow { get; set; }
        public string conference { get; set; }
        public string other { get; set; }
        public string other_detail { get; set; }

        //DevFix 20220805 --> after go-live เพิ่ม Tick box = Training 
        public string training { get; set; }

    }

    public class genDocNoModel
    {
        public string token_login { get; set; }
        public string doc_type { get; set; }
    }



}