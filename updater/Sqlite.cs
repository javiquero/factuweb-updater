using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using System.Data.OleDb;
using System.IO;
using System.Linq;
using Dapper;

namespace updaterFactuWeb {
    internal class Sqlite {
        private readonly string fileName;
        private readonly string connectionString = "";

        public Sqlite(bool reset = false) {
            fileName = "db.sqlite";
           
            if (reset)
                if (File.Exists(fileName)) 
                    File.Delete(fileName);
                
            if (!File.Exists(fileName)) {
                SQLiteConnection.CreateFile(fileName);
            } 

            connectionString = "Data Source=" + fileName + ";Version=3;";
        }

        public SQLiteConnection GetConnection() {
            var conn = new SQLiteConnection(connectionString);
            return conn.OpenAndReturn();
        }

        public bool dropDataTable(DataTable table) {
            var tableName = FixedTableName(table);

            using (var conn = this.GetConnection())
            using (var tx = conn.BeginTransaction()) {
                var dropSql = $"DROP TABLE IF EXISTS {tableName};";
                conn.Execute(dropSql, tx);
                tx.Commit();
                conn.Close();
            }
            return true;
        }

        public DataTable Select(string query) {
            DataTable dt = new DataTable();
            using (var conn = this.GetConnection()) {               
                using (var command = new SQLiteCommand(query, conn)) {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                    da.Fill(dt);
                    conn.Close();
                }
            }
            return dt;
        }

          
        public string ImportTable(DataTable dataTable, List<string> indexes, string y = null) {
            if (dataTable is null){
                Logger.log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - dataTable no puede ser null.");
                return "";
            }
            if (indexes is null) {
                Logger.log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - indexes no puede ser null.");
                return "";
            }

            var tableName = FixedTableName(dataTable);

            var ind = string.Join(", ", indexes);
            if (y != null) { ind = "YEAR," + ind; }

            var columns = new List<string>();
            if (y != null) { columns.Add("YEAR TEXT"); }

            foreach (var dataColumn in dataTable.Columns.Cast<DataColumn>()) {
                if (indexes.Contains(dataColumn.ColumnName)) {
                    dataColumn.AllowDBNull = false;
                }
                columns.Add(ImportColumn(dataColumn));
            }

            var columnSql = string.Join(", ", columns);

            //if (dataTable.PrimaryKey.Count() > 0)
            //    Console.WriteLine(dataTable.PrimaryKey);

            var sql = $"CREATE TABLE IF NOT EXISTS {tableName} ({columnSql} , PRIMARY KEY({ind}), CONSTRAINT 'ipx_{tableName}' UNIQUE({ind})) ";

            string resp = "";
            using (var conn = GetConnection()) {
                int r = conn.Execute(sql);
                if (r >-1)
                    resp = sql;
                conn.Close();
            }

            return resp;
        }

        public string ImportColumn(DataColumn dataColumn) {
            if (dataColumn is null) {
                Logger.log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - dataColumn no puede ser null.");
                return "";
            }

            var dataType = ResolveColumnDataType(dataColumn);
            var notNull = dataColumn.AllowDBNull ? string.Empty : " NOT NULL";
            var defaultVal = dataColumn.DefaultValue.ToString();
            var defaultValText = string.IsNullOrEmpty(defaultVal) ? string.Empty : $" DEFAULT {defaultVal}";
            var columnName = FixedColumnName(dataColumn);
            var sql = $"{columnName} {dataType}{notNull}{defaultValText}";
            return sql;
        }

        private static string ResolveColumnDataType(DataColumn dataColumn) {
            if (dataColumn is null) {
                Logger.log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - dataColumn no puede ser null.");
                return "";
            }

            var type = dataColumn.DataType;
            if (type == typeof(int) || type == typeof(short) || type == typeof(long) || type == typeof(bool))
                return "INTEGER";
            else if (type == typeof(string) || type == typeof(DateTime))
                return "TEXT";
            else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                return "REAL";
            else if (type == typeof(byte[]))
                return "TEXT";

            Logger.log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - Columns type desconocido.");
            throw new Exception("Unknown Column Type");
        }

        public static string FixedTableName(DataTable table) {
            if (table is null) {
                Logger.log(new Exception( System.Reflection.MethodBase.GetCurrentMethod().Name + " - table no puede ser null."));
                return "";
            }
            return table.TableName.Trim('[', ']').Replace("&", "_and_").Replace(' ', '_');
        }

        public string FixedColumnName(DataColumn column) {
            // Column names should not contain spaces or periods.
            var columnName = column.ColumnName.Trim().Replace(' ', '_').Replace('.', '_');
            // Restrict column names from containing reserved SQLite keywords.
            if (ReservedKeyWords.Contains(columnName.ToUpper()))
                return columnName + "_";
            return columnName;
        }
       
        private string[] ReservedKeyWords = new string[]
        {
            "ABORT",
            "ACTION",
            "ADD",
            "AFTER",
            "ALL",
            "ALTER",
            "ANALYZE",
            "AND",
            "AS",
            "ASC",
            "ATTACH",
            "AUTOINCREMENT",
            "BEFORE",
            "BEGIN",
            "BETWEEN",
            "BY",
            "CASCADE",
            "CASE",
            "CAST",
            "CHECK",
            "COLLATE",
            "COLUMN",
            "COMMIT",
            "CONFLICT",
            "CONSTRAINT",
            "CREATE",
            "CROSS",
            "CURRENT_DATE",
            "CURRENT_TIME",
            "CURRENT_TIMESTAMP",
            "DATABASE",
            "DEFAULT",
            "DEFERRABLE",
            "DEFERRED",
            "DELETE",
            "DESC",
            "DETACH",
            "DISTINCT",
            "DROP",
            "EACH",
            "ELSE",
            "END",
            "ESCAPE",
            "EXCEPT",
            "EXCLUSIVE",
            "EXISTS",
            "EXPLAIN",
            "FAIL",
            "FOR",
            "FOREIGN",
            "FROM",
            "FULL",
            "GLOB",
            "GROUP",
            "HAVING",
            "IF",
            "IGNORE",
            "IMMEDIATE",
            "IN",
            "INDEX",
            "INDEXED",
            "INITIALLY",
            "INNER",
            "INSERT",
            "INSTEAD",
            "INTERSECT",
            "INTO",
            "IS",
            "ISNULL",
            "JOIN",
            "KEY",
            "LEFT",
            "LIKE",
            "LIMIT",
            "MATCH",
            "NATURAL",
            "NO",
            "NOT",
            "NOTNULL",
            "NULL",
            "OF",
            "OFFSET",
            "ON",
            "OR",
            "ORDER",
            "OUTER",
            "PLAN",
            "PRAGMA",
            "PRIMARY",
            "QUERY",
            "RAISE",
            "RECURSIVE",
            "REFERENCES",
            "REGEXP",
            "REINDEX",
            "RELEASE",
            "RENAME",
            "REPLACE",
            "RESTRICT",
            "RIGHT",
            "ROLLBACK",
            "ROW",
            "SAVEPOINT",
            "SELECT",
            "SET",
            "TABLE",
            "TEMP",
            "TEMPORARY",
            "THEN",
            "TO",
            "TRANSACTION",
            "TRIGGER",
            "UNION",
            "UNIQUE",
            "UPDATE",
            "USING",
            "VACUUM",
            "VALUES",
            "VIEW",
            "VIRTUAL",
            "WHEN",
            "WHERE",
            "WITH",
            "WITHOUT",
        };
    }
}
