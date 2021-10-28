using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Models
{
    public class TB_SET
    {
        public double SPlantT { get; set; }
        public double SPlantH { get; set; }
        public double SRobotArm { get; set; }
        public double SConveyor { get; set; }
        public double SPumpT { get; set; }
        public double SFlowRate { get; set; }
        public double SDensity { get; set; }

        public static readonly string SELECT_QUERY = @"SELECT PlantT
                                                            , PlantH
                                                            , RobotArm
                                                            , Conveyor
                                                            , PumpT
                                                            , FlowRate
                                                            , Density
                                                         FROM TB_SET";
        public static readonly string UPDATE_QUERY = @"UPDATE TB_SET
                                                          SET PlantT = @PlantT
                                                              , PlantH   = @PlantH
                                                              , RobotArm = @RobotArm
                                                              , Conveyor = @Conveyor
                                                              , PumpT    = @PumpT
                                                              , FlowRate = @FlowRate
                                                              , Density  = @Density
                                                        WHERE Id = 1";
    }
}
                                            