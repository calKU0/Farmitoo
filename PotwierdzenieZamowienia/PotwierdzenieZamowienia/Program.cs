using System;
using System.IO;
using System.Linq;
using Renci.SshNet;
using System.Text.RegularExpressions;

namespace PotwierdzenieZamowienia
{
    class Program
    {
        public static void Main(string[] args)
        {
            string sftpHost = "xxxx";
            int sftpPort = 22;
            string sftpUsername = "xxxx";
            string sftpPassword = "xxxx";
            string sftpFolderPath = "xxxx";
            string localPath = @"\\Backup\k\Foldery pracowników\Krzysztof Kurowski\PotwierdzenieZamowienia\";
            string archivefolder = @"\\Backup\k\Foldery pracowników\Krzysztof Kurowski\Archiwalne\";
            string pattern = @"^confirmation_\d{14}\.csv$"; //Pattern który matchuje plikom zaczynającym się na confirmation_, potem 14 cyfr i rozszerzenie CSV

            using (var client = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        Console.WriteLine("Pomyślnie połączono z serwerem " + sftpHost);

                        // Dokopywanie się do plików, które matchują patternowi
                        var filesToUpload = Directory.GetFiles(localPath)
                                                    .Where(file => Regex.IsMatch(Path.GetFileName(file), pattern))
                                                    .ToArray();

                        Console.WriteLine("Liczba plików {0}.", filesToUpload.Length);
                        Console.WriteLine();
                        Console.WriteLine("Pliki: ");
                        foreach (string file in filesToUpload)
                        {
                            Console.WriteLine(Path.GetFileName(file));
                        }

                        Console.WriteLine();
                        Console.WriteLine("Naciśnij Enter, aby przesłać pliki na serwer sftp");
                        Console.ReadKey();
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

                        Console.WriteLine("Pomyślnie przesłano pliki");
                        Console.ReadKey();
                        client.Disconnect();
                    }
                    else
                    {
                        Console.WriteLine("Nie udało się połączyć z serwerem sftp!");
                        Console.ReadKey();
                    }

                }
                catch (Exception e)
                {
                    client.Disconnect();
                    Console.WriteLine("Wystąpił błąd, zawołaj Krzyśka: " + e);
                    Console.ReadKey();
                }
            }
        }
    }
}
