using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using System.Data;
using System.Data.SqlClient;

namespace KeyPOC
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //IEnumerable<string> profiles = Directory.GetDirectories(defaultFolder).Where(f => f.Contains("default"));
            //string bookmarksPath = Path.Combine(profiles.First(), "places.sqlite");
            //Console.WriteLine(profiles.Count());
            //if(!profiles.Any())
            //{
            //    Console.WriteLine("No such folder");
            //}
            //Console.WriteLine(bookmarksPath);


            //Console.WriteLine(lastModified.First().ToString());
            //Console.WriteLine(lastModified.Skip(1).First().ToString());

            //string path = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            //Console.WriteLine(path);
            //Console.WriteLine(Directory.GetFiles(path).Count());
            //Console.WriteLine(Directory.GetFiles(path).First());
            //Console.WriteLine(path + "\\desktop.ini");
            //if(Directory.GetFiles(path).Count().Equals(1) && Directory.GetFiles(path).First().Equals(path+"\\desktop.ini"))
            //{
            //    Console.WriteLine("NO FAVOURITES");
            //}
            //foreach (string file in files)
            //{
            //    FileInfo f = new FileInfo(file);
            //    Console.WriteLine(f.Attributes);
            //    if (f.Attributes != FileAttributes.Archive)
            //    {
            //        Console.WriteLine("The file is an ini file");
            //    }
            //}

            //string favoritesImport = "C:\\Users\\Varsha.Ravindra\\Desktop\\FAV";
            //IEnumerable<string> files = Directory.GetFiles(favoritesImport);
            //if (!files.Any() || files.Count().Equals(1) && new FileInfo(files.First()).Attributes != FileAttributes.Archive)
            //{
            //    Console.WriteLine("Old PC does not contain any Internet Explorer bookmarks");
            //}
            //if(!files.Any())
            //{
            //    Console.WriteLine("FIRST");
            //}
            //if(files.Count().Equals(1) && new FileInfo(files.First()).Attributes != FileAttributes.Archive)
            //{
            //    Console.WriteLine("SECOND");
            //}
            //Console.WriteLine(new FileInfo(files.First()).Attributes);

            // Export Mozilla Firefox bookmarks
            #region
            string fullPath = "C:\\Users\\Varsha.Ravindra\\Documents\\DB Test\\FROM\\places.sqlite";
            SQLiteConnection databaseConnection = new SQLiteConnection("Data Source=" + fullPath);
            databaseConnection.Open();
            string names = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";
            SQLiteCommand command = new SQLiteCommand(names, databaseConnection);
            SQLiteDataReader sqReader = command.ExecuteReader();
            List<string> tables = new List<string>();
            try
            {
                while (sqReader.Read())
                {
                    tables.Add((string)sqReader["name"]);
                }
                foreach (string table in tables)
                {
                    Console.WriteLine(table);
                    if (table.Equals("moz_bookmarks") || table.Equals("sqlite_sequence") || table.Equals("sqlite_stat1"))
                    {
                        Console.WriteLine(table);
                        continue;
                    }
                    command = new SQLiteCommand("DROP TABLE " + table, databaseConnection);
                    command.ExecuteNonQuery();
                }
                Console.WriteLine("DROPPED ALL TABLES");
            }
            finally
            {
                sqReader.Close();
                databaseConnection.Close();

            }
            Console.ReadKey();
            #endregion

            //// Import Mozilla Firefox bookmarks
            #region
            //string fromFolderPath = "C:\\Users\\Varsha.Ravindra\\Documents\\DB Test\\FROM\\places.sqlite";
            //SQLiteConnection fromDatabaseConnection = new SQLiteConnection("Data Source=" + fromFolderPath);
            //fromDatabaseConnection.Open();
            //string toFolderPath = "C:\\Users\\Varsha.Ravindra\\Documents\\DB Test\\TO\\places.sqlite";
            //SQLiteConnection toDatabaseConnection = new SQLiteConnection("Data Source=" + toFolderPath);
            //toDatabaseConnection.Open();

            //// Check if temporary files present.
            //string defaultFolder = "C:\\Users\\Varsha.Ravindra\\Documents\\DB Test\\TO";
            //FileInfo[] tempFiles = new DirectoryInfo(defaultFolder).GetFiles();
            //foreach(FileInfo file in tempFiles)
            //{
            //    if(file.ToString().Equals("places.sqlite-shm") || file.Equals("places.sqlite-wal"))
            //    {
            //        Console.WriteLine("File exists");
            //        file.Delete();
            //    }
            //}

            //string drop = "DROP TABLE IF EXISTS moz_bookmarks";
            //SQLiteCommand dropCommand = new SQLiteCommand(drop, toDatabaseConnection);
            //dropCommand.ExecuteNonQuery();
            //string attach = "ATTACH '" + fromFolderPath + "' AS fromDB";
            //SQLiteCommand attachCommand = new SQLiteCommand(attach, toDatabaseConnection);
            //attachCommand.ExecuteNonQuery();
            //string createTable = "CREATE TABLE moz_bookmarks AS SELECT * FROM fromDB.moz_bookmarks";
            //SQLiteCommand createTableCommand = new SQLiteCommand(createTable, toDatabaseConnection);
            //createTableCommand.ExecuteNonQuery();
            //fromDatabaseConnection.Close();
            //toDatabaseConnection.Close();

            //Console.WriteLine("Done");
            //Console.ReadKey();
            #endregion

            //Export DB to CSV
            #region
            try
            {
                string fileName = "C:\\Users\\Varsha.Ravindra\\Documents\\DB Test\\test.csv";
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Close();
                //define header of new file, and write header to file.
                string csvHeader = "id,type,fk,parent,position,title,keyword_id,folder_type,dateAdded,lastModified,guid,syncStatus,syncChangeCounter";
                using (FileStream fsWHT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                using (StreamWriter swT = new StreamWriter(fsWHT))
                {
                    swT.WriteLine(csvHeader.ToString());
                }
                string fullPath = "C:\\Users\\Varsha.Ravindra\\Documents\\DB Test\\FROM\\places.sqlite";
                SQLiteConnection databaseConnection = new SQLiteConnection("Data Source=" + fullPath);
                databaseConnection.Open();
                string attach = "ATTACH '" + fullPath + "' AS fromDB";
                string names = "SELECT * FROM fromDB.moz_bookmarks";
                SQLiteCommand command = new SQLiteCommand(attach, databaseConnection);
                command.ExecuteNonQuery();
                command = new SQLiteCommand(names, databaseConnection);
                SQLiteDataReader sqReader = command.ExecuteReader();
                while (sqReader.Read())
                {
                    //grab relevant tag data and set the csv line for the current row.
                    string csvDetails = sqReader["id"] + "," + sqReader["type"] + "," + sqReader["fk"] + "," + sqReader["parent"] + "," + sqReader["position"] + "," + sqReader["title"] + "," + sqReader["keyword_id"] + "," + sqReader["folder_type"] + "," + sqReader["dateAdded"] + "," + sqReader["lastModified"] + "," + sqReader["guid"] + "," + sqReader["syncStatus"] + "," + sqReader["syncChangeCounter"];

                    using (FileStream fsWDT = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                    using (StreamWriter swDT = new StreamWriter(fsWDT))
                    {
                        //write csv line to file.
                        swDT.WriteLine(csvDetails.ToString());
                    }
                }
                databaseConnection.Close();                
            }
            finally
            {
            }
            //Console.ReadKey();
            #endregion

            //Import csv to DB
            #region
            try
            {
                DataTable tblcsv = new DataTable();
                tblcsv.Columns.Add("id");
                tblcsv.Columns.Add("type");
                tblcsv.Columns.Add("fk");
                tblcsv.Columns.Add("parent");
                tblcsv.Columns.Add("position");
                tblcsv.Columns.Add("title");
                tblcsv.Columns.Add("keyword_id");
                tblcsv.Columns.Add("folder_type");
                tblcsv.Columns.Add("dateAdded");
                tblcsv.Columns.Add("lastModified");
                tblcsv.Columns.Add("guid");
                tblcsv.Columns.Add("syncStatus");
                tblcsv.Columns.Add("syncChangeCounter");
                string CSVFilePath = "C:\\Users\\Varsha.Ravindra\\Documents\\DB Test\\test.csv";
                string ReadCSV = File.ReadAllText(CSVFilePath);
                foreach (string csvRow in ReadCSV.Split('\n').Skip(1))
                {
                    //Adding each row into datatable  
                    tblcsv.Rows.Add();
                    int count = 0;
                    foreach (string FileRec in csvRow.Split(','))
                    {
                        tblcsv.Rows[tblcsv.Rows.Count - 1][count] = FileRec;
                        //Console.WriteLine(tblcsv.Rows[tblcsv.Rows.Count - 1][count]);
                        count++;
                    }
                }

                string fullPath = "C:\\Users\\Varsha.Ravindra\\Documents\\DB Test\\TO\\places.sqlite";
                SQLiteConnection databaseConnection = new SQLiteConnection("Data Source=" + fullPath);
                databaseConnection.Open();
                string tablesEqual = "SELECT type, fk, parent, position FROM moz_bookmarks";
                SQLiteCommand command = new SQLiteCommand(tablesEqual, databaseConnection);
                SQLiteDataReader sqReader = command.ExecuteReader();
                List<string> tables = new List<string>();
                while (sqReader.Read())
                {
                    tables.Add(Convert.ToString(sqReader["type"]) + Convert.ToString(sqReader["fk"]) + Convert.ToString(sqReader["parent"]) + Convert.ToString(sqReader["position"]));
                }
                foreach(string val in tables)
                {
                    Console.WriteLine(val);
                }
                using (SQLiteTransaction transaction = databaseConnection.BeginTransaction())
                {
                    foreach (DataRow row in tblcsv.Rows)
                    {
                        string type = string.IsNullOrEmpty(Convert.ToString(row[1])) ? "NULL" : Convert.ToString(row[1]);
                        string fk = string.IsNullOrEmpty(Convert.ToString(row[2])) ? "NULL" : Convert.ToString(row[2]);
                        string parent = string.IsNullOrEmpty(Convert.ToString(row[3])) ? "NULL" : Convert.ToString(row[3]);
                        string position = string.IsNullOrEmpty(Convert.ToString(row[4])) ? "NULL" : Convert.ToString(row[4]);
                        string title = string.IsNullOrEmpty(Convert.ToString(row[5])) ? "NULL" : Convert.ToString(row[5]);
                        string keyword_id = string.IsNullOrEmpty(Convert.ToString(row[6])) ? "NULL" : Convert.ToString(row[6]);
                        string folder_type = string.IsNullOrEmpty(Convert.ToString(row[7])) ? "NULL" : Convert.ToString(row[7]);
                        string dateAdded = string.IsNullOrEmpty(Convert.ToString(row[8])) ? "NULL" : Convert.ToString(row[8]);
                        string lastModified = string.IsNullOrEmpty(Convert.ToString(row[9])) ? "NULL" : Convert.ToString(row[9]);
                        string guid = string.IsNullOrEmpty(Convert.ToString(row[10])) ? "NULL" : Convert.ToString(row[10]);
                        string syncStatus = string.IsNullOrEmpty(Convert.ToString(row[11])) ? "NULL" : Convert.ToString(row[11]);
                        string syncChangeCounter = string.IsNullOrEmpty(Convert.ToString(row[12])) ? "NULL" : Convert.ToString(row[12]);
                        if(guid.Equals("NULL") && syncStatus.Equals("NULL") && syncChangeCounter.Equals("NULL"))
                        {
                            continue;
                        }
                        Console.WriteLine(type + fk + parent + position);
                        if (tables.Contains(type+fk+parent+position))
                        {
                            continue;
                        }
                        string sqlCommand = "insert into moz_bookmarks(type,fk,parent,position,title,keyword_id,folder_type,dateAdded,lastModified,guid,syncStatus,syncChangeCounter)" + " select " + type + "," + fk + "," + parent + "," + position + ",'" + title + "'," + keyword_id + ",'" + folder_type + "'," + dateAdded + "," + lastModified + ",'" + guid + "'," + syncStatus + "," + syncChangeCounter + " where not exists (select 1 from moz_bookmarks where guid = '" + guid + "');";
                        using (SQLiteCommand sqlitecommand = new SQLiteCommand(sqlCommand, databaseConnection))
                        {
                            sqlitecommand.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                databaseConnection.Close();
                Console.WriteLine("Finished writing to DB");
                Console.ReadKey();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }
            #endregion
        }
    }
}
