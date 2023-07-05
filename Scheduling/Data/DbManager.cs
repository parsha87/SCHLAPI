using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Data
{
    public static class DbManager
    {
        public static string SiteName;

        /// <summary>
        /// get db connection string by sitename and db name
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="dbName"></param>
        /// <returns>connectionString</returns>
        public static string GetDbConnectionString(string siteName, string dbName)
        {
            return DbConnectionManager.GetConnectionString(siteName, dbName);
        }
    }
}
