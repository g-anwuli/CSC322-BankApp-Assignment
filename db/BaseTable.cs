using System.Text.Json;

namespace BankApp.Db
{
    /// <summary>
    /// A generic base implementation of <see cref="IDbTable{T}"/> that uses JSON file storage.
    /// Provides basic CRUD and query functionality.
    /// </summary>
    /// <typeparam name="T">The type of record stored in the table.</typeparam>
    public class DbTable<T> : IDbTable<T>
    {
        private List<T> Records = new();
        public string TableName { get; set; } = string.Empty;
        public string IDENTIFIER { get; set; } = "Id";
        private string DataPath { get; set; }

        public DbTable(string tableName, string indentifier, string dataPathFolder)
        {

            TableName = tableName;
            DataPath = Path.Combine(dataPathFolder, tableName + ".json");
            IDENTIFIER = indentifier;
            Records = Load();
        }

        public List<T> Load()
        {
            if (!File.Exists(DataPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DataPath) ?? string.Empty);
                return new List<T>();
            }

            var json = File.ReadAllText(DataPath);

            if (string.IsNullOrWhiteSpace(json)) return new List<T>();

            var state = JsonSerializer.Deserialize<List<T>>(json);

            return state ?? new List<T>();
        }

        public void Commit()
        {
            var json = JsonSerializer.Serialize(Records, new JsonSerializerOptions { WriteIndented = true });
            try
            {
                File.WriteAllText(DataPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data to file: {ex.Message}");
                throw;
            }
        }

        public T Add(T data)
        {
            var idValue = GetIDValue(data);
            var exists = Records.Any(r => GetIDValue(r).Equals(idValue));
            if (exists)
            {
                throw new Exception($"{typeof(T).Name} with {IDENTIFIER} = {idValue} already exists.");
            }

            Records.Add(data);
            return data;
        }

        private object GetIDValue(T data)
        {
            var idValue = data?.GetType().GetProperty(IDENTIFIER)?.GetValue(data);
            if (idValue == null)
            {
                throw new Exception($"Identifier '{IDENTIFIER}' value is null in type {typeof(T).Name}");
            }
            return idValue;
        }

        public void Update(T data)
        {
            var idValue = GetIDValue(data);
            if (idValue == null)
            {
                throw new Exception($"Identifier '{IDENTIFIER}' value is null in type {typeof(T).Name}");
            }
            var index = Records.FindIndex(r => GetIDValue(r).Equals(idValue));
            if (index >= 0)
            {
                Records[index] = data;
            }
        }

        public void Delete(T data)
        {
            var idValue = GetIDValue(data);
            Records.RemoveAll(r => GetIDValue(r).Equals(idValue));
        }

        public List<T> Find(Func<T, bool> predicate)
        {
            return Records.Where(predicate).ToList();
        }

        public T? FindOne(Func<T, bool> predicate)
        {
            return Records.FirstOrDefault(predicate);
        }

        public IReadOnlyList<T> All => Records.AsReadOnly();
    }
}