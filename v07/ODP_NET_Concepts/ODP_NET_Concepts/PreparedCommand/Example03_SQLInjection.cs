using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODP_NET_Concepts.Util;
using Oracle.ManagedDataAccess.Client;

namespace ODP_NET_Concepts.PreparedCommand
{
	public static class Example03_SQLInjection
	{
		public static void Example()
		{
			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						CreateAndPopulateUsersTable(command);

						// prepare query command
						command.CommandText = "select username, password from odp_net_users where username=:username and password=:password";
						ParameterUtil.AddParameter(command, "username", DbType.String, 25);
						ParameterUtil.AddParameter(command, "password", DbType.String, 25);
						command.Prepare();

						// handle input
						// (try entering: "' or 1='1")
						Console.WriteLine("Username: ");
						string username = Console.ReadLine();

						Console.WriteLine("Password: ");
						string password = Console.ReadLine();

						ParameterUtil.SetParameterValue(command, "username", username);
						ParameterUtil.SetParameterValue(command, "password", password);

						using (IDataReader reader = command.ExecuteReader())
						{
							if (((DbDataReader)reader).HasRows)
								while (reader.Read())
								{
									Console.WriteLine("Successfully logged in! Username: {0}, password: {1}\n",
										reader.GetString(0), reader.GetString(1));
								}
							else
								Console.WriteLine("Invalid username or password!");
						}

						ExecuteNonQuery(command, "drop table odp_net_users");
					}
				}
				catch (DbException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		private static int ExecuteNonQuery(IDbCommand command, string sql)
		{
			command.CommandText = sql;
			int rowsAffected = command.ExecuteNonQuery();

			return rowsAffected;
		}

		private static void CreateAndPopulateUsersTable(IDbCommand command)
		{
			ExecuteNonQuery(command, "create table odp_net_users (username varchar(10) primary key, password varchar(10) not null)");
			ExecuteNonQuery(command, "insert into odp_net_users values ('admin', 'admin')");
			ExecuteNonQuery(command, "insert into odp_net_users values ('user1', '1234')");
			ExecuteNonQuery(command, "insert into odp_net_users values ('user2', 'qwerty')");
			ExecuteNonQuery(command, "insert into odp_net_users values ('user3', 'password')");
		}
	}
}
