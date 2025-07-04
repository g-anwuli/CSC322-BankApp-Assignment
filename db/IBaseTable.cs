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

}