using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Models
{
    public class TB_Line
    {
        public int Plantcode { get; set; }
        public int GoalQty { get; set; }
        public int TotalQty { get; set; }
        public int ProdQty { get; set; }
        public int BadQty { get; set; }
        public string Woker { get; set; }

        public static readonly string SELECT_QUERY = @"SELECT Plantcode
                                                            , GoalQty
                                                            , TotalQty
                                                            , ProdQty
                                                            , BadQty
                                                            , Woker
                                                         FROM TB_Line";
    }
}
