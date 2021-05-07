using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Simulator
{
    public static class WorldExporter
    {
        static WorldExporter()
        {
            
        }

        public static void Export(string name)
        {
            using (StreamWriter output = new StreamWriter(Content.PathToSaves + name + Content.FileExtension))
            {
                output.Write(PackState());
                output.Write(PackParams());
                output.Write(PackUnits());
            }
            WorldImporter.UpdateFiles();
        }

        private static string PackState()
        {
            string result = $"#State:\n" +
                $"\tSeed={Simulator.Seed}\n" +
                $"\tTimer={Simulator.Timer}\n";
            return result;
        }

        private static string PackParams()
        {
            string result = $"#Params:\n" +
                $"\tGroundPower={Simulator.GroundPower}\n" +
                $"\tSunPower={Simulator.SunPower}\n" +
                $"\tDropChance={Simulator.DropChance}\n" +
                $"\tEnvDensity={Simulator.EnvDensity}\n";
            return result;
        }

        private static string PackUnits()
        {
            StringBuilder result = new StringBuilder($"#Units:\n", 10000);
            int i = 0;
            foreach (Unit unit in Simulator.Units)
            {
                result.Append($"\t#{i}:\n");
                string temp = $"\tEnergy={unit.Energy}\n" +
                    $"\tLastDirection={unit.LastDirection}\n" +
                    $"\tChlorophyl={unit.Chlorophyl}\n" +
                    $"\tCapacity={unit.Capacity}\n" +
                    $"\tStatus={(int)unit.Status}\n" +
                    $"\tPosition={unit.Coords[0]} {unit.Coords[1]}\n" +
                    $"\tDirection={unit.Direction[0]} {unit.Direction[1]}\n" +
                    $"\tGenes={UnitTextConfigurator.GetStringGenes(unit)}\n";
                result.Append(temp);
                i++;
            }
            return result.ToString();
        }
    }
}
