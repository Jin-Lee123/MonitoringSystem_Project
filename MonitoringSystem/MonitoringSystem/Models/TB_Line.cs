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
                                                         FROM TB_Line";
        public static readonly string UPDATE_QUERY = @"UPDATE TB_Line
                                                          SET   GoalQty   = @GoalQty
                                                              , TotalQty  = @TotalQty
                                                              , ProdQty   = @ProdQty
                                                              , BadQty    = @BadQty
                                                        WHERE Plantcode = 1000";
    }
}
