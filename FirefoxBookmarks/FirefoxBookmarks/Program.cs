using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBookmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Exporting bookmarks

            string bookmarksPath = @"D:\FirefoxBookmarks\placesSSA.sqlite"; //Path to places.sqlite
            string bookmarks = @"D:\FirefoxBookmarks";
            string exportBookmark = Path.Combine(bookmarks, "Bookmarks.json");
            string exportBookmarkPlaces = Path.Combine(bookmarks, "BookmarksPlaces.json");
            string exportBookmarkUrl = Path.Combine(bookmarks, "BookmarksUrls.txt");
            string exportBookmarkFolders = Path.Combine(bookmarks, "BookmarksFolders.txt");

            SQLiteConnection databaseConnection = new SQLiteConnection("Data Source=" + bookmarksPath);
            databaseConnection.Open();
            List<string> folderTitle = new List<string>(new string[] { "menu", "toolbar", "tags", "unfiled", "mobile", "Bookmarks Menu", "Bookmarks Toolbar", "Other Bookmarks", "Tags", "Most Visited", "Recent Tags" });
            Dictionary<string, List<int>> foldersWithIDs = new Dictionary<string, List<int>>();

            string csvHeader = "id,type,fk,parent,position,title,keyword_id,folder_type,dateAdded,lastModified,guid,syncStatus,syncChangeCounter";
            string sqlCommand = "SELECT id,type,fk,parent,position,title,keyword_id,folder_type,dateAdded,lastModified,guid,syncStatus,syncChangeCounter FROM moz_bookmarks";
            SQLiteCommand command = databaseConnection.CreateCommand();
            command.CommandText = sqlCommand;
            SQLiteDataReader sqReader = command.ExecuteReader();
            string[] headers = csvHeader.Split(',');
            StringBuilder builder = new StringBuilder();
            List<int> fks = new List<int>();
            builder.Append("[");
            while (sqReader.Read())
            {
                string titleName = sqReader.GetValue(sqReader.GetOrdinal("title")).ToString();
                if (string.IsNullOrEmpty(titleName) || folderTitle.Contains(titleName))
                {
                    continue;
                }
                string type = sqReader.GetValue(sqReader.GetOrdinal("type")).ToString();
                if (type.Equals("2", StringComparison.InvariantCulture))
                {
                    List<int> ids = new List<int>
                                {
                                    Convert.ToInt32(sqReader.GetValue(sqReader.GetOrdinal("id")), CultureInfo.InvariantCulture),
                                    0
                                };
                    foldersWithIDs[titleName] = ids;
                }
                builder.Append("{");
                int getId = Convert.ToInt32(sqReader.GetValue(sqReader.GetOrdinal("id")), CultureInfo.InvariantCulture);
                foreach (string header in headers)
                {
                    if (header.Equals("fk", StringComparison.InvariantCulture) && !string.IsNullOrEmpty(sqReader.GetValue(sqReader.GetOrdinal(header)).ToString()))
                    {
                        fks.Add(Convert.ToInt32(sqReader.GetValue(sqReader.GetOrdinal(header)), CultureInfo.InvariantCulture));
                    }
                    builder.Append($"\"{header}\":");
                    if (header.Equals("title", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string value = sqReader.GetValue(sqReader.GetOrdinal(header)).ToString();
                        value = value.Replace("\"", "\'\'");
                        value = value.Replace("\\", "\\\\");
                        builder.Append($"\"{value}\"");
                    }
                    else
                    {
                        builder.Append($"\"{sqReader.GetValue(sqReader.GetOrdinal(header))}\"");
                    }
                    if (!headers.Last().Equals(header, StringComparison.InvariantCulture))
                    {
                        builder.Append(",");
                    }
                }
                builder.Append("},");
            }
            string json = builder.ToString();
            json = json.Substring(0, json.Length - 1);
            json = json += "]";
            using (FileStream fsWDT = new FileStream(exportBookmark, FileMode.Append, FileAccess.Write))
            using (StreamWriter swDT = new StreamWriter(fsWDT))
            {
                swDT.WriteLine(json);
            }
            using (FileStream fsWDT = new FileStream(exportBookmarkFolders, FileMode.Append, FileAccess.Write))
            using (StreamWriter swDT = new StreamWriter(fsWDT))
            {
                foreach (string folderName in foldersWithIDs.Keys)
                {
                    swDT.Write(folderName + ",|%|,");
                    foreach (int id in foldersWithIDs[folderName])
                    {
                        swDT.Write(id + ",|%|,");
                    }
                    swDT.Write("\n");
                }
            }

            // Exporting moz_places
            string columnHeaders = "id,url,title,rev_host,hidden,frecency,foreign_count,url_hash,preview_image_url";
            string bookmarksPlaces = "SELECT id,url,title,rev_host,hidden,frecency,foreign_count,url_hash,preview_image_url FROM moz_places";
            SQLiteCommand commandPlaces = databaseConnection.CreateCommand();
            commandPlaces.CommandText = bookmarksPlaces;
            SQLiteDataReader sqReaderPlaces = commandPlaces.ExecuteReader();

            Dictionary<string, List<int>> urlsOldPCWithId = new Dictionary<string, List<int>>();
            string[] headersPlaces = columnHeaders.Split(',');
            StringBuilder builderPlaces = new StringBuilder();
            builderPlaces.Append("[");
            while (sqReaderPlaces.Read())
            {
                int getId = Convert.ToInt32(sqReaderPlaces.GetValue(sqReaderPlaces.GetOrdinal("id")), CultureInfo.InvariantCulture);
                string url = sqReaderPlaces.GetValue(sqReaderPlaces.GetOrdinal("url")).ToString();
                if (!fks.Contains(getId))
                {
                    continue;
                }
                List<int> ids = new List<int>
                {
                    getId,
                    0
                };
                urlsOldPCWithId[url] = (ids);
                builderPlaces.Append("{");
                foreach (string header in headersPlaces)
                {
                    builderPlaces.Append($"\"{header}\":");
                    if (header.Equals("title", StringComparison.InvariantCultureIgnoreCase) ||
                        header.Equals("url", StringComparison.InvariantCultureIgnoreCase) ||
                        header.Equals("rev_host", StringComparison.InvariantCultureIgnoreCase) ||
                        header.Equals("preview_image_url", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string value = sqReaderPlaces.GetValue(sqReaderPlaces.GetOrdinal(header)).ToString();
                        value = value.Replace("\"", "\'\'");
                        value = value.Replace("\\", "\\\\");
                        builderPlaces.Append($"\"{value}\"");
                    }
                    else
                    {
                        builderPlaces.Append($"\"{sqReaderPlaces.GetValue(sqReaderPlaces.GetOrdinal(header))}\"");
                    }
                    if (!headersPlaces.Last().Equals(header, StringComparison.InvariantCulture))
                    {
                        builderPlaces.Append(",");
                    }
                }
                builderPlaces.Append("},");
            }
            string jsonPlaces = builderPlaces.ToString();
            jsonPlaces = jsonPlaces.Substring(0, jsonPlaces.Length - 1);
            jsonPlaces = jsonPlaces += "]";
            using (FileStream fsWDT = new FileStream(exportBookmarkPlaces, FileMode.Append, FileAccess.Write))
            using (StreamWriter swDT = new StreamWriter(fsWDT))
            {
                swDT.WriteLine(jsonPlaces);
            }

            using (FileStream fsWDT = new FileStream(exportBookmarkUrl, FileMode.Append, FileAccess.Write))
            using (StreamWriter swDT = new StreamWriter(fsWDT))
            {
                foreach (string url in urlsOldPCWithId.Keys)
                {
                    swDT.Write(url);
                    foreach (int id in urlsOldPCWithId[url])
                    {
                        swDT.Write(",|%|,");
                        swDT.Write(id);
                    }
                    swDT.Write("\n");
                }
            }

            command?.Dispose();
            sqReader.Close();
            commandPlaces?.Dispose();
            sqReaderPlaces.Close();
            databaseConnection.Close();

            #endregion

            ImportFirefoxBookmarks();           

            Console.ReadKey();
        }

        static void ImportFirefoxBookmarks()
        {
            #region Importing bookmarks

            string bookmarksPath = @"D:\FirefoxBookmarks\places.sqlite";
            string firefoxBookmarksFolder = @"D:\FirefoxBookmarks";
            string firefoxBookmarksPath = Path.Combine(firefoxBookmarksFolder, "Bookmarks.json");
            string firefoxBookmarksPlaces = Path.Combine(firefoxBookmarksFolder, "BookmarksPlaces.json");
            string exportBookmarkUrl = Path.Combine(firefoxBookmarksFolder, "BookmarksUrls.txt");
            string bookmarksFolders = Path.Combine(firefoxBookmarksFolder, "BookmarksFolders.txt");

            SQLiteConnection importDatabaseConnection = new SQLiteConnection("Data Source=" + bookmarksPath);
            importDatabaseConnection.Open();

            string[] urlsOldPC = File.ReadAllLines(exportBookmarkUrl);
            Dictionary<string, List<int>> urlsOldPCWithId = new Dictionary<string, List<int>>();
            foreach (string folder in urlsOldPC)
            {
                string[] split = folder.Split(new string[] { ",|%|," }, StringSplitOptions.None);
                List<int> oldNewIds = new List<int>
                            {
                                Convert.ToInt32(split[1]),
                                Convert.ToInt32(split[2])
                            };
                urlsOldPCWithId[split[0]] = oldNewIds;
            }

            string[] foldersIds = File.ReadAllLines(bookmarksFolders);
            Dictionary<string, List<int>> folderWithIdPairs = new Dictionary<string, List<int>>();
            foreach (string folder in foldersIds)
            {
                string[] split = folder.Split(new string[] { ",|%|," }, StringSplitOptions.None);
                try
                {
                    List<int> oldNewIds = new List<int>
                                {
                                    Convert.ToInt32(split[1]),
                                    Convert.ToInt32(split[2])
                                };
                    folderWithIdPairs[split[0]] = oldNewIds;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            string getfkNewPC = "SELECT id,type,fk,title,guid FROM moz_bookmarks";
            SQLiteCommand commandGetFk = importDatabaseConnection.CreateCommand();
            commandGetFk.CommandText = getfkNewPC;
            SQLiteDataReader sqReaderGetFk = commandGetFk.ExecuteReader();
            List<int> fks = new List<int>();
            List<string> parentFolders = new List<string>();
            List<string> guids = new List<string>();
            List<int> idsNew = new List<int>();
            while (sqReaderGetFk.Read())
            {
                if (!string.IsNullOrEmpty(sqReaderGetFk["fk"].ToString()))
                {
                    fks.Add(Convert.ToInt32(sqReaderGetFk["fk"]));
                }
                string type = sqReaderGetFk["type"].ToString();
                if (type.Equals("2", StringComparison.InvariantCulture))
                {
                    parentFolders.Add(sqReaderGetFk["title"].ToString());
                }
                guids.Add(sqReaderGetFk["guid"].ToString());
                idsNew.Add(Convert.ToInt32(sqReaderGetFk["id"]));
            }
            sqReaderGetFk.Close();
            int lastID = idsNew.Last();

            string geturlNewPC = "SELECT id,url FROM moz_places";
            SQLiteCommand commandGetUrl = importDatabaseConnection.CreateCommand();
            commandGetUrl.CommandText = geturlNewPC;
            SQLiteDataReader sqReaderGetUrl = commandGetUrl.ExecuteReader();
            List<string> urlsNewPC = new List<string>();
            while (sqReaderGetUrl.Read())
            {
                if (fks.Contains(Convert.ToInt32(sqReaderGetUrl["id"])))
                {
                    urlsNewPC.Add((sqReaderGetUrl["url"].ToString()));
                }
            }
            sqReaderGetUrl.Close();

            List<string> bookmarksToExclude = new List<string>();
            List<string> urlsOldKeys = urlsOldPCWithId.Keys.ToList();
            foreach (string oldUrl in urlsOldKeys)
            {
                if (urlsNewPC.Contains(oldUrl))
                {
                    bookmarksToExclude.Add(oldUrl);
                    urlsOldPCWithId.Remove(oldUrl);
                }
            }

            string readJsonPlaces = File.ReadAllText(firefoxBookmarksPlaces);
            DataTable dataTablePlaces = (DataTable)JsonConvert.DeserializeObject(readJsonPlaces, (typeof(DataTable)));

            using (SQLiteTransaction transaction = importDatabaseConnection.BeginTransaction())
            {
                foreach (DataRow row in dataTablePlaces.Rows)
                {
                    string id = Convert.ToString(row[0], CultureInfo.InvariantCulture);
                    string url = Convert.ToString(row[1], CultureInfo.InvariantCulture);
                    if (bookmarksToExclude.Contains(url))
                    {
                        Console.WriteLine("Repeated bookmarked URL in moz_places: {0}", id);
                        continue;
                    }
                    string title = Convert.ToString(row[2], CultureInfo.InvariantCulture);
                    string rev_host = Convert.ToString(row[3], CultureInfo.InvariantCulture);
                    string hidden = Convert.ToString(row[4], CultureInfo.InvariantCulture);
                    string frecency = Convert.ToString(row[5], CultureInfo.InvariantCulture);
                    string foreign_count = Convert.ToString(row[6], CultureInfo.InvariantCulture);
                    string url_hash = Convert.ToString(row[7], CultureInfo.InvariantCulture);
                    string preview_image_url = Convert.ToString(row[8], CultureInfo.InvariantCulture);
                    string sqlCommand = "insert into moz_places(url,title,rev_host,hidden,frecency,foreign_count,url_hash,preview_image_url)" + " select @url, @title, @rev_host, @hidden, @frecency, @foreign_count, @url_hash, @preview_image_url;";
                    SQLiteCommand sqlPlacesCommand = importDatabaseConnection.CreateCommand();
                    sqlPlacesCommand.CommandText = sqlCommand;
                    sqlPlacesCommand.Parameters.AddWithValue("@url", url);
                    sqlPlacesCommand.Parameters.AddWithValue("@title", title);
                    sqlPlacesCommand.Parameters.AddWithValue("@rev_host", rev_host);
                    sqlPlacesCommand.Parameters.AddWithValue("@hidden", hidden);
                    sqlPlacesCommand.Parameters.AddWithValue("@frecency", frecency);
                    sqlPlacesCommand.Parameters.AddWithValue("@foreign_count", foreign_count);
                    sqlPlacesCommand.Parameters.AddWithValue("@url_hash", url_hash);
                    sqlPlacesCommand.Parameters.AddWithValue("@preview_image_url", preview_image_url);
                    sqlPlacesCommand.ExecuteNonQuery();
                    sqlPlacesCommand.Dispose();
                }
                transaction.Commit();
            }
            importDatabaseConnection.Close();
            commandGetFk.Dispose();
            sqReaderGetFk.Dispose();
            commandGetUrl.Dispose();
            sqReaderGetUrl.Dispose();
            Console.WriteLine("Import moz_places completed.");

            Console.WriteLine("Finding new IDs of bookmarked URLs");
            SQLiteConnection importDatabaseConnectionPlacesUpdate = new SQLiteConnection("Data Source=" + bookmarksPath);
            importDatabaseConnectionPlacesUpdate.Open();
            List<string> urlsIncluded = urlsOldPCWithId.Keys.ToList();
            foreach (string urlName in urlsIncluded)
            {
                string sqlCommandGetId = "select id from moz_places where url = @urlName";
                SQLiteCommand getIdCommand = importDatabaseConnectionPlacesUpdate.CreateCommand();
                getIdCommand.CommandText = sqlCommandGetId;
                getIdCommand.Parameters.AddWithValue("@urlName", urlName);
                SQLiteDataReader sqReaderUpdate = getIdCommand.ExecuteReader();
                int newPcId = 0;
                while (sqReaderUpdate.Read())
                {
                    newPcId = Convert.ToInt32(sqReaderUpdate.GetValue(sqReaderUpdate.GetOrdinal("id")));
                }
                List<int> id = urlsOldPCWithId[urlName];
                id.Insert(1, newPcId);
                id.RemoveAt(2);
                urlsOldPCWithId[urlName] = id;
                sqReaderUpdate.Close();
                getIdCommand.Dispose();
            }
            Console.WriteLine("Creating a dictionary to map old and new moz_places ID");
            Dictionary<int, int> oldNewUrlId = new Dictionary<int, int>();
            foreach (string url in urlsOldPCWithId.Keys)
            {
                List<int> idList = urlsOldPCWithId[url];
                oldNewUrlId[idList[0]] = idList[1];
            }
            importDatabaseConnectionPlacesUpdate.Close();

            string readJson = File.ReadAllText(firefoxBookmarksPath);
            DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(readJson, (typeof(DataTable)));
            importDatabaseConnection.Open();
            Console.WriteLine("Import moz_bookmarks started.");
            using (SQLiteTransaction transaction = importDatabaseConnection.BeginTransaction())
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    try
                    {
                        string id = Convert.ToString(row[0], CultureInfo.InvariantCulture);
                        string type = Convert.ToString(row[1], CultureInfo.InvariantCulture);
                        string fk = Convert.ToString(row[2], CultureInfo.InvariantCulture);
                        string parent = Convert.ToString(row[3], CultureInfo.InvariantCulture);
                        string position = Convert.ToString(row[4], CultureInfo.InvariantCulture);
                        string title = Convert.ToString(row[5], CultureInfo.InvariantCulture);
                        title = title.Replace("\'\'", "\"");
                        string keyword_id = Convert.ToString(row[6], CultureInfo.InvariantCulture);
                        string folder_type = Convert.ToString(row[7], CultureInfo.InvariantCulture);
                        string dateAdded = Convert.ToString(row[8], CultureInfo.InvariantCulture);
                        string lastModified = Convert.ToString(row[9], CultureInfo.InvariantCulture);
                        string guid = Convert.ToString(row[10], CultureInfo.InvariantCulture);
                        string syncStatus = Convert.ToString(row[11], CultureInfo.InvariantCulture);
                        string syncChangeCounter = Convert.ToString(row[12], CultureInfo.InvariantCulture);

                        if (!string.IsNullOrEmpty(fk))
                        {
                            if (!oldNewUrlId.Keys.Contains(Convert.ToInt32(fk)) && !type.Equals("2", StringComparison.InvariantCulture))
                            {
                                Console.WriteLine("Repeated {0} bookmark skipped", id);
                                continue;
                            }
                            fk = oldNewUrlId[Convert.ToInt32(fk)].ToString();
                        }
                        if ((type.Equals("2", StringComparison.InvariantCulture) && parentFolders.Contains(title)))
                        {
                            Console.WriteLine("Repeated folder {0} skipped", id);
                            continue;
                        }
                        if (parent.Equals("4", StringComparison.InvariantCulture))
                        {
                            Console.WriteLine("Tags will not be migrated. {0} skipped", id);
                            continue;
                        }
                        if (guids.Contains(guid))
                        {
                            Console.WriteLine("Unique key constraint failed for {0}. Generating new GUID", title);
                            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-";
                            int length = 12;
                            StringBuilder result = new StringBuilder(length);
                            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                            {
                                for (int i = 0; i < length; i++)
                                {
                                    byte[] buffer = new byte[sizeof(uint)];
                                    rng.GetBytes(buffer);
                                    uint num = BitConverter.ToUInt32(buffer, 0);
                                    result.Append(characters[(int)(num % (uint)characters.Length)]);
                                }
                            }
                            guid = result.ToString();
                        }

                        string sqlCommand = "insert into moz_bookmarks(type,fk,parent,position,title,keyword_id,folder_type,dateAdded,lastModified,guid,syncStatus,syncChangeCounter)" + " select @type,@fk,@parent,@position,@title,@keyword_id,@folder_type,@dateAdded,@lastModified,@guid,@syncStatus,@syncChangeCounter where not exists (select 1 from moz_bookmarks where guid = @guid);";
                        SQLiteCommand sqlInsertCommand = importDatabaseConnection.CreateCommand();
                        sqlInsertCommand.CommandText = sqlCommand;
                        sqlInsertCommand.Parameters.AddWithValue("@type", type);
                        sqlInsertCommand.Parameters.AddWithValue("@fk", fk);
                        sqlInsertCommand.Parameters.AddWithValue("@parent", parent);
                        sqlInsertCommand.Parameters.AddWithValue("@position", position);
                        sqlInsertCommand.Parameters.AddWithValue("@title", title);
                        sqlInsertCommand.Parameters.AddWithValue("@keyword_id", keyword_id);
                        sqlInsertCommand.Parameters.AddWithValue("@folder_type", folder_type);
                        sqlInsertCommand.Parameters.AddWithValue("@dateAdded", dateAdded);
                        sqlInsertCommand.Parameters.AddWithValue("@lastModified", lastModified);
                        sqlInsertCommand.Parameters.AddWithValue("@guid", guid);
                        sqlInsertCommand.Parameters.AddWithValue("@syncStatus", syncStatus);
                        sqlInsertCommand.Parameters.AddWithValue("@syncChangeCounter", syncChangeCounter);
                        sqlInsertCommand.ExecuteNonQuery();
                        sqlInsertCommand.Dispose();
                        if (type.Equals("2", StringComparison.InvariantCulture))
                        {
                            Console.WriteLine("Inserting folder {0} successful", id);
                        }
                        else
                        {
                            Console.WriteLine("Inserting bookmark {0} successful", id);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Couldn't migrate the bookmark due to unique constraint failed", ex.Message);
                    }
                }
                transaction.Commit();
            }

            Console.WriteLine("Import moz_bookmarks completed.");
            dataTable.Dispose();
            importDatabaseConnection.Close();

            Console.WriteLine("Updating folder referencing started.");
            SQLiteConnection importDatabaseConnectionLater = new SQLiteConnection("Data Source=" + bookmarksPath);
            importDatabaseConnectionLater.Open();

            List<string> folderNames = folderWithIdPairs.Keys.ToList();
            foreach (string folderName in folderNames)
            {
                try
                {
                    string sqlCommandUpdate = "select id from moz_bookmarks where title = @folderName";
                    SQLiteCommand getIdCommand = importDatabaseConnectionLater.CreateCommand();
                    getIdCommand.CommandText = sqlCommandUpdate;
                    getIdCommand.Parameters.AddWithValue("@folderName", folderName);
                    SQLiteDataReader sqReaderUpdate = getIdCommand.ExecuteReader();
                    int newPcId = 0;
                    while (sqReaderUpdate.Read())
                    {
                        newPcId = Convert.ToInt32(sqReaderUpdate.GetValue(sqReaderUpdate.GetOrdinal("id")));
                    }
                    List<int> id = folderWithIdPairs[folderName];
                    id.Insert(1, newPcId);
                    id.RemoveAt(2);
                    folderWithIdPairs[folderName] = id;
                    Console.WriteLine("FolderName: {0}, ID: {1}", folderName, id);
                    sqReaderUpdate.Close();
                    getIdCommand.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception with getting id", ex.Message);
                }
            }
            Console.WriteLine("Creating a dictionary of old and new PC folder IDs.");
            Dictionary<int, int> oldNewId = new Dictionary<int, int>();
            foreach (string fold in folderWithIdPairs.Keys)
            {
                List<int> idList = folderWithIdPairs[fold];
                oldNewId[idList[0]] = idList[1];
            }
            Console.WriteLine("Updating moz_bookmarks with new folder ID reference (parent)");
            using (SQLiteTransaction transaction = importDatabaseConnectionLater.BeginTransaction())
            {
                foreach (int parentOldId in oldNewId.Keys)
                {
                    try
                    {
                        string parent = oldNewId[parentOldId].ToString();
                        if (parent.Equals("0", StringComparison.InvariantCulture))
                        {
                            continue;
                        }
                        string sqlUpdateCommand = "update moz_bookmarks set parent = @parent where parent = @oldId and id > @lastID";
                        SQLiteCommand updateCommand = importDatabaseConnectionLater.CreateCommand();
                        updateCommand.CommandText = sqlUpdateCommand;
                        updateCommand.Parameters.AddWithValue("@parent", parent);
                        updateCommand.Parameters.AddWithValue("@oldId", parentOldId);
                        updateCommand.Parameters.AddWithValue("@lastID", lastID);
                        updateCommand.ExecuteNonQuery();
                        updateCommand.Dispose();
                        Console.WriteLine("OldID: {0}, NewID: {1}", parentOldId, parent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception with getting id", ex.Message);
                    }
                }
                transaction.Commit();
            }
            importDatabaseConnectionLater.Close();
            Console.WriteLine("Updating folder referencing completed.");
            Console.WriteLine("Import Firefox Bookmarks completed");
            #endregion
        }
    }
}
