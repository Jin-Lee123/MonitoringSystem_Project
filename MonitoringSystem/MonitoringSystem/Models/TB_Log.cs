using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Models
{
    public class TB_Log
    {
        public int Seq { get; set; }
        public string Space { get; set; }
        public string Error { get; set; }
        public string Content { get; set; }

        public static readonly string SELECT_QUERY = @"SELECT Seq
                                                            , Space
                                                            , Error
                                                            , Content
                                                         FROM TB_Log";
    }
}
