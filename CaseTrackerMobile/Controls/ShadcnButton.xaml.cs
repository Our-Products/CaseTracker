using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CaseTrackerMobile.Controls
{
    public partial class ShadcnButton : ContentView
    {
        public ShadcnButton()
        {
            InitializeComponent();
        }

        #region Bindable Properties

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ShadcnButton), string.Empty);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ShadcnButton), null);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ShadcnButton), null);

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly BindableProperty IsLoadingProperty =
            BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(ShadcnButton), false);

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly BindableProperty ButtonColorProperty =
            BindableProperty.Create(nameof(ButtonColor), typeof(Color), typeof(ShadcnButton), Color.FromArgb("#113979"));

        public Color ButtonColor
        {
            get => (Color)GetValue(ButtonColorProperty);
            set => SetValue(ButtonColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ShadcnButton), Colors.White);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty VariantProperty =
            BindableProperty.Create(nameof(Variant), typeof(string), typeof(ShadcnButton), "Primary", propertyChanged: OnVariantChanged);

        public string Variant
        {
            get => (string)GetValue(VariantProperty);
            set => SetValue(VariantProperty, value);
        }

        #endregion

        private static void OnVariantChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShadcnButton button && newValue is string variant)
            {
                button.ApplyVariant(variant);
            }
        }

        private void ApplyVariant(string variant)
        {
            switch (variant.ToLowerInvariant())
            {
                case "secondary":
                    ButtonColor = Color.FromArgb("#F1F5F9");
                    TextColor = Color.FromArgb("#0F172A");
                    ButtonBorder.Stroke = null;
                    ButtonBorder.StrokeThickness = 0;
                    break;
                case "outline":
                    ButtonColor = Colors.White;
                    TextColor = Color.FromArgb("#1E293B");
                    ButtonBorder.Stroke = Color.FromArgb("#CBD5E1");
                    ButtonBorder.StrokeThickness = 1;
                    break;
                case "ghost":
                    ButtonColor = Colors.Transparent;
                    TextColor = Color.FromArgb("#1E293B");
                    ButtonBorder.Stroke = null;
                    ButtonBorder.StrokeThickness = 0;
                    break;
                case "destructive":
                    ButtonColor = Color.FromArgb("#EF4444");
                    TextColor = Colors.White;
                    ButtonBorder.Stroke = null;
                    ButtonBorder.StrokeThickness = 0;
                    break;
                case "primary":
                default:
                    ButtonColor = Color.FromArgb("#113979");
                    TextColor = Colors.White;
                    ButtonBorder.Stroke = null;
                    ButtonBorder.StrokeThickness = 0;
                    break;
            }
        }

        private async void OnButtonTapped(object? sender, EventArgs e)
        {
            if (IsLoading) return;

            // Subtle scale press animation (50ms down, 50ms up)
            await this.ScaleTo(0.96, 50, Easing.CubicOut);
            await this.ScaleTo(1.0, 50, Easing.CubicIn);

            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}
