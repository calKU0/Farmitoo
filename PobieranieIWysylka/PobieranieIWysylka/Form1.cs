using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;
using System.IO;
using System.Configuration;

namespace PobieranieIWysylka
{
    public partial class Form1 : Form
    {
        string sftpHost = ConfigurationManager.AppSettings["SftpHostname"];
        int sftpPort = int.Parse(ConfigurationManager.AppSettings["SftpPort"]);
        string sftpUsername = ConfigurationManager.AppSettings["SftpUsername"];
        string sftpPassword = ConfigurationManager.AppSettings["SftpPassword"];
        private string sftpFolderPath = "/home/gaska/order";
        private string localPath = @"\\Backup\k\Farmitoo\Zamówienia\";
        private string sftpArchFolderPath = "/home/gaska/archive";

        public Form1()
        {
            try
            {
                InitializeComponent();
                using (var client = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
                {
                    client.Connect();
                    var files = client.ListDirectory(sftpFolderPath);
                    int i = files.Count() - 2;
                    client.Disconnect();

                    if (i != 0)
                    {
                        if (i == 1)
                        {
                            maskedTextBox1.AppendText(i + " plik do pobrania");
                        }
                        else if (i == 2 || i == 3 || i == 4)
                        {
                            maskedTextBox1.AppendText(i + " pliki do pobrania");
                        }
                        else
                        {
                            maskedTextBox1.AppendText(i + " plików do pobrania");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Wystąpił błąd " + e);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

            //Łączenie z serwerem
            using (var client = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
            {
                try
                {
                    client.Connect();

                    //Pobieranie wszystkich plików
                    var files = client.ListDirectory(sftpFolderPath);
                    int i = files.Count() - 2;
                    string[] empty = new string[i];
                    int j = 0;

                    foreach (var file in files)
                    {
                        if (!file.Name.StartsWith("."))
                        {
                            using (Stream fileStream = File.Create(localPath + file.Name))
                            {
                                client.DownloadFile(file.FullName, fileStream);
                                empty[j] = file.Name;
                                j++;
                            }
                        }
                    }
                    if (i != 0)
                    {

                        MessageBox.Show("Pomyślnie pobrano zamówienia");
                        maskedTextBox1.Clear();

                        //Przenoszenie wszystkich plików do folderu archiwalnego na sftp
                        foreach (var file in files)
                        {
                            if (file.IsDirectory) continue;
                            client.RenameFile(file.FullName, $"{sftpArchFolderPath}/{file.Name}");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Nie ma żadnych plików do pobrania!");

                    }
                    client.Disconnect();
                }


                catch (Exception k)
                {
                    client.Disconnect();
                    MessageBox.Show("Wystąpił błąd zawołaj dział IT: " + k);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 f2 = new Form2(); //Redirect do formularza 2
            f2.ShowDialog();
            this.Close();

        }

    }
}
