using Microsoft.Maui.Controls;
using CaseTrackerApp.Resources;

namespace CaseTrackerApp.Controls;

public class StatCard : ContentView
{
    readonly Label _valueLabel;
    readonly Label _titleLabel;
    readonly Border _border;

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(string), typeof(StatCard), string.Empty, propertyChanged: OnValueChanged);
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(StatCard), string.Empty, propertyChanged: OnTitleChanged);

    public string Value
    {
        get => (string)GetValue(ValueProperty)!;
        set => SetValue(ValueProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty)!;
        set => SetValue(TitleProperty, value);
    }

    public StatCard()
    {
        _valueLabel = new Label { FontSize = 24, FontAttributes = FontAttributes.Bold };
        _titleLabel = new Label { FontSize = 12, TextColor = Theme.Muted };

        _border = new Border
        {
            Padding = 12,
            Background = new SolidColorBrush(Theme.CardBackground),
            //CornerRadius = new CornerRadius(Theme.CardCornerRadius),
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children = { _valueLabel, _titleLabel }
            }
        };

        Content = _border;
    }

    static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((StatCard)bindable)._valueLabel.Text = newValue?.ToString() ?? string.Empty;
    }

    static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((StatCard)bindable)._titleLabel.Text = newValue?.ToString() ?? string.Empty;
    }
}
