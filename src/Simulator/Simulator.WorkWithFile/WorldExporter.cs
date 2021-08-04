using System.IO;
using System.Text;

using Simulator.World;

namespace Simulator
{
    public static class WorldExporter
    {
        static WorldExporter()
        {
            
        }

        public static void Export(string name, Swamp world)
        {
            if (name.Equals(""))
            {
                Events.ErrorHandler.KnockKnock(null, "Swamp saving error. Empty name.", false);
                return;
            }
            using (StreamWriter output = new StreamWriter(Content.PathToSaves + name + Content.FileExtension))
            {
                output.Write(PackState(world));
                output.Write(PackParams(world));
                output.Write(PackUnits(world));
            }
            WorldImporter.UpdateFiles();
            Events.ErrorHandler.KnockKnock(null, "Success world saving.", true);
        }

        private static string PackState(Swamp world)
        {
            string result = $"#State:\n" +
                $"\tSeed={world.Seed}\n" +
                $"\tTimer={world.Timer}\n";
            return result;
        }

        private static string PackParams(Swamp world)
        {
            string result = $"#Params:\n" +
                $"\tGroundPower={world.GroundPower}\n" +
                $"\tSunPower={world.SunPower}\n" +
                $"\tDropChance={world.DropChance}\n" +
                $"\tEnvDensity={world.EnvDensity}\n";
            return result;
        }

        private static string PackUnits(Swamp world)
        {
            StringBuilder result = new StringBuilder($"#Units:\n", 10000);
            int i = 0;
            foreach (int unit in world.Units.UnitsNumbers)
            {
                result.Append($"\t#{unit}:\n");
                string temp = $"\tEnergy={world.Units.UnitsEnergy[unit]}\n" +
                    $"\tLastDirection={world.Units.UnitsLastDirection[unit]}\n" +
                    $"\tChlorophyl={world.Units.UnitsChlorophyl[unit]}\n" +
                    $"\tAttackPower={world.Units.UnitsAttackPower[unit]}\n" +
                    $"\tCapacity={world.Units.UnitsCapacity[unit]}\n" +
                    $"\tParent={world.Units.UnitsParent[unit]}\n" +
                    $"\tStatus={(int)world.Units.UnitsStatus[unit]}\n" +
                    $"\tPosition={world.Units.UnitsCoords[unit][0]} {world.Units.UnitsCoords[unit][1]}\n" +
                    $"\tDirection={world.Units.UnitsDirection[unit][0]} {world.Units.UnitsDirection[unit][1]}\n" +
                    $"\tGenes={GetStringGenes(world.Units.UnitsGenes[unit])}\n";
                result.Append(temp);
                i++;
            }
            return result.ToString();
        }

        public static string GetStringGenes(Unit unit)
        {
            StringBuilder genes = new StringBuilder(10);
            IAction[][] temp = unit.Genes;
            for (int i = 0; i < temp.Length - 1; i++)
            {
                for (int j = 0; j < temp[i].Length; j++)
                    genes.Append((int)temp[i][j].Type());
                genes.Append("|");
            }
            for (int j = 0; j < temp[temp.Length - 1].Length; j++)
                genes.Append((int)temp[temp.Length - 1][j].Type());
            return genes.ToString();
        }

        private static string GetStringGenes(IAction[][] genes)
        {
            StringBuilder result = new StringBuilder(10);
            for (int i = 0; i < genes.Length - 1; i++)
            {
                for (int j = 0; j < genes[i].Length; j++)
                    result.Append((int)genes[i][j].Type());
                result.Append("|");
            }
            for (int j = 0; j < genes[genes.Length - 1].Length; j++)
                result.Append((int)genes[genes.Length - 1][j].Type());
            return result.ToString();
        }
    }
}
