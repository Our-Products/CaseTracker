using Microsoft.Maui.Controls;
using CaseTracker.Shared.Dtos;
using CaseTrackerApp.Resources;

namespace CaseTrackerApp.Controls;

public class CaseCard : ContentView
{
    public CaseCard()
    {
        var border = new Border
        {
            Padding = 12,
            Margin = new Thickness(8,4),
            Background = new SolidColorBrush(Theme.CardBackground),
            //CornerRadius = new CornerRadius(Theme.CardCornerRadius),
            Content = new VerticalStackLayout
            {
                Children =
                {
                    CreateBoundLabel("CaseNumber", bold: true),
                    CreateBoundLabel("Title"),
                    CreateBoundLabel("NextHearingDate", stringFormat: "Next: {0:dd MMM yyyy}", textColor: Theme.Muted)
                }
            }
        };

        Content = border;
    }

    static Label CreateBoundLabel(string path, string? stringFormat = null, bool bold = false, Microsoft.Maui.Graphics.Color? textColor = null)
    {
        var lbl = new Label();
        if (bold) lbl.FontAttributes = FontAttributes.Bold;
        if (textColor != null) lbl.TextColor = textColor;
        if (!string.IsNullOrEmpty(stringFormat))
            lbl.SetBinding(Label.TextProperty, new Binding(path, stringFormat: stringFormat));
        else
            lbl.SetBinding(Label.TextProperty, path);
        return lbl;
    }
}
