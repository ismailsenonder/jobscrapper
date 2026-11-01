using Dapper;
using Microsoft.Data.Sqlite;
using JobScrapper.Models;

namespace JobScrapper.Services;

public class DatabaseService
{
    private readonly string _connectionString = "Data Source=jobs.db";

    public DatabaseService()
    {
        InitDatabase();
    }

    private void InitDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Jobs (
                Id TEXT PRIMARY KEY,
                Title TEXT,
                Company TEXT,
                Location TEXT,
                Url TEXT,
                FetchedAt TEXT
            );");
    }

    public bool IsJobExists(string id)
    {
        using var connection = new SqliteConnection(_connectionString);
        var count = connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Jobs WHERE Id = @Id", new { Id = id });
        return count > 0;
    }

    public void SaveJob(JobPost job)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(@"
            INSERT INTO Jobs (Id, Title, Company, Location, Url, FetchedAt)
            VALUES (@Id, @Title, @Company, @Location, @Url, @FetchedAt)", job);
    }
}