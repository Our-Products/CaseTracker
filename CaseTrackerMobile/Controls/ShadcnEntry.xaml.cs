using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CaseTrackerMobile.Controls
{
    public partial class ShadcnEntry : ContentView
    {
        public ShadcnEntry()
        {
            InitializeComponent();
        }

        #region Bindable Properties

        public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText), typeof(string), typeof(ShadcnEntry), string.Empty);

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public static readonly BindableProperty RightLabelTextProperty =
            BindableProperty.Create(nameof(RightLabelText), typeof(string), typeof(ShadcnEntry), string.Empty);

        public string RightLabelText
        {
            get => (string)GetValue(RightLabelTextProperty);
            set => SetValue(RightLabelTextProperty, value);
        }

        public static readonly BindableProperty RightLabelCommandProperty =
            BindableProperty.Create(nameof(RightLabelCommand), typeof(ICommand), typeof(ShadcnEntry), null);

        public ICommand RightLabelCommand
        {
            get => (ICommand)GetValue(RightLabelCommandProperty);
            set => SetValue(RightLabelCommandProperty, value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ShadcnEntry), string.Empty, BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ShadcnEntry), string.Empty);

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly BindableProperty PrefixTextProperty =
            BindableProperty.Create(nameof(PrefixText), typeof(string), typeof(ShadcnEntry), string.Empty);

        public string PrefixText
        {
            get => (string)GetValue(PrefixTextProperty);
            set
            {
                SetValue(PrefixTextProperty, value);
                if (string.IsNullOrEmpty(LeadingIcon))
                {
                    LeadingIcon = value;
                }
            }
        }

        public static readonly BindableProperty LeadingIconProperty =
            BindableProperty.Create(nameof(LeadingIcon), typeof(string), typeof(ShadcnEntry), string.Empty, propertyChanged: OnLeadingIconChanged);

        public string LeadingIcon
        {
            get => (string)GetValue(LeadingIconProperty);
            set => SetValue(LeadingIconProperty, value);
        }

        private static void OnLeadingIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShadcnEntry control && newValue is string icon)
            {
                control.PrefixText = icon;
            }
        }

        public static readonly BindableProperty IsPasswordProperty =
            BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(ShadcnEntry), false);

        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }

        public static readonly BindableProperty KeyboardProperty =
            BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(ShadcnEntry), Keyboard.Default);

        public Keyboard Keyboard
        {
            get => (Keyboard)GetValue(KeyboardProperty);
            set => SetValue(KeyboardProperty, value);
        }

        public static readonly BindableProperty ShowTrailingActionProperty =
            BindableProperty.Create(nameof(ShowTrailingAction), typeof(bool), typeof(ShadcnEntry), false);

        public bool ShowTrailingAction
        {
            get => (bool)GetValue(ShowTrailingActionProperty);
            set => SetValue(ShowTrailingActionProperty, value);
        }

        public static readonly BindableProperty TrailingActionTextProperty =
            BindableProperty.Create(nameof(TrailingActionText), typeof(string), typeof(ShadcnEntry), string.Empty);

        public string TrailingActionText
        {
            get => (string)GetValue(TrailingActionTextProperty);
            set => SetValue(TrailingActionTextProperty, value);
        }

        public static readonly BindableProperty TrailingActionCommandProperty =
            BindableProperty.Create(nameof(TrailingActionCommand), typeof(ICommand), typeof(ShadcnEntry), null);

        public ICommand TrailingActionCommand
        {
            get => (ICommand)GetValue(TrailingActionCommandProperty);
            set => SetValue(TrailingActionCommandProperty, value);
        }

        public static readonly BindableProperty HasErrorProperty =
            BindableProperty.Create(nameof(HasError), typeof(bool), typeof(ShadcnEntry), false, propertyChanged: OnHasErrorChanged);

        public bool HasError
        {
            get => (bool)GetValue(HasErrorProperty);
            set => SetValue(HasErrorProperty, value);
        }

        public static readonly BindableProperty ErrorTextProperty =
            BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(ShadcnEntry), string.Empty);

        public string ErrorText
        {
            get => (string)GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        #endregion

        private static void OnHasErrorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShadcnEntry control)
            {
                control.UpdateBorderState();
            }
        }

        private void OnEntryFocused(object? sender, FocusEventArgs e)
        {
            UpdateBorderState(isFocused: true);
        }

        private void OnEntryUnfocused(object? sender, FocusEventArgs e)
        {
            UpdateBorderState(isFocused: false);
        }

        private void UpdateBorderState(bool isFocused = false)
        {
            if (InputBorder == null) return;

            if (HasError)
            {
                InputBorder.Stroke = Color.FromArgb("#EF4444"); // Red error border
                InputBorder.StrokeThickness = 1.5;
            }
            else if (isFocused)
            {
                InputBorder.Stroke = Color.FromArgb("#113979"); // Accent focus border
                InputBorder.StrokeThickness = 1.5;
            }
            else
            {
                InputBorder.Stroke = Color.FromArgb("#E2E8F0"); // Neutral border
                InputBorder.StrokeThickness = 1;
            }
        }
    }
}
