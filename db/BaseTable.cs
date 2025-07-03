using System.Text.Json;

namespace BankApp.Db
{
    /// <summary>
    /// Represents a generic interface for managing a persistent collection of records,
    /// typically stored as a JSON file on disk.
    /// </summary>
    /// <typeparam name="T">The type of the record being managed.</typeparam>
    public interface IDbTable<T>
    {
        /// <summary>
        /// Loads records from persistent storage (e.g., JSON file).
        /// </summary>
        /// <returns>A list of records of type <typeparamref name="T"/>.</returns>
        List<T> Load();

        /// <summary>
        /// Adds a new record to the table.
        /// Throws an exception if a record with the same identifier already exists.
        /// </summary>
        /// <param name="data">The record to add.</param>
        /// <returns>The added record.</returns>
        T Add(T data);

        /// <summary>
        /// Updates an existing record in the table by replacing the matching record based on the identifier.
        /// </summary>
        /// <param name="data">The updated record.</param>
        void Update(T data);

        /// <summary>
        /// Deletes a record from the table by matching the identifier.
        /// </summary>
        /// <param name="data">The record to delete.</param>
        void Delete(T data);

        /// <summary>
        /// Searches for and returns a list of records that match a given condition.
        /// </summary>
        /// <param name="predicate">A function to test each record.</param>
        /// <returns>A list of matching records.</returns>
        List<T> Find(Func<T, bool> predicate);

        /// <summary>
        /// Searches for and returns the first record that matches a given condition.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="predicate">A function to test the record.</param>
        /// <returns>A single matching record, or null.</returns>
        T? FindOne(Func<T, bool> predicate);

        /// <summary>
        /// Persists the current state of the records to storage.
        /// </summary>
        void Commit();

        /// <summary>
        /// Gets a read-only view of all records currently in memory.
        /// </summary>
        IReadOnlyList<T> All { get; }
    }

    /// <summary>
    /// A generic base implementation of <see cref="IDbTable{T}"/> that uses JSON file storage.
    /// Provides basic CRUD and query functionality.
    /// </summary>
    /// <typeparam name="T">The type of record stored in the table.</typeparam>
    public class DbTable<T> : IDbTable<T>
    {
        private List<T> Records = new();
        public string TableName { get; set; } = string.Empty;
        public string IDENTIFIER { get; set; } = "Id"; // Default identifier, can be overridden in derived classes
        private static string DataPathFolder { get; set; } = "/Users/macbook/Documents/practice/c#/BankApp/store/";
        private string DataPath => DataPathFolder + TableName + ".json";

        public DbTable(string tableName, string indentifier)
        {
            TableName = tableName;
            IDENTIFIER = indentifier;
            if (!Directory.Exists(DataPathFolder))
            {
                Directory.CreateDirectory(DataPathFolder);
            }
            Records = Load();
        }

        public List<T> Load()
        {
            if (!File.Exists(DataPath)) return new List<T>();
            var json = File.ReadAllText(DataPath);

            Console.WriteLine("Loading Table" + TableName);

            if (string.IsNullOrWhiteSpace(json)) return new List<T>();

            var state = JsonSerializer.Deserialize<List<T>>(json);

            return state ?? new List<T>();
        }

        public void Commit()
        {
            var json = JsonSerializer.Serialize(Records, new JsonSerializerOptions { WriteIndented = true });
            try
            {
                File.WriteAllText(DataPath, json); // This will create or overwrite the file
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
            // Find the record by identifier
            var index = Records.FindIndex(r => GetIDValue(data).Equals(idValue));
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

        // ✅ Find a single matching record
        public T? FindOne(Func<T, bool> predicate)
        {
            return Records.FirstOrDefault(predicate);
        }

        // Optional: Expose all records if needed (readonly)
        public IReadOnlyList<T> All => Records.AsReadOnly();
    }
}