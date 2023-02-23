using System;
using System.Linq;
using System.Collections.Generic;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;
using System.Configuration;
using System.IO.Compression;
using Chilkat;

namespace PobieranieZamówieniaFarmitoo
{
    public class Program
    {
        static void Main(string[] args)
        {
            string sftpHost = "xxxx";
            int sftpPort = 22;
            string sftpUsername = "xxxx";
            string sftpPassword = "xxxx";
            string sftpFolderPath = "/home/gaska/order";
            string localPath = @"C:\temp\";
            string sftpArchFolderPath = "/home/gaska/archive/";

            Chilkat.SFtp sftpp = new Chilkat.SFtp();

            bool success = sftpp.Connect(sftpHost, sftpPort);
            if (success == true)
            {
                success = sftpp.AuthenticatePw(sftpUsername, sftpPassword);
            }

            if (success == true)
            {
                success = sftpp.InitializeSftp();
                Console.WriteLine("Pomyślnie połączono z serwerem " + sftpHost);
            }

            if (success != true)
            {
                Console.WriteLine(sftpp.LastErrorText);
                return;
            }

            // Mode 0 causes SyncTreeDownload to download all files.
            int mode = 0;
            // Do not recursively descend the remote directory tree.  Just download all the files in specified directory.
            bool recursive = false;

            success = sftpp.SyncTreeDownload(sftpFolderPath, localPath, mode, recursive);
            if (success != true)
            {
                Console.WriteLine(sftpp.LastErrorText);
                return;
            }
            else
            {
                using (var client = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
                {
                    client.Connect();
                    var files = client.ListDirectory(sftpFolderPath);

                    foreach (var file in files)
                    {
                        client.RenameFile(file.FullName, $"{sftpArchFolderPath}/{file.Name}");
                    }
                    client.Disconnect();
                }
                Console.WriteLine("Pomyślnie pobrano pliki z katalogu " + sftpFolderPath);
            }
        }

    }
}
