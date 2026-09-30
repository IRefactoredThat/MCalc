using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using CalculatorApp.CalculatorPage.History.Storage;
using CalculatorApp.SettingsPage.History;

namespace CalculatorApp.CalculatorPage.History;

public class HistoryViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ICommand InsertCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand DeleteAllCommand { get; }
    public ObservableCollection<HistoryItem> Items { get; }

    private readonly IHistoryStorage _storage;
    private readonly HistorySettings _historySettings;

    public ItemsUpdatingScrollMode ScrollMode
    {
        get;
        private set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScrollMode)));
        }
    } = ItemsUpdatingScrollMode.KeepLastItemInView;

    public HistoryViewModel(IHistoryStorage storage, HistorySettings historySettings)
    {
        _storage = storage;
        _historySettings = historySettings;

        Items = storage.GetItems();
        Items.CollectionChanged += OnItemsCollectionChanged;

        DeleteCommand = new Command<HistoryItem>(DeleteItem);
        DeleteAllCommand = new Command(DeleteAllItems);
        InsertCommand = new Command<HistoryItem>(AddItem);
    }

    private readonly SemaphoreSlim _saveGate = new(1, 1);

    private async void OnItemsCollectionChanged(object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        await _saveGate.WaitAsync();
        try
        {
            await _storage.SaveItems(Items);
        }
        finally
        {
            _saveGate.Release();
        }
    }

    private void AddItem(HistoryItem item)
    {
        switch (_historySettings.DuplicationBehavior)
        {
            case EntryDuplicationBehavior.Ignore:
                if (Items.Contains(item))
                    return;
                break;

            case EntryDuplicationBehavior.Move:
                Items.Remove(item);
                break;

            case EntryDuplicationBehavior.Allow:
                break;
        }

        if (Items.Count >= _historySettings.MaxEntryCount)
        {
            switch (_historySettings.OverflowBehavior)
            {
                case EntryOverflowBehavior.Keep:
                    return;

                case EntryOverflowBehavior.Overwrite:
                    Items.RemoveAt(0);
                    break;
            }
        }

        Items.Add(item);
    }

    private void DeleteItem(HistoryItem item)
    {
        ScrollMode = ItemsUpdatingScrollMode.KeepItemsInView;
        Items.Remove(item);
        ScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
    }

    private void DeleteAllItems()
    {
        Items.Clear();
    }
}
