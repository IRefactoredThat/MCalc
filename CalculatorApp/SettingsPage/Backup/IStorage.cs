namespace CalculatorApp.SettingsPage.Backup;

public interface IStorage<T> where T : class
{
    T Get();
    void Save(T item);
    Task SaveAsync(T item);
}
