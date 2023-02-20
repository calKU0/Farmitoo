using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using Chilkat;

namespace PobieranieZamówieniaFarmitoo
{
    class Program
    {
        static void Main(string[] args)
        {
            string sftpHost = "xxxx";
            int sftpPort = 22;
            string sftpUsername = "xxxx";
            string sftpPassword = "xxxx";
            string sftpFolderPath = "xxxx";
            string localPath = @"C:\temp\";

            Chilkat.SFtp sftp = new Chilkat.SFtp();

            bool success = sftp.Connect(sftpHost, sftpPort);
            if (success == true)
            {
                success = sftp.AuthenticatePw(sftpUsername, sftpPassword);
            }

            if (success == true)
            {
                success = sftp.InitializeSftp();
                Console.WriteLine("Pomyślnie połączono z serwerem " + sftpHost);
            }

            if (success != true)
            {
                Console.WriteLine(sftp.LastErrorText);
                return;
            }

            // Mode 0 causes SyncTreeDownload to download all files.
            int mode = 0;
            // Do not recursively descend the remote directory tree.  Just download all the files in specified directory.
            bool recursive = false;

            success = sftp.SyncTreeDownload(sftpFolderPath, localPath, mode, recursive);
            if (success != true)
            {
                Console.WriteLine(sftp.LastErrorText);
                return;
            }

            Console.WriteLine("Pomyślnie pobrano pliki z katalogu "+sftpFolderPath);


        }
    }
}
