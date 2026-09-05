using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CaseTrackerMobile.Controls
{
    public partial class ShadcnCard : ContentView
    {
        public ShadcnCard()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CardColorProperty =
            BindableProperty.Create(nameof(CardColor), typeof(Color), typeof(ShadcnCard), Colors.White);

        public Color CardColor
        {
            get => (Color)GetValue(CardColorProperty);
            set => SetValue(CardColorProperty, value);
        }

        public static readonly BindableProperty StrokeColorProperty =
            BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(ShadcnCard), Color.FromArgb("#E2E8F0"));

        public Color StrokeColor
        {
            get => (Color)GetValue(StrokeColorProperty);
            set => SetValue(StrokeColorProperty, value);
        }

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(ShadcnCard), new CornerRadius(16));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly BindableProperty CardPaddingProperty =
            BindableProperty.Create(nameof(CardPadding), typeof(Thickness), typeof(ShadcnCard), new Thickness(24));

        public Thickness CardPadding
        {
            get => (Thickness)GetValue(CardPaddingProperty);
            set => SetValue(CardPaddingProperty, value);
        }
    }
}
