using Android.Content;
using CalculatorApp.SettingsPage.Types.StepSlider;
using CalculatorApp.SharedElements.StepSlider;
using Google.Android.Material.Slider;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Slider = Microsoft.Maui.Controls.Slider;
using MaterialSlider = Google.Android.Material.Slider.Slider;

namespace CalculatorApp.Platforms.Android.Handlers;

internal sealed class MaterialStepSlider : MaterialSlider
{
    private readonly StepSlider _slider;

    public MaterialStepSlider(Context context, StepSlider slider) : base(context)
    {
        _slider = slider;
        LabelBehavior = LabelFormatter.LabelGone;
        Change += OnSliderChanged;
    }

    private void OnSliderChanged(object? sender, ChangeEventArgs e)
    {
        if (e.P2)
        {
            _slider.Value = e.P1;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Change -= OnSliderChanged;
        }

        base.Dispose(disposing);
    }
}

public class StepSliderHandler() : ViewHandler<StepSlider, MaterialSlider>(Mapper)
{
    private static readonly IPropertyMapper<StepSlider, StepSliderHandler> Mapper =
        new PropertyMapper<StepSlider, StepSliderHandler>(ViewMapper)
        {
            [nameof(Slider.Minimum)] = MapMinimum,
            [nameof(Slider.Maximum)] = MapMaximum,
            [nameof(Slider.Value)] = MapValue,
            [nameof(StepSlider.Step)] = MapStep,
            [nameof(Slider.ThumbColor)] = MapThumbColor,
            [nameof(Slider.MinimumTrackColor)] = MapMinimumTrackColor,
            [nameof(Slider.MaximumTrackColor)] = MapMaximumTrackColor,
            [nameof(VisualElement.IsEnabled)] = MapIsEnabled,
        };

    protected override MaterialSlider CreatePlatformView() =>
        new MaterialStepSlider(Context, VirtualView);

    private static void MapMinimum(StepSliderHandler handler, StepSlider slider)
    {
        handler.PlatformView.ValueFrom = (float)slider.Minimum;

        if (handler.PlatformView.Value < handler.PlatformView.ValueFrom)
        {
            handler.PlatformView.Value = handler.PlatformView.ValueFrom;
        }
    }

    private static void MapMaximum(StepSliderHandler handler, StepSlider slider)
    {
        handler.PlatformView.ValueTo = (float)slider.Maximum;

        if (handler.PlatformView.Value > handler.PlatformView.ValueTo)
        {
            handler.PlatformView.Value = handler.PlatformView.ValueTo;
        }
    }

    private static void MapValue(StepSliderHandler handler, StepSlider slider)
    {
        handler.PlatformView.Value = (float)slider.Value;
    }

    private static void MapStep(StepSliderHandler handler, StepSlider slider)
    {
        handler.PlatformView.StepSize = (float)slider.Step;
    }

    private static void MapThumbColor(StepSliderHandler handler, StepSlider slider)
    {
        handler.PlatformView.ThumbTintList =
            slider.ThumbColor.ToDefaultColorStateList();
    }

    private static void MapMinimumTrackColor(StepSliderHandler handler, StepSlider slider)
    {
        handler.PlatformView.TrackActiveTintList =
            slider.MinimumTrackColor.ToDefaultColorStateList();
    }

    private static void MapMaximumTrackColor(StepSliderHandler handler, StepSlider slider)
    {
        handler.PlatformView.TrackInactiveTintList =
            slider.MaximumTrackColor.ToDefaultColorStateList();
    }

    private static void MapIsEnabled(StepSliderHandler handler, StepSlider slider)
    {
        handler.PlatformView.Enabled = slider.IsEnabled;
    }
}