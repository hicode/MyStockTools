using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrganizeSelection
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要移动自选股吗？", "确认", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;
            string path = @"D:\dzh2\USERDATA\block\自选股";
            File.Delete(path + "2.BLK");
            for (int i = 2; i < 8; i++)
            {
                string file1 = path + i + ".BLK";
                string file2 = path + (i + 1) + ".BLK";
                File.Move(file2, file1);
            }
            FileStream file8 = new FileStream(path + "8.BLK", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(file8);
            byte[] head = { 0xa6, 0x20, 0x51, 0xff, 0x01 };
            bw.Write(head);
            bw.Close();
            file8.Close();
            MessageBox.Show("自选股移动完成！");
        }

        private void btnUnion_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            List<List<byte>> result = new List<List<byte>>();
            byte[] aStock = new byte[16];
            byte[] head = new byte[5];
            string path = @"D:\dzh2\USERDATA\block\自选股";
            FileStream selectedStocks = new FileStream(path + "1.BLK", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter targetWriter = new BinaryWriter(selectedStocks);
            for (int num = 8; num > 1; num--)
            {
                string file = path + num + ".BLK";
                FileStream currentFile = new FileStream(file, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(currentFile);
                var oneDay = new List<List<byte>>();
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
                            oneDay.Add(aStock.ToList());
                        }
                    }
                }
                catch (EndOfStreamException)
                {

                }
                oneDay.Sort((item1, item2) =>
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (item1[i] < item2[i]) return 1;
                        else if (item1[i] > item2[i]) return -1;
                    }
                    return 0;
                });
                result.AddRange(oneDay);
                br.Close();
                currentFile.Close();
            }
            for (int i = result.Count - 1; i >= 0; i--)
            {
                targetWriter.Write(result[i].ToArray());
            }
            targetWriter.Close();
            selectedStocks.Close();
            MessageBox.Show("合并完成！");
        }
    }
}
