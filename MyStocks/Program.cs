using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStocks
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> list = new List<string>();
            List<List<byte>> result = new List<List<byte>>();
            byte[] aStock = new byte[16];
            byte[] head = new byte[5];
            FileStream selectedStocks = new FileStream(@"d:\temp\自选股1.BLK", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter targetWriter = new BinaryWriter(selectedStocks);
            for (int num = 8; num > 1; num--)
            {
                string file = @"d:\temp\自选股" + num + ".BLK";
                FileStream currentFile = new FileStream(file, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(currentFile);
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        head[i] = br.ReadByte();
                    }
                    if (num == 8) targetWriter.Write(head);
                    while (true)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < 16; i++)
                        {
                            byte temp = br.ReadByte();
                            if (i < 8) sb.Append((char)temp);
                            aStock[i] = temp;
                        }
                        if (!list.Contains(sb.ToString()))
                        {
                            list.Add(sb.ToString());
                            result.Add(aStock.ToList());
                        }
                    }
                }
                catch (EndOfStreamException)
                {

                }
                br.Close();
                currentFile.Close();
            }
            for (int i = result.Count - 1; i >= 0; i--)
            {
                targetWriter.Write(result[i].ToArray());
            }
            targetWriter.Close();
            selectedStocks.Close();
            //FileStream fs = new FileStream(@"d:\temp\自选股1.BLK", FileMode.Open, FileAccess.Read);
            //BinaryReader br = new BinaryReader(fs);
            //byte[] head = new byte[5];

            //try
            //{
            //    for (int i = 0; i < 5; i++)
            //    {
            //        head[i] = br.ReadByte();
            //    }
            //    while (true)
            //    {
            //        StringBuilder sb = new StringBuilder();
            //        for (int i = 0; i < 8; i++)
            //        {
            //            char ch = (char)br.ReadByte();
            //            sb.Append(ch);
            //        }
            //        list.Add(sb.ToString());
            //        for (int i = 0; i < 8; i++)
            //        {
            //            br.ReadByte();
            //        }
            //    }
            //}
            //catch (EndOfStreamException)
            //{


            //}
            //br.Close();
            //fs.Close();
            int count = 0;
            foreach (var item in list)
            {
                Console.WriteLine("{0}  {1}", ++count, item);
            }
            Console.ReadKey();
        }
    }
}
