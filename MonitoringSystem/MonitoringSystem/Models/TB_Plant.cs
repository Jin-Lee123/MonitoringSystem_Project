using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Models
{
    public class TB_Plant
    { 
    public float PlantT { get; set; }
    public float PlantH { get; set; }
    public float RobotArm { get; set; }
    public float Conveyor { get; set; }
    public float PumpT { get; set; }

        public static readonly string SELECT_QUERY = @"SELECT PlantT
                                                       FROM TB_SETTING
                                                       WHERE currtime = '{currtime}'";
    }
}
