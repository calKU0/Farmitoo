using System;
using System.Linq;
using System.Collections.Generic;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;
using System.Configuration;
using System.IO.Compression;

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
            string sftpFolderPath = "xxxx";
            string localPath = @"C:\temp\";
            string sftpArchFolderPath = "xxxx";
            int j = 0;


            //Łączenie z serwerem
            using (var client = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
            {
                try
                {   
                    client.Connect();
                    Console.WriteLine("Pomyślnie połączono z serwerem " + sftpHost);

                    //Pobieranie plików
                    var files = client.ListDirectory(sftpFolderPath);
                    foreach (var file in files)
                    {
                        if (!file.Name.StartsWith("."))
                        {
                            using (Stream fileStream = File.Create(localPath+file.Name))
                            {
                                client.DownloadFile(file.FullName, fileStream);
                                j++;
                            }
                        }
                    }



                    Console.WriteLine("Pomyślnie pobrano " + j + " plików z katalogu " + sftpFolderPath);
                    int i = 0;
                    //Przenoszenie plików do folderu archiwalnego na sftp
                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            if (file.IsDirectory) continue;
                            client.RenameFile(file.FullName, $"{sftpArchFolderPath}/{file.Name}");
                            i++;
                        }
                        client.Disconnect();

                        Console.WriteLine("Pomyślnie przeniesiono " + i + " plików do folderu archiwalnego");
                    }
                    else
                    {
                        Console.WriteLine("Brak plików do pobrania");
                        client.Disconnect();
                    }
                }

                

                catch (Exception e)
                {
                    Console.WriteLine("Wystąpił błąd: " + e);
                }
            }
        }

    } }

    

