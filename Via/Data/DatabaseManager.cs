using System;
using SQLite;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;
using Via.Models;
using Via.Helpers;
using Newtonsoft.Json;

namespace Via.Data
{
    public class DatabaseManager
    {
        SQLiteConnection dbConnection;
        public DatabaseManager()
        {
            dbConnection = DependencyService.Get<IDBInterface>().CreateConnection();

            dbConnection.CreateTable<SqlLiteReport>();
        }

        public List<SqlLiteReport> GetReports()
        {
            return dbConnection.Table<SqlLiteReport>().ToList();
        }

        public SqlLiteReport GetReport(int reportID)
        {
            return dbConnection.Table<SqlLiteReport>().Where(i => i.reportID == reportID).FirstOrDefault();
        }

        public int SaveReport(SqlLiteReport report)
        {
            return dbConnection.Insert(report);
        }

        public int UpdateReport(ReportData report)
        {
            try
            {
               SqlLiteReport sqlLiteReport = this.GetReports().Find(x => (JsonConvert.DeserializeObject<ReportData>(x.reportData).Data.Id == report.Data.Id));
                if(sqlLiteReport != null)
                    return dbConnection.Execute($"UPDATE SqlLiteReport SET reportData = ? WHERE reportID = ?", JsonConvert.SerializeObject(report), sqlLiteReport.reportID);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public int DeleteReport(SqlLiteReport report)
        {
            return dbConnection.Delete(report);
        }
    }
}
