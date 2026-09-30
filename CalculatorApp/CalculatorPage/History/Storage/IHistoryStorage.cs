using System.Collections.ObjectModel;

namespace CalculatorApp.CalculatorPage.History.Storage;

public interface IHistoryStorage
{
    ObservableCollection<HistoryItem> GetItems();
    Task SaveItems(ObservableCollection<HistoryItem> items);
}
