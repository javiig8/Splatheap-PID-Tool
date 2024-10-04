using splatheap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;






namespace cemuModMenuForSplatoon
{

    public partial class MainForm : Form
    {
        public static IntPtr ProcessPtr;

        public static ProcessWrapper myProcessWrapper;

        public static long baseAddress;


        public static long pow16(int exp)
        {
            if (exp == 0)
                return 1;
            return 16 * pow16(exp - 1);
        }


        public MainForm()
        {
            myProcessWrapper = new ProcessWrapper();

            ProcessPtr = myProcessWrapper.openProcess(0x001F0FFF, false);



            try
            {
                string path = myProcessWrapper.getMainModuleFileName();
                path = path.Remove(path.Length - 8);
                TextReader txt = new StreamReader(File.Open(path + "log.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                char lastChar = '\0';
                char currentChar = '\0';
                while (lastChar != '0' || currentChar != 'x')
                {
                    lastChar = currentChar;
                    currentChar = (char)txt.Read();
                }
                long tempBaseAddress = 0;
                for (int i = 15; i >= 0; i--)
                {
                    int c = txt.Read();
                    if (c > '9')
                        c -= 'a' - 10;
                    else
                        c -= '0';
                    tempBaseAddress += c * pow16(i);
                }
                baseAddress = tempBaseAddress;
                txt.Close();
            }
            catch (Exception)
            {

            }




            if (myProcessWrapper.openProcess(0x001F0FFF, false) != new IntPtr(0))
            {
                InitializeComponent();
            }
            else
                MessageBox.Show("Splatoon not running!" + " Make sure your cemu executable is named Cemu.exe and that there's only one Cemu window running!");

        }



        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void QuestionMarkButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Javi (DC: javi.ig) \n\nSpecial thanks to bl for sharing the PID pointer!\n\n Thanks to Raffy (DC: raffy3000) for helping with the original Splatheap code!.\n\nTo use just click on *Update Player List and PID's*. Keep in mind the values will only be updated during a match!");
        }







        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            (Process.GetCurrentProcess()).Kill();

        }

        private void btnDisplayPID_Click(object sender, EventArgs e)
        {
            Label[] PIDLabels = { player1PID, player2PID, player3PID, player4PID, player5PID, player6PID, player7PID, player8PID };
            Label[] PIDDecimalLabels = { player1PIDDec, player2PIDDec, player3PIDDec, player4PIDDec, player5PIDDec, player6PIDDec, player7PIDDec, player8PIDDec };
            Label[] nameLabels = { player1name, player2name, player3name, player4name, player5name, player6name, player7name, player8name };

            for (int playerID = 0; playerID < 8; playerID++)
            {
                byte[] pointer = new byte[4];
                MainForm.myProcessWrapper.readProcessMemory(MainForm.ProcessPtr, (IntPtr)MainForm.baseAddress + 0x101DD330, pointer, 4, new IntPtr(0));
                Array.Reverse(pointer, 0, pointer.Length);
                IntPtr p = new IntPtr(MainForm.baseAddress + BitConverter.ToInt32(pointer, 0) + 0x10);
                MainForm.myProcessWrapper.readProcessMemory(MainForm.ProcessPtr, p, pointer, 4, new IntPtr(0));
                Array.Reverse(pointer, 0, pointer.Length);
                p = new IntPtr(MainForm.baseAddress + BitConverter.ToInt32(pointer, 0) + playerID * 4);
                MainForm.myProcessWrapper.readProcessMemory(MainForm.ProcessPtr, p, pointer, 4, new IntPtr(0));
                Array.Reverse(pointer, 0, pointer.Length);
                p = new IntPtr(MainForm.baseAddress + BitConverter.ToInt32(pointer, 0) + 0xd0);
                MainForm.myProcessWrapper.readProcessMemory(MainForm.ProcessPtr, p, pointer, 4, new IntPtr(0));

                string nnidHex = BitConverter.ToString(pointer).Replace("-", "");
                int nnidDec = BitConverter.ToInt32(pointer.Reverse().ToArray(), 0);

                PIDLabels[playerID].Text = $"PID (HEX): {nnidHex}";
                PIDLabels[playerID].ForeColor = Color.Blue;
                PIDDecimalLabels[playerID].Text = $"PID (DEC): {nnidDec}";
                PIDDecimalLabels[playerID].ForeColor = Color.Red;
            }

            for (int playerID = 0; playerID < 8; playerID++)
            {
                byte[] pointer = new byte[4];
                byte[] nameBytes = new byte[40];
                MainForm.myProcessWrapper.readProcessMemory(MainForm.ProcessPtr, (IntPtr)MainForm.baseAddress + 0x101DD330, pointer, 4, new IntPtr(0));
                Array.Reverse(pointer, 0, pointer.Length);
                IntPtr p = new IntPtr(MainForm.baseAddress + BitConverter.ToInt32(pointer, 0) + 0x10);
                MainForm.myProcessWrapper.readProcessMemory(MainForm.ProcessPtr, p, pointer, 4, new IntPtr(0));
                Array.Reverse(pointer, 0, pointer.Length);
                p = new IntPtr(MainForm.baseAddress + BitConverter.ToInt32(pointer, 0) + playerID * 4);
                MainForm.myProcessWrapper.readProcessMemory(MainForm.ProcessPtr, p, pointer, 4, new IntPtr(0));
                Array.Reverse(pointer, 0, pointer.Length);
                p = new IntPtr(MainForm.baseAddress + BitConverter.ToInt32(pointer, 0) + 0x6);
                MainForm.myProcessWrapper.readProcessMemory(MainForm.ProcessPtr, p, nameBytes, 40, new IntPtr(0));

                string name = Encoding.BigEndianUnicode.GetString(nameBytes);
                name = name.Replace("\n", "").Replace("\r", ""); // Remove newline characters
                nameLabels[playerID].Text = $"Player {playerID + 1}: {name}";
            }
        }
    }
}