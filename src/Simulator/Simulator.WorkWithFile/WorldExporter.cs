using System.IO;
using System.Text;

namespace Simulator
{
    public static class WorldExporter
    {
        static WorldExporter()
        {
            
        }

        public static void Export(string name, Simulator world)
        {
            if (name.Equals(""))
            {
                Events.ErrorHandler.KnockKnock(null, "World saving error. Empty name.", false);
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

        private static string PackState(Simulator world)
        {
            string result = $"#State:\n" +
                $"\tSeed={world.Seed}\n" +
                $"\tTimer={world.Timer}\n";
            return result;
        }

        private static string PackParams(Simulator world)
        {
            string result = $"#Params:\n" +
                $"\tGroundPower={world.GroundPower}\n" +
                $"\tSunPower={world.SunPower}\n" +
                $"\tDropChance={world.DropChance}\n" +
                $"\tEnvDensity={world.EnvDensity}\n";
            return result;
        }

        private static string PackUnits(Simulator world)
        {
            StringBuilder result = new StringBuilder($"#Units:\n", 10000);
            int i = 0;
            foreach (Unit unit in world.Units)
            {
                result.Append($"\t#{i}:\n");
                string temp = $"\tEnergy={unit.Energy}\n" +
                    $"\tLastDirection={unit.LastDirection}\n" +
                    $"\tChlorophyl={unit.Chlorophyl}\n" +
                    $"\tCapacity={unit.Capacity}\n" +
                    $"\tStatus={(int)unit.Status}\n" +
                    $"\tPosition={unit.Coords[0]} {unit.Coords[1]}\n" +
                    $"\tDirection={unit.Direction[0]} {unit.Direction[1]}\n" +
                    $"\tGenes={GetStringGenes(unit)}\n";
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
    }
}
