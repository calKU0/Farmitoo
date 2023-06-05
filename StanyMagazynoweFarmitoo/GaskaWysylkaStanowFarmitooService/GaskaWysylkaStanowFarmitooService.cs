using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using System.Timers;
using System.Data.SqlClient;
using Renci.SshNet;
using System.Runtime.InteropServices;

namespace GaskaWysylkaStanowFarmitooService
{
    public partial class GaskaWysylkaStanowFarmitooService : ServiceBase
    {
        [DllImport("ClaRUN.dll")]
        static extern void AttachThreadToClarion(int _flag);

        System.Timers.Timer timer = new System.Timers.Timer();
        string sftpHost = ConfigurationManager.AppSettings["SftpHostname"];
        int sftpPort = int.Parse(ConfigurationManager.AppSettings["SftpPort"]);
        string sftpUsername = ConfigurationManager.AppSettings["SftpUsername"];
        string sftpPassword = ConfigurationManager.AppSettings["SftpPassword"];
        string sciezkaLogow = ConfigurationManager.AppSettings["Ścieżka Logów"] + @"\log.txt";
        string godzinaWysylki = ConfigurationManager.AppSettings["Godzina wysylki"];
        string sftpFolderPath = ConfigurationManager.AppSettings["SftpFolderPath"];
        int odpytujCoMinut = int.Parse(ConfigurationManager.AppSettings["Co ile minut odpytywac"]);
        string czyJuzWykonano = "";
        SqlConnection connection;
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public GaskaWysylkaStanowFarmitooService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ZapiszLog("Uruchomienie usługi");
            try
            {
                Thread Wysylka = new Thread(Watek);
                Wysylka.Start();
                
            }
            catch (Exception ex)
            {
                ZapiszLog("Błąd OnStart. " + ex.ToString());
                Stop();
            }
        }

        protected override void OnStop()
        {
            ZapiszLog("Zatrzymanie usługi");
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            timer.Stop();
        }

        private void ZapiszLog(string tekst)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(sciezkaLogow, true))
                {
                    sw.Write(DateTime.Now + " " + tekst + "\r\n\n");
                }
            }
            catch
            {

            }
        }

        /* Funkcja, która sprawdza czy jest godzina 9:00 i czy został już wysłany plik ze stanami. Jeśli jest podana godzina i plik nie został wysłany w dzisiejszym dniu to uruchamia funkcję Wysylka*/
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            string aktualnaGodzina = DateTime.Now.Hour.ToString();
            string data = DateTime.Now.ToShortDateString();

            if (aktualnaGodzina == godzinaWysylki &&  czyJuzWykonano != data) // jesli juz wykonalem operacje o zadanej godzinie w dzisiejszym dniu to juz jej nie wykonam ponownie
            {
                Thread threadWysylka = new Thread(Wysylka);
                threadWysylka.Start();
                czyJuzWykonano = data;
            }
        }

        /* Funkcja, która zwraca datatable ze stored procedury*/
        public DataTable GetDataTable()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["GaskaConnectionString"].ConnectionString))
            {
                connection.Open();
                // Zapytanie 
                string query = @"dbo.GaskaStanyMagazynoweFarmitoo";

                // Utworzenie DataTable, aby przechowywało wynik zapytania
                DataTable dataTable = new DataTable();

                // Wypełnienie DataTable 
                SqlCommand commandProducts = new SqlCommand(query, connection);
                commandProducts.CommandType = CommandType.StoredProcedure;
                using (SqlDataAdapter da = new SqlDataAdapter(commandProducts))
                {
                    da.Fill(dataTable);
                }
                return dataTable;
            }
        }

        /* Funkcja, która tworzy plik ze stanami magazynowymi i przesyłą ten plik na serwert sftp*/
        public void Wysylka()
        {
            AttachThreadToClarion(1);
            try
            {
                // Pobranie dzisiejszej daty i obecnej godziny a później przekonwertowanie na stringa w formacie RokMiesiącDzieńGodinaMinuta, w celu nazwania pliku
                DateTime dzis = DateTime.Now;
                string data = dzis.ToString("yyyyMMddHHmmss");
                string filePath = @"D:\Programy\GaskaStanyMagazynoweFarmitooService\Wygenerowane\stock_" + data + ".csv";

                DataTable dataTable = GetDataTable();

                // Konwersja na CSV
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Nagłówki
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        writer.Write(dataTable.Columns[i]);

                        if (i < dataTable.Columns.Count - 1)
                        {
                            writer.Write(",");
                        }
                    }

                    writer.WriteLine();

                    // Dane z zapytania
                    foreach (DataRow row in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            writer.Write(row[i].ToString());

                            if (i < dataTable.Columns.Count - 1)
                            {
                                writer.Write(",");
                            }
                        }

                        writer.WriteLine();
                    }
                    ZapiszLog("Plik stock_" + data + ".csv został wygenerowany pomyślnie");
                }

                string sftpFileName = "stock_" + data + ".csv";

                // Wysyłka danych na sftp
                using (SftpClient sftpClient = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
                {
                    sftpClient.Connect();

                    // Zmiana folderu
                    sftpClient.ChangeDirectory(sftpFolderPath);

                    // Wysyłka
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        sftpClient.UploadFile(fileStream, sftpFileName);
                    }

                    sftpClient.Disconnect();
                }


                ZapiszLog("Plik stock_" + data + ".csv został przesłany na serwer SFTP: " + sftpHost);
            }
            catch (Exception e)
            {
                ZapiszLog("Błąd z wysyłką danych na sftp." + e);
            }
        }

        /* Wątek, który co 5 minut uruchamia funkcję OnTimer*/
        private void Watek()
        {
            AttachThreadToClarion(1);
            timer.Interval = odpytujCoMinut * 60000;
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

            autoResetEvent.WaitOne(); // czekam na sygnał zatrzymania wątku
        }
    }
}
