namespace CalculatorApp.SettingsPage.Backup;

public interface IStorage<T>
{
    T Get();
    void Save(T item);
    Task SaveAsync(T item);
}
