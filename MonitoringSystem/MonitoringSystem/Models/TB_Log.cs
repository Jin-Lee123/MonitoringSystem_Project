using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Models
{
    public class TB_Log
    {
        public int ID { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string AdditionalInfo { get; set; }
        public string LoggedOnDate { get; set; }

        public static readonly string SELECT_QUERY = @"SELECT ID
                                                            , Level
                                                            , Message
                                                            , AdditionalInfo
                                                            , LoggedOnDate
                                                         FROM Logs";
    }
}
