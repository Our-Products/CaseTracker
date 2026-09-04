using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CaseTrackerMobile.Controls;

public partial class ShadcnMetricCard : ContentView
{
    public static readonly BindableProperty MetricTitleProperty =
        BindableProperty.Create(nameof(MetricTitle), typeof(string), typeof(ShadcnMetricCard), string.Empty);

    public static readonly BindableProperty MetricValueProperty =
        BindableProperty.Create(nameof(MetricValue), typeof(string), typeof(ShadcnMetricCard), string.Empty);

    public static readonly BindableProperty MetricIconProperty =
        BindableProperty.Create(nameof(MetricIcon), typeof(string), typeof(ShadcnMetricCard), string.Empty);

    public static readonly BindableProperty IconBgColorProperty =
        BindableProperty.Create(nameof(IconBgColor), typeof(Color), typeof(ShadcnMetricCard), Color.FromArgb("#F1F5F9"));

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(ShadcnMetricCard), string.Empty);

    public string MetricTitle
    {
        get => (string)GetValue(MetricTitleProperty);
        set => SetValue(MetricTitleProperty, value);
    }

    public string MetricValue
    {
        get => (string)GetValue(MetricValueProperty);
        set => SetValue(MetricValueProperty, value);
    }

    public string MetricIcon
    {
        get => (string)GetValue(MetricIconProperty);
        set => SetValue(MetricIconProperty, value);
    }

    public Color IconBgColor
    {
        get => (Color)GetValue(IconBgColorProperty);
        set => SetValue(IconBgColorProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public ShadcnMetricCard()
    {
        InitializeComponent();
    }
}
