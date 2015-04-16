using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;

namespace keyless_result_parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var inf = args[0];
            var outf = args[1];


            string[] files = new string[1]{inf};
            foreach (var fp in files)
            {
                using (var infs = new FileStream(fp, FileMode.Open))
                {
                    using (var sr = new StreamReader(infs))
                    {
                        File.Delete(outf);
                        File.Create(outf).Close();
                        using (var outfs = new FileStream(outf, FileMode.Truncate))
                        {
                            using (var sw = new StreamWriter(outfs))
                            {
                                var csv = new CsvWriter(sw);
                                try
                                {
                                    while (true)
                                    {
                                        var line = sr.ReadLine();
                                        if (line != " ok" && !line.StartsWith("make") && line.StartsWith(" 1000"))
                                        {

                                            var smashed = 0;
                                            var spaced = line.Split(' ');

                                            // fix for #ms
                                            smashed++;
                                            spaced[spaced.Length - 2] = spaced[spaced.Length - 2] + spaced[spaced.Length - 1];

                                            // fix for pipeline requests
                                            if (spaced[2] == "pipeline" && spaced[3] == "requests")
                                            {
                                                //smashed++;
                                                spaced[2] = spaced[2] + " " + spaced[3];
                                                for (var i = 4; i < spaced.Length-2;i++ )
                                                {
                                                    spaced[i - 1] = spaced[i];
                                                }
                                            }

                                            // fix for adjs after KSSL_ENUM => #ms
                                            // from spaced[4] -> spaced[length -2]
                                            var fix = "";
                                            for (var i = 4; i < spaced.Length - 2;i++ )
                                            {
                                                fix += spaced[i] + " ";
                                                smashed++;
                                            }
                                            spaced[4] = fix;
                                            smashed--;
                                            spaced[5] = spaced[spaced.Length - 2];

                                                // write em
                                                for (var i = 1; i < spaced.Length - smashed; i++)
                                                {
                                                    csv.WriteField(spaced[i]);
                                                }
                                            csv.NextRecord();
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
