using System.ComponentModel;

namespace CalculatorApp.Shell;

using Shell = Microsoft.Maui.Controls.Shell;

public partial class ShellItem
{
    public ShellItem() => InitializeComponent();

    protected override void OnBindingContextChanged()
    {
        if (BindingContext is BaseShellItem oldItem)
        {
            oldItem.PropertyChanged -= OnItemPropertyChanged;
        }
        base.OnBindingContextChanged();
        if (BindingContext is BaseShellItem newItem)
        {
            newItem.PropertyChanged += OnItemPropertyChanged;
            UpdateVisuals(newItem.IsChecked);
        }
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(FlyoutItem.IsChecked) &&
            sender is FlyoutItem item)
        {
            UpdateVisuals(item.IsChecked);
        }
    }

    private void UpdateVisuals(bool isChecked)
    {
        VisualStateManager.GoToState(this, isChecked ? "Checked" : "Normal");
        VisualStateManager.GoToState(Icon, isChecked ? "Checked" : "Normal");
    }

    private void OnItemClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not FlyoutItem item)
        {
            return;
        }

        Shell.Current.CurrentItem = item;
        Shell.Current.FlyoutIsPresented = false;
    }
}
