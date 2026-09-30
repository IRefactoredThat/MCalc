using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CalculatorApp.CalculatorPage.Page;

namespace CalculatorApp.CalculatorPage.History;

public partial class HistoryView
{
    public HistoryView() => InitializeComponent();

    public static readonly BindableProperty PageViewModelProperty =
        BindableProperty.Create(
            nameof(PageViewModel),
            typeof(CalculatorPageViewModel),
            typeof(HistoryView),
            propertyChanged: OnPageViewModelChanged);

    public CalculatorPageViewModel PageViewModel
    {
        get => (CalculatorPageViewModel)GetValue(PageViewModelProperty);
        set => SetValue(PageViewModelProperty, value);
    }

    private static void OnPageViewModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not HistoryView historyView)
        {
            return;
        }

        if (oldValue is CalculatorPageViewModel { HistoryViewModel: var oldModel })
        {
            oldModel.Items.CollectionChanged -= historyView.OnItemsCollectionChanged;
        }

        if (newValue is CalculatorPageViewModel { HistoryViewModel: var newModel })
        {
            historyView.Overlay.Opacity = newModel.Items.Count == 0 ? 1 : 0;
            newModel.Items.CollectionChanged += historyView.OnItemsCollectionChanged;
        }
    }

    protected override void OnHandlerChanged()
    {
        if (Handler is null)
        {
            PageViewModel?.HistoryViewModel?
                .Items.CollectionChanged -= OnItemsCollectionChanged;
        }
        base.OnHandlerChanged();
    }

    private async void OnItemsCollectionChanged(object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (sender is ObservableCollection<HistoryItem> { Count: 0 })
        {
            await Overlay.FadeToAsync(1, easing: Easing.SinOut);
        }
        else
        {
            Overlay.Opacity = 0;
        }
    }
}
