using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Renci.SshNet;



namespace StanyMagazynoweFarmitoo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Łączenie się z bazą
            string connectionString = "user id=Gaska;password=xxxx;Data Source=xxxx;Trusted_Connection=no;database=xxxx;connection timeout=5;";

            // Zapytanie
            string query = @"Select distinct TOP 100
                            Twr_kod as [reference],
                            case when(isnull((select sum(TwZ_IlSpr) from cdn.TwrZasoby with(nolock) join cdn.TwrKarty us with(nolock) on Twr_GIDNumer = TwZ_TwrNumer where TwZ_MagNumer = 1 and us.Twr_Kod = ss.twr_kod), 0)
                            - isnull((select sum(Rez_Ilosc) from cdn.Rezerwacje with(nolock) join cdn.TwrKarty us with(nolock) on Twr_GIDNumer = Rez_TwrNumer where Rez_MagNumer = 1 and us.twr_kod = ss.twr_kod and Rez_Aktywna = 1 and Rez_Typ = 1 and Rez_DataWaznosci > DATEDIFF(DD, '18001228', GETDATE())),0)) < 0 then 0
                            else cast(isnull((select sum(TwZ_IlSpr) from cdn.TwrZasoby with(nolock) join cdn.TwrKarty us with(nolock) on Twr_GIDNumer = TwZ_TwrNumer where TwZ_MagNumer = 1 and us.Twr_Kod = ss.twr_kod), 0)
                            - isnull((select sum(Rez_Ilosc) from cdn.Rezerwacje with(nolock) join cdn.TwrKarty us with(nolock) on Twr_GIDNumer = Rez_TwrNumer where Rez_MagNumer = 1 and us.twr_kod = ss.twr_kod and Rez_Aktywna = 1 and Rez_Typ = 1 and Rez_DataWaznosci > DATEDIFF(DD, '18001228', GETDATE())),0) as decimal (10, 0)) end as [quantity],
                            'sku' as [reference_type]

                            from cdn.TwrKarty ss with(nolock)
                            join cdn.Atrybuty with(nolock) on Twr_GIDNumer = Atr_ObiNumer and Atr_OBITyp = 16 and Atr_OBILp = 0
                            join cdn.AtrybutyKlasy with(nolock) on AtK_ID = Atr_AtkId
                            left join cdn.twrgrupy KK with(nolock) on Twr_GIDTyp = TwG_GIDTyp AND Twr_GIDNumer = TwG_GIDNumer and TwG_GIDTyp = 16
                            where Twr_PrdNumer in (19468, 19467)
                            and Atr_Wartosc = 'Standardowy'
                            and TwG_GrONumer BETWEEN 36501 AND 53404
                            and CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) > 0
                            and CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) + 1) > 0
                            and(select top 1 TwG_Nazwa from cdn.twrgrupy ss where ss.twg_gidnumer = kk.twg_gronumer and TwG_GIDTyp = -16) <> 'PRASY KOSTKUJĄCE'
                            group by twr_kod,twr_ean";

            // Utworzenie DataTable, aby przechowywało wynik zapytania
            DataTable dataTable = new DataTable();

            try
            {


                // Wypełnienie DataTable 
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine();
                Console.WriteLine("Bląd z łączeniem z bazą danych. Powiadom Krzyśka " + e);
                Console.ReadKey();
            }

            try
            {

                // Pobranie dzisiejszej daty i obecnej godziny a później przekonwertowanie na stringa w formacie RokMiesiącDzieńGodinaMinuta, w celu nazwania pliku
                DateTime dzis = DateTime.Now;
                string data = dzis.ToString("yyyyMMddHHmmss");

                // Ścieżka gdzie zapisuje plik .csv
                string filePath = @"C:\temp\stock_" + data + ".csv";


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
                    Console.WriteLine("Plik stock_" + data + ".csv został wygenerowany pomyślnie");
                }

                // Dane do logowanaia SFTP
                string sftpHost = "sftp.farmitoo.tech";
                int sftpPort = 22;
                string sftpUsername = "gaska";
                string sftpPassword = "2NmqPbx54b2F3U";
                string sftpFolderPath = "/home/gaska/stock";
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


                Console.WriteLine("Plik stock_" + data + ".csv został przesłany na serwer SFTP: " + sftpHost);
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine();
                Console.WriteLine("Błąd z wysyłką danych na sftp. Powiadom Krzyśka! ");
                Console.ReadKey();
            }
            }
    }
}
