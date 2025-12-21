using System;
using System.IO;
using LiteDB;
using SmallSchedulingApp.Models;

namespace SmallSchedulingApp.Services
{
    public class DatabaseService : IDisposable
    {
        private static DatabaseService? _instance;
        private readonly LiteDatabase _database;
        private const string DatabaseFileName = "events.db";

        public static DatabaseService Instance => _instance ??= new DatabaseService();

        private DatabaseService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SmallSchedulingApp"
            );

            Directory.CreateDirectory(appDataPath);

            string dbPath = Path.Combine(appDataPath, DatabaseFileName);
            _database = new LiteDatabase(dbPath);
        }

        public ILiteCollection<CalendarEvent> Events =>
            _database.GetCollection<CalendarEvent>("events");

        public void Dispose()
        {
            _database?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
