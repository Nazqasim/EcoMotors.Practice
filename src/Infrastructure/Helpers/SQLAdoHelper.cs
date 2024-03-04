using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoMotorsPractice.Infrastructure.Persistence;

namespace EcoMotorsPractice.Infrastructure.SqlAdoHelper;
public class SQLAdoHelper : IDisposable
{
    private readonly DatabaseSettings _settings;

    // Internal members
    protected string _connString = null;
    protected SqlConnection _conn = null;
    protected SqlTransaction _trans = null;
    protected bool _disposed = false;

    /// <summary>
    /// Sets or returns the connection string use by all instances of this class.
    /// </summary>
    public static string ConnectionString { get; set; }

    /// <summary>
    /// Returns the current SqlTransaction object or null if no transaction
    /// is in effect.
    /// </summary>
    public SqlTransaction Transaction => _trans;

    /// <summary>
    /// Constructor using global connection string.
    /// </summary>
    public SQLAdoHelper(IOptions<DatabaseSettings> settings)
    {
        ConnectionString = settings.Value.ConnectionString;
        _connString = ConnectionString;
        Connect();
    }

    // Creates a SqlConnection using the current connection string
    protected void Connect()
    {
        _conn = new SqlConnection(_connString);
        _conn.Open();
    }

    /// <summary>
    /// Constructs a SqlCommand with the given parameters. This method is normally called
    /// from the other methods and not called directly. But here it is if you need access
    /// to it.
    /// </summary>
    /// <param name="qry">SQL query or stored procedure name</param>
    /// <param name="type">Type of SQL command</param>
    /// <param name="args">Query arguments. Arguments should be in pairs where one is the
    /// name of the parameter and the second is the value. The very last argument can
    /// optionally be a SqlParameter object for specifying a custom argument type</param>
    /// <returns></returns>
    public SqlCommand CreateCommand(string qry, CommandType type, params object[] args)
    {
        SqlCommand cmd = new SqlCommand(qry, _conn);

        // Associate with current transaction, if any
        if (_trans != null)
            cmd.Transaction = _trans;

        // Set command type
        cmd.CommandType = type;

        if (args != null)
        {
            // Construct SQL parameters
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is string && i < (args.Length - 1))
                {
                    SqlParameter parm = new SqlParameter();
                    parm.ParameterName = (string)args[i];
                    parm.Value = args[++i];
                    cmd.Parameters.Add(parm);
                }
                else if (args[i] is SqlParameter)
                {
                    cmd.Parameters.Add((SqlParameter)args[i]);
                }
                else throw new ArgumentException("Invalid number or type of arguments supplied");
            }
        }

        return cmd;
    }

    /// <summary>
    /// Executes a query that returns no results
    /// </summary>
    /// <param name="qry">Query text</param>
    /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
    /// <returns>The number of rows affected</returns>
    public int ExecNonQuery(string qry, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
        {
            return cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Executes a stored procedure that returns no results
    /// </summary>
    /// <param name="proc">Name of stored proceduret</param>
    /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
    /// <returns>The number of rows affected</returns>
    public int ExecNonQueryProc(string proc, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(proc, CommandType.StoredProcedure, args))
        {
            return cmd.ExecuteNonQuery();
        }
    }

    /// <summary>Executes a stored procedure that returns no results</summary>
    /// <param name="proc">Name of stored proceduret</param>
    /// <param name="commandTimeOut">The command Time Out.</param>
    /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
    /// <returns>The number of rows affected</returns>
    public async Task<int> ExecNonQueryProcAsync(string proc, int commandTimeOut, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(proc, CommandType.StoredProcedure, args))
        {
            cmd.CommandTimeout = commandTimeOut;
            return await cmd.ExecuteNonQueryAsync();
        }
    }

    /// <summary>The exec non query proc async.</summary>
    /// <param name="proc">The proc.</param>
    /// <param name="args">The args.</param>
    /// <returns>The <see cref="Task"/>.</returns>
    public async Task<int> ExecNonQueryProcAsync(string proc, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(proc, CommandType.StoredProcedure, args))
        {
            return await cmd.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// Executes a query that returns a single value
    /// </summary>
    /// <param name="qry">Query text</param>
    /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
    /// <returns>Value of first column and first row of the results</returns>
    public object ExecScalar(string qry, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
        {
            return cmd.ExecuteScalar();
        }
    }

    /// <summary>
    /// Executes a query that returns a single value
    /// </summary>
    /// <param name="proc">Name of stored proceduret</param>
    /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
    /// <returns>Value of first column and first row of the results</returns>
    public object ExecScalarProc(string qry, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
        {
            return cmd.ExecuteScalar();
        }
    }

    /// <summary>
    /// Executes a query and returns the results as a SqlDataReader
    /// </summary>
    /// <param name="qry">Query text</param>
    /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
    /// <returns>Results as a SqlDataReader</returns>
    public SqlDataReader ExecDataReader(string qry, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
        {
            return cmd.ExecuteReader();
        }
    }

    /// <summary>
    /// Executes a stored procedure and returns the results as a SqlDataReader
    /// </summary>
    /// <param name="proc">Name of stored proceduret</param>
    /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
    /// <returns>Results as a SqlDataReader</returns>
    public SqlDataReader ExecDataReaderProc(string qry, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
        {
            return cmd.ExecuteReader();
        }
    }

    /// <summary>
    /// Executes a query and returns the results as a DataSet
    /// </summary>
    /// <param name="qry">Query text</param>
    /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
    /// <returns>Results as a DataSet</returns>
    public DataSet ExecDataSet(string qry, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
        {
            SqlDataAdapter adapt = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapt.Fill(ds);
            return ds;
        }
    }

    /// <summary>Executes a stored procedure and returns the results as a Data Set</summary>
    /// <param name="qry">The qry.</param>
    /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
    /// <returns>Results as a DataSet</returns>
    public DataSet ExecDataSetProc(string qry, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
        {
            cmd.CommandTimeout = 300;
            SqlDataAdapter adapt = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapt.Fill(ds);
            return ds;
        }
    }

    /// <summary>The exec data set proc.</summary>
    /// <param name="qry">The qry.</param>
    /// <param name="commandTimeOut">The command time out.</param>
    /// <param name="args">The args.</param>
    /// <returns>The <see cref="DataSet"/>.</returns>
    public DataSet ExecDataSetProc(string qry, int commandTimeOut, params object[] args)
    {
        using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
        {
            cmd.CommandTimeout = commandTimeOut;
            SqlDataAdapter adapt = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapt.Fill(ds);
            return ds;
        }
    }

    /// <summary>
    /// Begins a transaction
    /// </summary>
    /// <returns>The new SqlTransaction object</returns>
    public SqlTransaction BeginTransaction()
    {
        Rollback();
        _trans = _conn.BeginTransaction();
        return Transaction;
    }

    /// <summary>
    /// Commits any transaction in effect.
    /// </summary>
    public void Commit()
    {
        if (_trans != null)
        {
            _trans.Commit();
            _trans = null;
        }
    }

    /// <summary>
    /// The rollback.
    /// </summary>
    public void Rollback()
    {
        if (this._trans != null)
        {
            this._trans.Rollback();
            this._trans = null;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    /// <param name="disposing">
    /// The disposing.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            // Need to dispose managed resources if being called manually
            if (disposing)
            {
                if (this._conn != null)
                {
                    this.Rollback();
                    this._conn.Dispose();
                    this._conn = null;
                }
            }

            this._disposed = true;
        }
    }

    public async Task<DataSet> ExecuteProcedureAsync(string procedureName, params SqlParameter[] parameters)
    {
        using (var conn = await this.GetOpenConnectionAsync())
        using (var cmd = new SqlCommand(procedureName, conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parameters);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                DataSet dataSet = new DataSet();

                while (!reader.IsClosed)
                {
                    var table = new DataTable();
                    table.Load(reader);

                    dataSet.Tables.Add(table);
                }

                return dataSet;
            }
        }
    }

    private async Task<SqlConnection> GetOpenConnectionAsync()
    {
        var asyncConnectionString = new SqlConnectionStringBuilder()
        {
            ConnectionString = _connString,
        }.ToString();

        var conn = new SqlConnection(asyncConnectionString);

        await conn.OpenAsync();

        return conn;
    }
}
