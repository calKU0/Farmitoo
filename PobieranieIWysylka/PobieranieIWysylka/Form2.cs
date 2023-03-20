using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
        private string sftpFolderPath = "/home/gaska/order";
        private string localPath = @"C:\temp\";
        private string archivefolder = @"\\Backup\k\Foldery pracowników\Krzysztof Kurowski\Archiwalne (test na farmitoo)\";
        private string pattern = @"^confirmation_\d{14}\.csv$"; //Pattern który matchuje pliki zaczynające się na confirmation_, potem 14 cyfr i rozszerzenie CSV

        public Form2()
        {
            InitializeComponent();
            var filesToUpload = Directory.GetFiles(localPath)
                                                    .Where(file => Regex.IsMatch(Path.GetFileName(file), pattern))
                                                    .ToArray();

            foreach (string file in filesToUpload)
            {
                listBox1.Items.Add(Path.GetFileName(file).ToString() + Environment.NewLine);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            using (var client = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {

                        // Dokopywanie się do plików, które matchują patternowi
                        var filesToUpload = Directory.GetFiles(localPath)
                                                    .Where(file => Regex.IsMatch(Path.GetFileName(file), pattern))
                                                    .ToArray();

                        if (filesToUpload.Length != 0)
                        {
                            Wysyłka();
                        }
                        else
                        {
                            MessageBox.Show("Nie ma żadnych plików do przesłania");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się połączyć z serwerem sftp!");
                    }

                }
                catch (Exception m)
                {
                    client.Disconnect();
                    MessageBox.Show("Wystąpił błąd: " + m);
                }

                void Wysyłka()
                {
                    var filesToUpload = Directory.GetFiles(localPath)
                            .Where(file => Regex.IsMatch(Path.GetFileName(file), pattern))
                            .ToArray();

                    client.ChangeDirectory(sftpFolderPath);

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

                    MessageBox.Show("Pomyślnie przesłano pliki");
                    listBox1.Items.Clear();
                    client.Disconnect();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1(); //Redirect do formularza 1
            f1.ShowDialog();
        }

    }
}
