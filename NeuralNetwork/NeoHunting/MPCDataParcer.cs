using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;

namespace NeoHunting
{
    public class Pair<T1, T2>
    {
        public Pair(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
    }

    public class MPCDataParcer
    {
        private string GeneralDBPath = @"D:\Projects\NeoTestData\Old\MPCORB_modified.txt";
        private string NeoDBPath = @"D:\Projects\NeoTestData\NEAs.txt";
        private string NeoOutputPath = @"D:\Projects\NeoTestData\NotNeas5.txt";
        private List<Pair<double, double>> minMaxes = new List<Pair<double,double>>()
        {
            new Pair<double, double>(0.3, 24.3),
            new Pair<double, double>(0.00005, 359.99924),
            new Pair<double, double>(0.0001, 359.99925),
            new Pair<double, double>(0.00018, 359.99986),
            new Pair<double, double>(0.00457, 172.86794)
            //new Pair<double, double>(0.0000245, 0.97)
        };


        public List<double> ParseAndConvertToDouble(string testLine)
        {
            var components = testLine.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<double>();

            result.Add(Convert.ToDouble(components[1].Replace(".", ",")));
            result.Add(Convert.ToDouble(components[4].Replace(".", ",")));
            result.Add(Convert.ToDouble(components[5].Replace(".", ",")));
            result.Add(Convert.ToDouble(components[6].Replace(".", ",")));
            result.Add(Convert.ToDouble(components[7].Replace(".", ",")));
            //result.Add(Convert.ToDouble(components[8].Replace(".", ",")));

            return result;
        }

        public List<double> Normalize(List<double> input)
        {
            var normalizedList = new List<double>();

            for (var i = 0; i < input.Count; i++)
            {
                var normalizedValue = (input[i] - minMaxes[i].Item1)/minMaxes[i].Item2 - minMaxes[i].Item1;

                normalizedList.Add(normalizedValue);
            }

            return normalizedList;
        }

        public void GetNonNeosByTYpeCode()
        {
            var generalDbFile = File.ReadAllLines(GeneralDBPath);
            
            var result = new List<string>();

            foreach (var line in generalDbFile)
            {
                if (line == string.Empty)
                {
                    result.Add(line);
                    break;
                }
                //var spaceIndex = line.IndexOf(' ');
                //var lineCode = line.Substring(0, spaceIndex);

                var components = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (components[20] == "000A")
                    components[20] = "0010";

                double type;
                if (!Double.TryParse(components[20], out type)) //.Replace(".", ","));
                    continue;


                var notNeo = type == 5;//(int)type < 11 && type > 7;

                //bool neoFound = neoCodes.Contains(lineCode);

                if (notNeo)//!neoFound)
                {
                    result.Add(line);
                }
            }

            File.WriteAllLines(NeoOutputPath, result);

        }

        public void GetNonNeos()
        {
            var generalDbFile = File.ReadAllLines(GeneralDBPath);

            var result = new List<string>();
            var neoCodes = NeoCodes();

            foreach (var line in generalDbFile)
            {
                if (line == string.Empty)
                {
                    break;
                    //result.Add(line);
                }
                var spaceIndex = line.IndexOf(' ');
                var lineCode = line.Substring(0, spaceIndex);

                bool neoFound = neoCodes.Contains(lineCode);

                if (!neoFound)
                {
                    result.Add(line);
                }
            }

            File.WriteAllLines(NeoOutputPath, result);

        }

        public void GetMinMaxes()
        {
            var generalDbFile = File.ReadAllLines(GeneralDBPath);

            foreach (var line in generalDbFile)
            {
                if (line == string.Empty)
                {
                    break;
                }

                var components = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                var specificComponents = new List<string>();

                specificComponents.Add(components[1]);
                specificComponents.Add(components[4]);
                specificComponents.Add(components[5]);
                specificComponents.Add(components[6]);
                specificComponents.Add(components[7]);
                //specificComponents.Add(components[8]);

                for (var i = 0; i < specificComponents.Count; i++)
                {
                    var componentDouble = Convert.ToDouble(specificComponents[i].Replace(".", ","));
                    if (componentDouble < minMaxes[i].Item1)
                    {
                        minMaxes[i].Item1 = Math.Abs(componentDouble);
                    }
                    if (componentDouble > minMaxes[i].Item2)
                    {
                        minMaxes[i].Item2 = componentDouble;
                    }
                }
            }

            var sb = new List<string>();

            foreach (var minMax in minMaxes)
            {
                sb.Add(minMax.Item1 + " - " + minMax.Item2);
            }

            File.WriteAllLines(@"D:\hackaton\minMaxes.txt", sb);

        }

        public List<TeachSequence> ParseToTeachSeqs(string negativeClass, string positiveClass)
        {

            var result = new List<TeachSequence>();

            var neoTeachLines = File.ReadAllLines(positiveClass);
            foreach (var line in neoTeachLines)
            {
                var normalized = Normalize(ParseAndConvertToDouble(line));
                result.Add(new TeachSequence() { Values = normalized, IsCorrect = 1 });
            }

            var notNeoTeachLines = File.ReadAllLines(negativeClass);
            foreach (var line in notNeoTeachLines)
            {
                var normalized = Normalize(ParseAndConvertToDouble(line));
                result.Add(new TeachSequence() { Values = normalized, IsCorrect = 0 });
            }

            return result;
        }

        private List<string> NeoCodes()
        {
            var neoDbFile = File.ReadAllLines(NeoDBPath);
            var result = new List<string>();

            foreach (var line in neoDbFile)
            {
                int spaceNeoIndex = line.IndexOf(' ');
                var neoLineCode = line.Substring(0, spaceNeoIndex);
                result.Add(neoLineCode);
            }

            return result;
        }



    }
}
