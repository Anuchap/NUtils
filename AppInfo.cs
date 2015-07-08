﻿using Oracle.DataAccess.Client;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace NUtils
{
    public class AppInfo
    {
        public static DateTime GetBulidTime()
        {
            var filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int cPeHeaderOffset = 60;
            const int cLinkerTimestampOffset = 8;
            var b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            var i = BitConverter.ToInt32(b, cPeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(b, i + cLinkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.ToLocalTime();
            return dt;
        }

        public static SqlConnectionStringBuilder GetConnectionStringBuilder(string connectionName)
        {
            var builder = new SqlConnectionStringBuilder
            {
                ConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString
            };

            return builder;
        }

        public static string GetDbVersion(string connectionName)
        {
            try
            {
                using (var conn = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"SELECT HOST_NAME ||  '-' || VERSION AS DBNAME FROM V$INSTANCE";

                    return (string)cmd.ExecuteScalar();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}