using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.CalculatorPage.MainGrid.Parts;

namespace CalculatorApp.CalculatorPage.MainGrid.Display;

public class CalculatorLayout : Grid
{
    public static readonly BindableProperty SelectorProperty =
        BindableProperty.Create(
            nameof(Selector),
            typeof(DataTemplateSelector),
            typeof(CalculatorLayout));

    public DataTemplateSelector Selector
    {
        get => (DataTemplateSelector)GetValue(SelectorProperty);
        set => SetValue(SelectorProperty, value);
    }

    public static readonly BindableProperty ButtonSpacingProperty =
        BindableProperty.Create(
            nameof(ButtonSpacing),
            typeof(double),
            typeof(CalculatorLayout),
            propertyChanged: OnButtonSpacingChanged);

    public double ButtonSpacing
    {
        get => (double)GetValue(ButtonSpacingProperty);
        set => SetValue(ButtonSpacingProperty, value);
    }

    private static void OnButtonSpacingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CalculatorLayout layout ||
            newValue is not double spacing)
        {
            return;
        }

        layout.Padding = new Thickness(spacing);
        layout.ColumnSpacing = spacing;
        layout.RowSpacing = spacing;
    }

    public static readonly BindableProperty EnableCarouselTapsProperty =
        BindableProperty.Create(
            nameof(EnableCarouselInteraction),
            typeof(bool),
            typeof(CalculatorLayout),
            defaultValue: false);

    public bool EnableCarouselInteraction
    {
        get => (bool)GetValue(EnableCarouselTapsProperty);
        set => SetValue(EnableCarouselTapsProperty, value);
    }

    public event EventHandler<CellRange>? CarouselTapped;

    private void UpdateRows(int newRows)
    {
        while (RowDefinitions.Count > newRows)
        {
            RowDefinitions.RemoveAt(RowDefinitions.Count - 1);
        }

        while (RowDefinitions.Count < newRows)
        {
            RowDefinitions.Add(new RowDefinition());
        }
    }

    private void UpdateColumns(int newColumns)
    {
        while (ColumnDefinitions.Count > newColumns)
        {
            ColumnDefinitions.RemoveAt(ColumnDefinitions.Count - 1);
        }

        while (ColumnDefinitions.Count < newColumns)
        {
            ColumnDefinitions.Add(new ColumnDefinition());
        }
    }

    public static readonly BindableProperty GridViewModelProperty =
        BindableProperty.Create(
            nameof(GridViewModel),
            typeof(IGridViewModel),
            typeof(CalculatorLayout),
            propertyChanged: OnGridViewModelChanged);

    public IGridViewModel GridViewModel
    {
        get => (IGridViewModel)GetValue(GridViewModelProperty);
        set => SetValue(GridViewModelProperty, value);
    }

    private static void OnGridViewModelChanged(BindableObject bindable,
        object oldValue, object newValue)
    {
        if (bindable is not CalculatorLayout layout ||
            newValue is not IGridViewModel model)
        {
            return;
        }

        layout.UpdateLayout(model);
        if (oldValue is CustomGridViewModel oldModel)
        {
            oldModel.PartChanged -= layout.OnPartChanged;
        }
        if (newValue is CustomGridViewModel newModel)
        {
            newModel.PartChanged += layout.OnPartChanged;
        }
    }

    protected override void OnHandlerChanged()
    {
        if (Handler is null && GridViewModel is CustomGridViewModel model)
        {
            model.PartChanged -= OnPartChanged;
        }
        base.OnHandlerChanged();
    }

    private void UpdateLayout(IGridViewModel model)
    {
        UpdateRows(model.UnitHeight);
        UpdateColumns(model.UnitWidth);

        _rangeToView.Clear();
        Children.Clear();

        foreach (var (cell, part) in model.Parts)
        {
            UpdateView(part, cell);
        }
    }

    private readonly Dictionary<CellRange, View> _rangeToView = [];

    private void OnPartChanged(object? sender, PartChangedEventArgs e)
    {
        UpdateRows(GridViewModel.UnitHeight);
        UpdateColumns(GridViewModel.UnitWidth);

        var cell = e.AffectedCell;
        if (e.Action is PartChangedAction.Add)
        {
            var model = e.AffectedPart?.FirstOrDefault() ??
                throw new ArgumentException("Add invoked with more that one model");
            _rangeToView.Add(cell, AddButton(model, cell));
            return;
        }

        if (e.Action is PartChangedAction.Clear)
        {
            _rangeToView.Clear();
            Clear();
            return;
        }

        var affectedView = _rangeToView.GetValueOrDefault(cell);
        if (e.Action is PartChangedAction.Remove)
        {
            Children.Remove(affectedView);
            _rangeToView.Remove(cell);
            return;
        }

        if (e.Action is PartChangedAction.Update && e.AffectedPart is { } affectedPart)
        {
            Children.Remove(affectedView);
            UpdateView(affectedPart, cell);
        }
    }

    private void UpdateView(GridViewModelPart affectedPart, CellRange cell)
    {
        var view = affectedPart.CanBeShownAsSingleButton(out var model) ?
            AddButton(model, cell) : AddView(cell, affectedPart);
        _rangeToView[cell] = view;
    }

    private View AddButton(IBaseButtonViewModel model, CellRange range) =>
        AddButton(this, model, range.Row, range.Column, range.RowSpan, range.ColumnSpan);

    private View AddButton(Grid target, IBaseButtonViewModel model,
        int row, int column, int rowSpan, int columnSpan)
    {
        if (Selector.SelectTemplate(model, target)?.
                CreateContent() is not View view)
        {
            throw new InvalidOperationException(
                "Button template must create a View.");
        }

        Grid.SetRow(view, row);
        Grid.SetColumn(view, column);
        Grid.SetRowSpan(view, rowSpan);
        Grid.SetColumnSpan(view, columnSpan);

        view.BindingContext = model;

        target.Children.Add(view);
        return view;
    }

    private View AddView(CellRange range, GridViewModelPart part)
    {
        if (part.LayoutMode is SubgridLayoutMode.Carousel)
        {
            return AddCarousel(range, part);
        }

        var grid = CreateGrid(range, part);
        FillGrid(grid, part, range.ColumnSpan + part.ColumnSpanExtension);

        Grid.SetRow(grid, range.Row);
        Grid.SetColumn(grid, range.Column);
        Grid.SetRowSpan(grid, range.RowSpan);
        Grid.SetColumnSpan(grid, range.ColumnSpan);

        Children.Add(grid);
        return grid;
    }

    private CarouselView AddCarousel(CellRange range, GridViewModelPart part)
    {
        var carouselSize = Math.Max(1, (range.RowSpan + part.RowSpanExtension)
                           * (range.ColumnSpan + part.ColumnSpanExtension));
        var carouselView = new CarouselView()
        {
            ItemsSource = part.Chunk(carouselSize),

            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,

            ItemsLayout = new LinearItemsLayout(
                orientation: ItemsLayoutOrientation.Horizontal)
            {
                SnapPointsType = SnapPointsType.MandatorySingle,
                SnapPointsAlignment = SnapPointsAlignment.Center
            },

            ItemTemplate = new DataTemplate(() =>
            {
                var grid = CreateGrid(range, part);

                grid.BindingContextChanged += (_, _) =>
                {
                    if (grid.BindingContext
                        is not IEnumerable<IBaseButtonViewModel> items)
                    {
                        return;
                    }

                    FillGrid(grid, items, range.ColumnSpan + part.ColumnSpanExtension);
                };

                return grid;
            })
        };

        if (EnableCarouselInteraction)
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (_, _) => CarouselTapped?.Invoke(this, range);
            carouselView.GestureRecognizers.Add(tapGesture);
        }
        else
        {
            carouselView.Loaded += OnCarouselLoaded;
        }

        Grid.SetRow(carouselView, range.Row);
        Grid.SetColumn(carouselView, range.Column);
        Grid.SetRowSpan(carouselView, range.RowSpan);
        Grid.SetColumnSpan(carouselView, range.ColumnSpan);

        Children.Add(carouselView);
        return carouselView;
    }

    private void OnCarouselLoaded(object? sender, EventArgs e)
    {
        if (sender is not CarouselView carousel)
        {
            return;
        }

        carousel.ScrollTo(index: 0,
            position: ScrollToPosition.Center, animate: false);
        carousel.Loaded -= OnCarouselLoaded;
    }

    private Grid CreateGrid(CellRange range, GridViewModelPart part)
    {
        var grid = new Grid();
        grid.SetBinding(ColumnSpacingProperty,
            new Binding(nameof(ButtonSpacing), source: this));
        grid.SetBinding(RowSpacingProperty,
            new Binding(nameof(ButtonSpacing), source: this));

        var rowSpan = range.RowSpan + part.RowSpanExtension;
        var columnSpan = range.ColumnSpan + part.ColumnSpanExtension;

        for (var row = 0; row < rowSpan; row++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
        }

        for (var column = 0; column < columnSpan; column++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        return grid;
    }

    private void FillGrid(Grid grid,
        IEnumerable<IBaseButtonViewModel> models, int columnSpan)
    {
        grid.Children.Clear();
        var i = 0;

        foreach (var button in models)
        {
            var row = i / columnSpan;
            var column = i % columnSpan;

            AddButton(grid, button, row, column, 1, 1);
            i++;
        }
    }
}
