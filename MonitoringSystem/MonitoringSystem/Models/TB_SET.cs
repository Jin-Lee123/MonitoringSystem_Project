using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Models
{
    public class TB_SET
    {
        public float SPlantT { get; set; }
        public float SPlantH { get; set; }
        public float SRobotArm { get; set; }
        public float SConveyor { get; set; }
        public float SPumpT { get; set; }
        public float SFlowRate { get; set; }
        public float SDensity { get; set; }

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
                                            