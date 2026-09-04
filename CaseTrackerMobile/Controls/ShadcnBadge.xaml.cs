using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CaseTrackerMobile.Controls
{
    public partial class ShadcnBadge : ContentView
    {
        public ShadcnBadge()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ShadcnBadge), string.Empty);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(string), typeof(ShadcnBadge), string.Empty);

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly BindableProperty BadgeColorProperty =
            BindableProperty.Create(nameof(BadgeColor), typeof(Color), typeof(ShadcnBadge), Color.FromArgb("#EFF6FF"));

        public Color BadgeColor
        {
            get => (Color)GetValue(BadgeColorProperty);
            set => SetValue(BadgeColorProperty, value);
        }

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ShadcnBadge), Color.FromArgb("#BFDBFE"));

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ShadcnBadge), Color.FromArgb("#1E40AF"));

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty VariantProperty =
            BindableProperty.Create(nameof(Variant), typeof(string), typeof(ShadcnBadge), "default", propertyChanged: OnVariantChanged);

        public string Variant
        {
            get => (string)GetValue(VariantProperty);
            set => SetValue(VariantProperty, value);
        }

        private static void OnVariantChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShadcnBadge badge && newValue is string variant)
            {
                badge.ApplyVariant(variant);
            }
        }

        private void ApplyVariant(string variant)
        {
            switch (variant.ToLowerInvariant())
            {
                case "secondary":
                    BadgeColor = Color.FromArgb("#F1F5F9");
                    BorderColor = Color.FromArgb("#E2E8F0");
                    TextColor = Color.FromArgb("#475569");
                    break;
                case "outline":
                    BadgeColor = Colors.Transparent;
                    BorderColor = Color.FromArgb("#CBD5E1");
                    TextColor = Color.FromArgb("#334155");
                    break;
                case "destructive":
                    BadgeColor = Color.FromArgb("#FEE2E2");
                    BorderColor = Color.FromArgb("#FCA5A5");
                    TextColor = Color.FromArgb("#991B1B");
                    break;
                case "default":
                default:
                    BadgeColor = Color.FromArgb("#EFF6FF");
                    BorderColor = Color.FromArgb("#BFDBFE");
                    TextColor = Color.FromArgb("#1E40AF");
                    break;
            }
        }
    }
}
