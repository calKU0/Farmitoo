using System;
using System.Linq;
using Renci.SshNet;
using System.IO;


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


            //Łączenie z serwerem
            using (var client = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
            {
                try
                {
                    client.Connect();
                    Console.WriteLine("Pomyślnie połączono z serwerem " + sftpHost);

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

                        if (i == 1)
                        {
                            Console.WriteLine("Pomyślnie pobrano " + i + " plik z katalogu " + sftpFolderPath);
                        }
                        else if (i == 2 || i == 3 || i == 4)
                        {
                            Console.WriteLine("Pomyślnie pobrano " + i + " pliki z katalogu " + sftpFolderPath);
                        }
                        else
                        {
                            Console.WriteLine("Pomyślnie pobrano " + i + " plików z katalogu " + sftpFolderPath);
                        }


                        //Przenoszenie wszystkich plików do folderu archiwalnego na sftp
                        foreach (var file in files)
                        {
                            if (file.IsDirectory) continue;
                            client.RenameFile(file.FullName, $"{sftpArchFolderPath}/{file.Name}");
                        }


                        if (i == 1)
                        {
                            Console.WriteLine("Pomyślnie przeniesiono " + i + " plik do folderu archiwalnego");
                            Console.WriteLine("");
                            Console.WriteLine("Plik: ");
                            foreach (var index in empty)
                            {
                                Console.WriteLine(index);
                            }
                            Console.ReadKey();
                        }
                        else if (i == 2 || i == 3 || i == 4)
                        {
                            Console.WriteLine("Pomyślnie przeniesiono " + i + " pliki do folderu archiwalnego");
                            Console.WriteLine("");
                            Console.WriteLine("Pliki: ");
                            foreach (var index in empty)
                            {
                                Console.WriteLine(index);
                            }
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Pomyślnie przeniesiono " + i + " plików do folderu archiwalnego");
                            Console.WriteLine("");
                            Console.WriteLine("Pliki: ");
                            foreach (var index in empty)
                            {
                                Console.WriteLine(index);
                            }
                            Console.ReadKey();
                        }
                    

                    }
                    else
                    {
                        Console.WriteLine("Nie ma żadnych plików do pobrania!");
                        Console.ReadKey();
                    }
                    client.Disconnect();
                }


                catch (Exception e)
                {
                    client.Disconnect();
                    Console.WriteLine("Wystąpił błąd: " + e);
                    Console.ReadKey();
                }
            }
        }

    } }

    

