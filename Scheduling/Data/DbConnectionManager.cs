using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Data
{
    public static class DbConnectionManager
    {
        /// <summary>
        /// get all connections from local json
        /// </summary>
        /// <returns></returns>
        public static List<DbConnection> GetAllConnections()
        {
            List<DbConnection> result;
            using (StreamReader r = new StreamReader("dbconnections.json"))
            {
                string json = r.ReadToEnd();
                result = DbConnection.FromJson(json);
            }
            return result;
        }

        /// <summary>
        /// ge connection string by sitename and connection (main/ events/ timestamp)
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string siteName, string dbName)
        {
            if (dbName == "Main")
            {
                return GetAllConnections().FirstOrDefault(c => c.SiteName == siteName)?.DefaultConnection;
            }
            else if (dbName == "Events")
            {
                return GetAllConnections().FirstOrDefault(c => c.SiteName == siteName)?.DefaultConnectionEvents;
            }
            else if (dbName == "TimeStamp")
            {
                return GetAllConnections().FirstOrDefault(c => c.SiteName == siteName)?.DefaultConnectionTimeStamp;
            }
            return GetAllConnections().FirstOrDefault(c => c.SiteName == siteName)?.DefaultConnection;
        }
    }
}
