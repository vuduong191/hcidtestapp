using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp.Models
{
    public class Bims2ViewModel
    {
        public Nullable<int> StatementNum { get; set; }
        public string StatementDate { get; set; }
        public long APN { get; set; }
        public string Property_Address { get; set; }
        public string Property_City_State_Zip { get; set; }
        public Nullable<int> RSO_Exemptions { get; set; }
        public Nullable<int> SCEP_Exmpetions { get; set; }
        public string IS_RSO { get; set; }
        public string IS_SCEP { get; set; }
        public string RSO_Invoice_Num { get; set; }
        public string SCEP_Invoice_Num { get; set; }
        public Nullable<int> Total_Units { get; set; }
        public Nullable<int> RSO_Units_Billed { get; set; }
    }
}