using System;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Renci.SshNet;
using System.IO;
using System.Linq;
using System.Configuration;

namespace PobieranieIWysylka
{
    public partial class Form2 : Form
    {
        string sftpHost = ConfigurationManager.AppSettings["SftpHostname"];
        int sftpPort = int.Parse(ConfigurationManager.AppSettings["SftpPort"]);
        string sftpUsername = ConfigurationManager.AppSettings["SftpUsername"];
        string sftpPassword = ConfigurationManager.AppSettings["SftpPassword"];
        string sftpFolderPath = ConfigurationManager.AppSettings["sftpConfirmFolderPath"];
        private string localConfirmationPath = ConfigurationManager.AppSettings["localConfirmationPath"];
        private string trackingarchivefolder = ConfigurationManager.AppSettings["trackingArchiveFolder"];
        private string confirmationarchivefolder = ConfigurationManager.AppSettings["confirmationArchiveFolder"];
        private string confirmationPattern = ConfigurationManager.AppSettings["confirmationPattern"]; //Pattern który matchuje pliki zaczynające się na confirmation_, potem 14 cyfr i rozszerzenie CSV
        private string trackingPattern = ConfigurationManager.AppSettings["trackingPattern"]; //Pattern który matchuje pliki zaczynające się na tracking_, potem 14 cyfr i rozszerzenie CSV
        private string sftpTrackingFolder = ConfigurationManager.AppSettings["sftpTrackingFolder"];
        private string localTrackingFolder = ConfigurationManager.AppSettings["localTrackingFolder"];

        public Form2()
        {
            InitializeComponent();
            //Dodawanie plików do pierwszej listy
            var conf_files = Directory.GetFiles(localConfirmationPath)
                                                    .Where(file => Regex.IsMatch(Path.GetFileName(file), confirmationPattern))
                                                    .ToArray();

            foreach (string file in conf_files)
            {
                listBox1.Items.Add(Path.GetFileName(file).ToString() + Environment.NewLine);
            }

            //Dodawanie plików do drugiej listy
            var tracking_files = Directory.GetFiles(localTrackingFolder)
                                        .Where(file => Regex.IsMatch(Path.GetFileName(file), trackingPattern))
                                        .ToArray();

            foreach (string file in tracking_files)
            {
                listBox2.Items.Add(Path.GetFileName(file).ToString() + Environment.NewLine);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Wysyłka(localConfirmationPath, confirmationPattern, sftpFolderPath, confirmationarchivefolder);
            Wysyłka(localTrackingFolder, trackingPattern, sftpTrackingFolder, trackingarchivefolder);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1(); //Redirect do formularza 1
            f1.ShowDialog();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Wysyłka(localConfirmationPath, confirmationPattern, sftpFolderPath, confirmationarchivefolder);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Wysyłka(localTrackingFolder, trackingPattern, sftpTrackingFolder, trackingarchivefolder);
        }

        void Wysyłka(string local_folder, string pattern, string sftp_folder, string archivefolder)
        {
            using (var client = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        var filesToUpload = Directory.GetFiles(local_folder)
                                .Where(file => Regex.IsMatch(Path.GetFileName(file), pattern))
                                .ToArray();

                        client.ChangeDirectory(sftp_folder);

                        // Wysyłanie plików na sftp
                        foreach (string file in filesToUpload)
                        {
                            using (FileStream fs = new FileStream(file, FileMode.Open))
                            {
                                client.BufferSize = 1024;
                                client.UploadFile(fs, Path.GetFileName(file));
                            }
                            File.Move(file, archivefolder + Path.GetFileName(file)); // Przenoszenie pliku do folderu archiwalnego
                        }
                        if (local_folder == localTrackingFolder)
                        {
                            MessageBox.Show("Pomyślnie przesłano tracking listy");
                            listBox2.Items.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Pomyślnie przesłano potwierdzenia zamównień");
                            listBox1.Items.Clear();
                        }
                        client.Disconnect();
                    }
                    else
                    {
                        MessageBox.Show("Nie połączona z serwerem SFTP");
                    }
                }
                catch (Exception m)
                {
                    client.Disconnect();
                    MessageBox.Show("Wystąpił błąd: " + m);
                }
            }
        }
    }
}
