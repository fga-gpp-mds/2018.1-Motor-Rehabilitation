using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace DataBaseAttributes
{
  /**
   * Classe que cria a base de dados em que as relações serão criadas.
   */
	public class DataBase
	{
		private string SqlQuery;
		public string sqlQuery 
		{
			get
			{
				return SqlQuery;
			}
			set
			{
				SqlQuery = value;
			}
		}
		private IDbConnection Conn;
		public IDbConnection conn
		{
			get
			{
				return Conn;
			}
			set
			{
				Conn = value;
			}
		}
		private IDbCommand Cmd;
		public IDbCommand cmd
		{
			get
			{
				return Cmd;
			}
			set
			{
				Cmd = value;
			}
		}

		public void Create (string path, string query)
		{
			using (conn = new SqliteConnection(path))
			{
				conn.Open();
				cmd = conn.CreateCommand();

				sqlQuery = query;

				cmd.CommandText = sqlQuery;
				cmd.ExecuteScalar();
				conn.Close();
			}
		}

		public void Insert (string path, Object[] columns, string tableName, int tableId)
		{
			using (conn = new SqliteConnection(path))
			{
				conn.Open();
				cmd = conn.CreateCommand();

				sqlQuery = string.Format("insert into {0} (", tableName);

				int tableSize = TablesManager.Tables[tableId].colName.Count;

				for (int i = 1; i < tableSize; ++i) 
				{
					string aux;

					if (i + 1 == tableSize)
					{
						aux = ")";
					}
					else
					{
						aux = ",";
					}

					sqlQuery += (TablesManager.Tables[tableId].colName[i] + aux);
				}

				sqlQuery += " values (";

				for (int i = 0; i < columns.Length; ++i) 
				{
					string aux;

					if (i + 1 == columns.Length)
					{
						aux = ")";
					}
					else
					{
						aux = ",";
					}

					sqlQuery += (string.Format("\"{0}\"", columns[i]) + aux);
				}

				cmd.CommandText = sqlQuery;
				cmd.ExecuteScalar();
				conn.Close();
			}
		}

		public void Update (string path, Object[] columns, string tableName, int tableId)
		{
			using (conn = new SqliteConnection(path))
			{
				conn.Open();
				cmd = conn.CreateCommand();

				sqlQuery = string.Format("UPDATE \"{0}\" set ", tableName);

				for (int i = 1; i < TablesManager.Tables[tableId].colName.Count - 1; ++i)
				{
					string aux;

					if (i + 2 == columns.Length)
					{
						aux = " ";
					}
					else
					{
						aux = ",";
					}

					sqlQuery += string.Format("\"{0}\"=\"{1}\"{2}", TablesManager.Tables[tableId].colName[i], columns[i], aux);
				}

				sqlQuery += string.Format("WHERE \"{0}\" = \"{1}\"", TablesManager.Tables[tableId].colName[0], columns[0]);

				cmd.CommandText = sqlQuery;
				
				cmd.ExecuteScalar();
				conn.Close();
			}
		}


		public List<T> Read<T> (string path, string tableName, Object[] columns)
		{
			using (conn = new SqliteConnection(path))
			{
				conn.Open();
				cmd = conn.CreateCommand();

				sqlQuery = "SELECT * " + string.Format("FROM \"{0}\";", tableName);
				cmd.CommandText = sqlQuery;

				IDataReader reader = cmd.ExecuteReader();
				List<T> classList = new List<T>();
				while (reader.Read())
				{

					var z = columns;
					ObjectArray (ref z, ref reader);
					columns = z;

					Type classType = typeof(T);
					ConstructorInfo classConstructor = classType.GetConstructor(new Type[] { columns.GetType() });
					T classInstance = (T)classConstructor.Invoke(new object[] { columns });

					classList.Add(classInstance);
				}

				CloseDB(reader, cmd, conn);
				return classList;	 
			}
		}

		public T ReadValue<T> (string path, string tableName, string colName, int idTable, Object[] columns)
		{
			using (conn = new SqliteConnection(path))
			{
				conn.Open();
				cmd = conn.CreateCommand();
				sqlQuery = "SELECT * " + string.Format("FROM \"{0}\" WHERE \"{1}\" = \"{2}\";", tableName,
																		    					  colName,
																								 idTable);
				cmd.CommandText = sqlQuery;
				IDataReader reader = cmd.ExecuteReader();
				reader.Read();

				var z = columns;
				ObjectArray (ref z, ref reader);
				columns = z;

				Type classType = typeof(T);
				ConstructorInfo classConstructor = classType.GetConstructor(new Type[] { columns.GetType() });
				T classInstance = (T)classConstructor.Invoke(new object[] { columns });

				CloseDB(reader, cmd, conn);

				return classInstance;	 
			}
		}

		/**
		* Função que deleta dados cadastrados anteriormente na relação de pessoas.
		 */
		public void DeleteValue(int tableId, int id)
		{
			using (conn = new SqliteConnection(GlobalController.instance.path))
			{
				conn.Open();
				cmd = conn.CreateCommand();

				sqlQuery = string.Format("delete from \"{0}\" WHERE \"{1}\" = \"{2}\"", TablesManager.Tables[tableId].tableName, TablesManager.Tables[tableId].colName[0], id);

				cmd.CommandText = sqlQuery;
				cmd.ExecuteScalar();
				conn.Close();
			}
		}

		/**
		* Função que apaga a relação de pessoas inteira de uma vez.
		 */
		public void Drop(int tableId)
		{
			using (conn = new SqliteConnection(GlobalController.instance.path))
			{
				conn.Open();
				cmd = conn.CreateCommand();

				sqlQuery = string.Format("DROP TABLE IF EXISTS \"{0}\"", TablesManager.Tables[tableId].tableName);

				cmd.CommandText = sqlQuery;
				cmd.ExecuteScalar();
				conn.Close();
			}
		}

		private void CloseDB (IDataReader reader, IDbCommand cmd, IDbConnection conn)
		{
			reader.Close();
			cmd.Dispose();
			conn.Close();
		}

		private void ObjectArray (ref Object[] columns, ref IDataReader reader)
		{
			for (int i = 0; i < columns.Length; ++i)
			{
				Type t = columns[i].GetType();
				if (!reader.IsDBNull(i))
				{
					if ( t.Equals(typeof(int)) ) 
					{
						columns[i] = reader.GetInt32(i);
					}
					else if ( t.Equals(typeof(string)) ) 
					{
						columns[i] = reader.GetString(i);
					}
					else if ( t.Equals(typeof(double)) ) 
					{
						columns[i] = reader.GetDouble(i);
					}
				}
			}
		}
	}
}
