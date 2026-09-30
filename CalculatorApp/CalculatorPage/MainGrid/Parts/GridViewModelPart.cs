using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CalculatorApp.CalculatorPage.Buttons;

namespace CalculatorApp.CalculatorPage.MainGrid.Parts;

public enum SubgridLayoutMode { Grid, Carousel }

public sealed class GridViewModelPart(SubgridLayoutMode layoutMode,
    int rowSpanExtension = 0, int columnSpanExtension = 0) :
    IEnumerable<IBaseButtonViewModel>
{
    private readonly List<IBaseButtonViewModel> _parts = [];

    public bool CanBeShownAsSingleButton([NotNullWhen(true)]
        out IBaseButtonViewModel? model)
    {
        if (_parts.Count == 1 &&
            RowSpanExtension == 0 &&
            ColumnSpanExtension == 0)
        {
            model = _parts[0];
            return true;
        }

        model = null;
        return false;
    }

    public bool HasNoButtons => _parts.Count == 0;

    public SubgridLayoutMode LayoutMode { get; set; } = layoutMode;
    public int RowSpanExtension { get; set; } = rowSpanExtension;
    public int ColumnSpanExtension { get; set; } = columnSpanExtension;

    public void Add(IBaseButtonViewModel model) => _parts.Add(model);
    public void Remove(IBaseButtonViewModel model) => _parts.Remove(model);

    public IEnumerator<IBaseButtonViewModel> GetEnumerator() => _parts.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
