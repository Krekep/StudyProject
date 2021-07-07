using Simulator.World;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Simulator
{
    public static class WorldImporter
    {
        private static string[] files;

        private static int seed;
        private static int timer;

        private static int groundPower;
        private static int sunPower;
        private static double envDensity;
        private static double dropChance;

        private static List<Unit> units;

        static WorldImporter()
        {
            files = Directory.GetFiles(Content.PathToSaves, "*" + Content.FileExtension);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Substring(Content.PathToSaves.Length, files[i].Length - Content.PathToSaves.Length - Content.FileExtension.Length);
            }
        }
        public static void Import(string fileName, Swamp world)
        {
            using (StreamReader input = new StreamReader(Content.PathToSaves + fileName + Content.FileExtension))
            {
                string line = input.ReadLine();
                if (line.Equals("#State:"))
                {
                    if (!ReadState(input))
                    {
                        Events.ErrorHandler.KnockKnock(null, "Swamp loading error. Invalid save.", false);
                        return;
                    }
                }
                else
                {
                    Events.ErrorHandler.KnockKnock(null, "Swamp loading error. Invalid save.", false);
                    return;
                }
                line = input.ReadLine();
                if (line.Equals("#Params:"))
                {
                    if (!ReadParams(input))
                    {
                        Events.ErrorHandler.KnockKnock(null, "Swamp loading error. Invalid save.", false);
                        return;
                    }
                }
                else
                {
                    Events.ErrorHandler.KnockKnock(null, "Swamp loading error. Invalid save.", false);
                    return;
                }
                line = input.ReadLine();
                if (line.Equals("#Units:"))
                {
                    if (!ReadUnits(input))
                    {
                        Events.ErrorHandler.KnockKnock(null, "Swamp loading error. Invalid save.", false);
                        return;
                    }
                }
                else
                {
                    Events.ErrorHandler.KnockKnock(null, "Swamp loading error. Invalid save.", false);
                    return;
                }
            }
            world.Import(seed, timer, groundPower, sunPower, envDensity, dropChance, units);
        }

        private static bool ReadUnits(StreamReader input)
        {
            string line = input.ReadLine();
            bool fl = true;

            units = new List<Unit>();
            int energy = 0;
            int lastDirection = 0;
            int chlorophyl = 0;
            int attackPower = 0;
            int parent = 0;
            int capacity = 0;
            int status = 0;
            int[] position = new int[2];
            int[] direction = new int[2];
            IAction[][] genes = new IAction[2][];


            while (line != null && line[1] == '#' && fl)
            {
                try
                {
                    fl = false;
                    line = input.ReadLine();
                    fl = fl || int.TryParse(line.Trim().Substring("Energy=".Length), out energy);
                    line = input.ReadLine();
                    fl = fl && int.TryParse(line.Trim().Substring("LastDirection=".Length), out lastDirection);
                    line = input.ReadLine();
                    fl = fl && int.TryParse(line.Trim().Substring("Chlorophyl=".Length), out chlorophyl);
                    line = input.ReadLine();
                    fl = fl && int.TryParse(line.Trim().Substring("AttackPower=".Length), out attackPower);
                    line = input.ReadLine();
                    fl = fl && int.TryParse(line.Trim().Substring("Capacity=".Length), out capacity);
                    line = input.ReadLine();
                    fl = fl && int.TryParse(line.Trim().Substring("Parent=".Length), out parent);
                    line = input.ReadLine();
                    fl = fl && int.TryParse(line.Trim().Substring("Status=".Length), out status);
                    line = input.ReadLine();
                    position = line.Trim().Substring("Position=".Length).Split(' ').Select(Int32.Parse).ToArray();
                    line = input.ReadLine();
                    direction = line.Trim().Substring("Direction=".Length).Split(' ').Select(Int32.Parse).ToArray();
                    line = input.ReadLine();
                    genes = StringToGenesArray(line.Trim().Substring("Genes=".Length), '|');
                    fl = fl && (position.Length == 2) && (direction.Length == 2);
                }
                catch
                {
                    fl = false;
                }
                units.Add(new Unit(energy, lastDirection, capacity, chlorophyl, attackPower, status, position, direction, genes, parent));
                line = input.ReadLine();
            }
            return fl;
        }

        private static bool ReadState(StreamReader input)
        {
            string line = input.ReadLine();
            bool fl = int.TryParse(line.Trim().Substring("Seed=".Length), out seed);
            line = input.ReadLine();
            fl = fl && int.TryParse(line.Trim().Substring("Timer=".Length), out timer);
            return fl;
        }

        private static bool ReadParams(StreamReader input)
        {
            string line = input.ReadLine();
            bool fl = int.TryParse(line.Trim().Substring("GroundPower=".Length), out groundPower);
            line = input.ReadLine();
            fl = fl && int.TryParse(line.Trim().Substring("SunPower=".Length), out sunPower);
            line = input.ReadLine();
            fl = fl && double.TryParse(line.Trim().Substring("DropChance=".Length), out dropChance);
            line = input.ReadLine();
            fl = fl && double.TryParse(line.Trim().Substring("EnvDensity=".Length), out envDensity);
            return fl;
        }

        public static void UpdateFiles()
        {
            files = Directory.GetFiles(Content.PathToSaves, "*" + Content.FileExtension);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Substring(Content.PathToSaves.Length, files[i].Length - Content.PathToSaves.Length - Content.FileExtension.Length);
            }
        }

        public static string[] GetFilesArray()
        {
            return files;
        }

        private static IAction[][] StringToGenesArray(string input, char separator)
        {
            string[] temp = input.Split(separator);
            IAction[][] result = new IAction[temp.Length][];
            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = new IAction[temp[i].Length];
                int seq = 0;
                bool isParsed = int.TryParse(Reverse(temp[i]), out seq);
                if (!isParsed)
                {
                    Events.ErrorHandler.KnockKnock(null, "Error in applying unit genes. Invalid number.", false);
                    return null;
                }
                for (int j = 0; j < temp[i].Length; j++)
                {
                    switch (seq % 10)
                    {
                        case (int)ActionType.Wait:
                            result[i][j] = new Wait();
                            break;
                        case (int)ActionType.Move:
                            result[i][j] = new Move();
                            break;
                        case (int)ActionType.Photosyntesis:
                            result[i][j] = new Photosyntesis();
                            break;
                        case (int)ActionType.Attack:
                            result[i][j] = new Attack();
                            break;
                        default:
                            result[i][j] = new Wait();
                            break;
                    }
                    seq /= 10;
                }
            }
            Events.ErrorHandler.KnockKnock(null, "Successful applying of unit parameters.", true);
            return result;
        }

        private static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
