using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace luigi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://vignette2.wikia.nocookie.net/deathbattle/images/2/24/Luigi_(Poltergust_5000).png", "luigi.png");
            }
            //DisplayPicture(@"./luigi.png", true);
            string file = "luigi.png";
            string current = Path.GetDirectoryName(Application.ExecutablePath);
            string hopeful = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\luigi";
            //MessageBox.Show(hopeful);
            Directory.CreateDirectory(hopeful);
            string source = Path.Combine(current, file);
            string after = Path.Combine(hopeful, file);
            if (File.Exists(after))
            {
                File.Delete(after);
            }
            File.Move(source, after);
            DisplayPicture(after, true);
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 2.ToString());
            Process currentProcess = Process.GetCurrentProcess();
            string pid = currentProcess.Id.ToString();
            var start = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = "taskkill /f /pid " + pid + "; exit",
                CreateNoWindow = true
            };
            var process = Process.Start(start);
        }
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, uint fWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x14;
        private const uint SPIF_UPDATEINIFILE = 0x1;
        private const uint SPIF_SENDWININICHANGE = 0x2;

        private void DisplayPicture(string file_name, bool update_registry)
        {
            try
            {
                // If we should update the registry,
                // set the appropriate flags.
                uint flags = 0;
                if (update_registry)
                    flags = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;

                // Set the desktop background to this file.
                if (!SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0, file_name, flags))
                {
                    MessageBox.Show("SystemParametersInfo failed.",
                        "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying picture " +
                    file_name + ".\n" + ex.Message,
                    "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }
    }
}
