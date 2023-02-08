﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace StanyMagazynoweFarmitoo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Łączenie się z bazą
            string connectionString = "user id=Gaska;password=mNgHghY4fGhTRQw;Data Source=192.168.0.105;Trusted_Connection=no;database=CDNXL_GASKA;connection timeout=5;";

            // Zapytanie
            string query = "Select distinct TOP 100 Twr_Ean as [reference], case when(isnull((select sum(TwZ_IlSpr) from cdn.TwrZasoby with(nolock) join cdn.TwrKarty us with(nolock) on Twr_GIDNumer = TwZ_TwrNumer where TwZ_MagNumer = 1 and us.Twr_Kod = ss.twr_kod), 0)- isnull((select sum(Rez_Ilosc) from cdn.Rezerwacje with(nolock) join cdn.TwrKarty us with(nolock) on Twr_GIDNumer = Rez_TwrNumer where Rez_MagNumer = 1 and us.twr_kod = ss.twr_kod and Rez_Aktywna = 1 and Rez_Typ = 1 and Rez_DataWaznosci > DATEDIFF(DD, '18001228', GETDATE())),0)) < 0 then 0 else cast(isnull((select sum(TwZ_IlSpr) from cdn.TwrZasoby with(nolock) join cdn.TwrKarty us with(nolock) on Twr_GIDNumer = TwZ_TwrNumer where TwZ_MagNumer = 1 and us.Twr_Kod = ss.twr_kod), 0)- isnull((select sum(Rez_Ilosc) from cdn.Rezerwacje with(nolock) join cdn.TwrKarty us with(nolock) on Twr_GIDNumer = Rez_TwrNumer where Rez_MagNumer = 1 and us.twr_kod = ss.twr_kod and Rez_Aktywna = 1 and Rez_Typ = 1 and Rez_DataWaznosci > DATEDIFF(DD, '18001228', GETDATE())),0) as decimal (10, 0)) end as [quantity],'ean' as [reference_type] from cdn.TwrKarty ss with(nolock) join cdn.Atrybuty with(nolock) on Twr_GIDNumer = Atr_ObiNumer and Atr_OBITyp = 16 and Atr_OBILp = 0 join cdn.AtrybutyKlasy with(nolock) on AtK_ID = Atr_AtkId where Twr_PrdNumer in (19468, 19467) and Atr_Wartosc = 'Standardowy' group by twr_kod,twr_ean";

            // Utworzenie DataTable, aby przechowywało wynik zapytania
            DataTable dataTable = new DataTable();

            // Wypełnienie DataTable 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dataTable);
            }

            // Pobranie dzisiejszej daty i obecnej godziny a później przekonwertowanie na stringa w formacie RokMiesiącDzieńGodinaMinuta, w celu nazwania pliku
            DateTime dzis = DateTime.Now;
            string data = dzis.ToString("yyyyMMddHHmm");

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

                // Dane
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        writer.Write("\"" + row[i].ToString().Replace("\"", "\"\"") + "\"");

                        if (i < dataTable.Columns.Count - 1)
                        {
                            writer.Write(",");
                        }
                    }

                    writer.WriteLine();
                }
            }

            Console.WriteLine("CSV wygenerowane.");
            Console.ReadKey();
        }
    }
}
