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
        public string Dev_addr { get; set; }
        public string CurrTime { get; set; }
        public string Code { get; set; }
        public double Value { get; set; }
        public double Sensor { get; set; }


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
        public static readonly string SELECT_QUERY2 = @"SELECT ORG.CurrTime, ORG.Code, FORMAT(AVG(ORG.Sensor), 'N2') AS Sensor
                                                          FROM
                                                        (
                                                        SELECT Dev_addr
                                                            , LEFT(CONVERT(VARCHAR, CurrTime, 120), 16) AS CurrTime
                                                            , Code
                                                            , Value
                                                            , Sensor
                                                            FROM TB_LINETEMP
                                                        WHERE Code = 'RobotTemp'
                                                          AND convert(varchar(10), CurrTime, 102) = convert(varchar(10), getdate(), 102)
                                                        ) AS ORG
                                                        GROUP BY ORG.CurrTime, ORG.Code";
        public static readonly string SELECT_QUERY3 = @"SELECT ORG.CurrTime, ORG.Code, FORMAT(AVG(ORG.Sensor), 'N2') AS Sensor
                                                          FROM
                                                        (
                                                        SELECT Dev_addr
                                                            , LEFT(CONVERT(VARCHAR, CurrTime, 120), 16) AS CurrTime
                                                            , Code
                                                            , Value
                                                            , Sensor
                                                            FROM TB_LINETEMP
                                                        WHERE Code = 'ConveyTemp'
                                                          AND convert(varchar(10), CurrTime, 102) = convert(varchar(10), getdate(), 102)
                                                        ) AS ORG
                                                        GROUP BY ORG.CurrTime, ORG.Code";
    }
}
