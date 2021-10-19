using Pedometer.Data;
using Pedometer.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pedometer.Services
{
    public class SqliteService
    {
        public static SQLiteConnection Connection;

        public static void Init()
        {
            Connection = new SQLiteConnection(Constants.DatabasePath);
            Connection.CreateTable<AccelerationData>();
        }

        public static void AddData(string ax, string ay, string az, string timestamp ,string all ,int stepsCount)
        {
            AccelerationData accelerationData = new AccelerationData
            {
                Ax = ax,
                Ay = ay,
                Az = az,
                Aall = all,
                Atime = timestamp,
                StepsCount = stepsCount,
                Checked = false
            };

            Connection.Insert(accelerationData);
        }

        public static void RemoveData(int id)
        {
            Connection.Delete<AccelerationData>(id);
        }

        public static IEnumerable<AccelerationData> GetData()
        {
            var data = Connection.Table<AccelerationData>().ToList();
            return data;
        }

        public static IEnumerable<AccelerationData> GetNullData()
        {
            var data = Connection.Table<AccelerationData>().Where(d => d.Checked == false).ToList();
            return data;
        }
    }
}
