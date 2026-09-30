using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Layout;
using CalculatorApp.SharedElements.ContentButton;

namespace CalculatorApp.SettingsPage.CustomLayout.LayoutBuilder;

public partial class LayoutBuilder
{
    public LayoutBuilder()
    {
        InitializeComponent();
        BindableLayout.SetItemsSource(Categories,
            Enum.GetValues<LayoutItemCategory>());
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        UpdateCheckedCategory(ActiveCategory);
        Options.ScrollTo(GetFirstItem((LayoutSettings)BindingContext, ActiveCategory),
            position: ScrollToPosition.Start, animate: false);
        Loaded -= OnLoaded;
    }

    private void OnCarouselTapped(object? sender, CellRange range) =>
        SpreadsheetGrid.TrySelectOccupied(range.Row, range.Column);

    public static readonly BindableProperty ActiveCategoryProperty =
        BindableProperty.Create(
            nameof(ActiveCategory),
            typeof(LayoutItemCategory),
            typeof(LayoutBuilder),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnActiveCategoryChanged);

    public LayoutItemCategory ActiveCategory
    {
        get => (LayoutItemCategory)GetValue(ActiveCategoryProperty);
        set => SetValue(ActiveCategoryProperty, value);
    }

    private static void OnActiveCategoryChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not LayoutBuilder builder)
        {
            return;
        }

        builder.UpdateCheckedCategory(newValue);
    }

    private void UpdateCheckedCategory(object? category)
    {
        foreach (var child in Categories.Children.OfType<ContentButton>())
        {
            var newState = Equals(child.BindingContext, category) ? "Checked" : "Normal";
            VisualStateManager.GoToState(child, newState);
            VisualStateManager.GoToState(child.Content, newState);
        }
    }

    private void OnScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        var layoutSettings = (LayoutSettings)BindingContext;
        if (e.FirstVisibleItemIndex < 0 ||
            e.FirstVisibleItemIndex >= layoutSettings.LayoutItems.Count)
        {
            return;
        }

        var item = layoutSettings.LayoutItems[e.FirstVisibleItemIndex];
        var category = item.Category;
        if (ActiveCategory != category)
        {
            ActiveCategory = category;
        }
    }

    private void OnCategoryClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not LayoutSettings layoutSettings ||
            sender is not BindableObject { BindingContext: LayoutItemCategory category })
        {
            return;
        }

        Options.ScrollTo(GetFirstItem(layoutSettings, category),
            position: ScrollToPosition.Start, animate: true);
    }

    private static IBaseButtonViewModel? GetFirstItem(
        LayoutSettings layoutSettings, LayoutItemCategory category)
    {
        var items = layoutSettings.LayoutItems;
        var low = 0;
        var high = items.Count - 1;
        var firstIndex = -1;

        while (low <= high)
        {
            int midIndex = low + (high - low) / 2;
            var midItem = items[midIndex];
            var midCategory = midItem.Category;

            if (midCategory >= category)
            {
                if (midCategory == category)
                {
                    firstIndex = midIndex;
                }

                high = midIndex - 1;
            }
            else
            {
                low = midIndex + 1;
            }
        }

        return firstIndex >= 0 ? items[firstIndex] : null;
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        SpreadsheetGrid.ClearGrid();
    }

    public static readonly BindableProperty LayoutHeightProperty =
        BindableProperty.Create(
            nameof(LayoutHeight),
            typeof(double),
            typeof(LayoutBuilder));

    public double LayoutHeight
    {
        get => (double)GetValue(LayoutHeightProperty);
        set => SetValue(LayoutHeightProperty, value);
    }

    public static readonly BindableProperty SelectorProperty =
        BindableProperty.Create(
            nameof(Selector),
            typeof(DataTemplateSelector),
            typeof(LayoutBuilder));

    public DataTemplateSelector Selector
    {
        get => (DataTemplateSelector)GetValue(SelectorProperty);
        set => SetValue(SelectorProperty, value);
    }
}
