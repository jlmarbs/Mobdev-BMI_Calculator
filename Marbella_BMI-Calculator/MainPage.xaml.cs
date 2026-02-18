using Microsoft.Maui.Controls;

namespace Marbella_BMI_Calculator;

public partial class MainPage : ContentPage
{
    private bool _isHeightInCm = true;
    private bool _isWeightInKg = true;

    public MainPage()
    {
        InitializeComponent();
    }

    // ─────────────────────────────────────────────
    //  Unit Toggle Handlers
    // ─────────────────────────────────────────────

    private void OnCmClicked(object sender, EventArgs e)
    {
        _isHeightInCm = true;
        HeightUnitLabel.Text = "centimeters";
        HeightEntry.Placeholder = "0";

        CmBtn.BackgroundColor = Color.FromArgb("#6C63FF");
        CmBtn.TextColor = Colors.White;
        FtBtn.BackgroundColor = Colors.Transparent;
        FtBtn.TextColor = Color.FromArgb("#555577");

        HeightEntry.Text = string.Empty;
        RecalculateIfPossible();
    }

    private void OnFtClicked(object sender, EventArgs e)
    {
        _isHeightInCm = false;
        HeightUnitLabel.Text = "feet (e.g. 5.9)";
        HeightEntry.Placeholder = "0.0";

        FtBtn.BackgroundColor = Color.FromArgb("#6C63FF");
        FtBtn.TextColor = Colors.White;
        CmBtn.BackgroundColor = Colors.Transparent;
        CmBtn.TextColor = Color.FromArgb("#555577");

        HeightEntry.Text = string.Empty;
        RecalculateIfPossible();
    }

    private void OnKgClicked(object sender, EventArgs e)
    {
        _isWeightInKg = true;
        WeightUnitLabel.Text = "kilograms";
        WeightEntry.Placeholder = "0";

        KgBtn.BackgroundColor = Color.FromArgb("#6C63FF");
        KgBtn.TextColor = Colors.White;
        LbBtn.BackgroundColor = Colors.Transparent;
        LbBtn.TextColor = Color.FromArgb("#555577");

        WeightEntry.Text = string.Empty;
        RecalculateIfPossible();
    }

    private void OnLbClicked(object sender, EventArgs e)
    {
        _isWeightInKg = false;
        WeightUnitLabel.Text = "pounds";
        WeightEntry.Placeholder = "0";

        LbBtn.BackgroundColor = Color.FromArgb("#6C63FF");
        LbBtn.TextColor = Colors.White;
        KgBtn.BackgroundColor = Colors.Transparent;
        KgBtn.TextColor = Color.FromArgb("#555577");

        WeightEntry.Text = string.Empty;
        RecalculateIfPossible();
    }

    // ─────────────────────────────────────────────
    //  Input Changed
    // ─────────────────────────────────────────────

    private void OnInputChanged(object sender, TextChangedEventArgs e)
    {
        // Live recalculation is optional — we calculate on button press
        // But we can hide result if inputs are cleared
        if (string.IsNullOrEmpty(HeightEntry.Text) || string.IsNullOrEmpty(WeightEntry.Text))
        {
            ResultCard.IsVisible = false;
        }
    }

    // ─────────────────────────────────────────────
    //  Calculate Button
    // ─────────────────────────────────────────────

    private async void OnCalculateClicked(object sender, EventArgs e)
    {
        if (!double.TryParse(HeightEntry.Text, out double rawHeight) || rawHeight <= 0)
        {
            await DisplayAlert("Invalid Input", "Please enter a valid height greater than 0.", "OK");
            return;
        }

        if (!double.TryParse(WeightEntry.Text, out double rawWeight) || rawWeight <= 0)
        {
            await DisplayAlert("Invalid Input", "Please enter a valid weight greater than 0.", "OK");
            return;
        }

        // Convert to SI units
        double heightInMeters = _isHeightInCm ? rawHeight / 100.0 : rawHeight * 0.3048;
        double weightInKg     = _isWeightInKg ? rawWeight          : rawWeight * 0.453592;

        if (heightInMeters <= 0 || weightInKg <= 0)
        {
            await DisplayAlert("Invalid Input", "Please enter values greater than zero.", "OK");
            return;
        }

        double bmi = weightInKg / (heightInMeters * heightInMeters);

        DisplayResult(bmi);
        await AnimateResultCard();
    }

    // ─────────────────────────────────────────────
    //  Helper: recalculate if both fields filled
    // ─────────────────────────────────────────────

    private void RecalculateIfPossible()
    {
        ResultCard.IsVisible = false;
    }

    // ─────────────────────────────────────────────
    //  Display Result
    // ─────────────────────────────────────────────

    private void DisplayResult(double bmi)
    {
        BmiValueLabel.Text = bmi.ToString("F1");

        string category, description;
        Color  ringColor, badgeColor;

        if (bmi < 18.5)
        {
            category    = "Underweight";
            description = "Your BMI is below the healthy range.\nConsider consulting a nutritionist\nto develop a balanced diet plan.";
            ringColor   = Color.FromArgb("#4FC3F7");
            badgeColor  = Color.FromArgb("#1565C0");
        }
        else if (bmi < 25.0)
        {
            category    = "Normal Weight";
            description = "Great job! Your BMI is within\nthe healthy range. Maintain your\ncurrent lifestyle habits.";
            ringColor   = Color.FromArgb("#81C784");
            badgeColor  = Color.FromArgb("#1B5E20");
        }
        else if (bmi < 30.0)
        {
            category    = "Overweight";
            description = "Your BMI is slightly above the\nhealthy range. Regular exercise and\na balanced diet are recommended.";
            ringColor   = Color.FromArgb("#FFB74D");
            badgeColor  = Color.FromArgb("#E65100");
        }
        else
        {
            category    = "Obese";
            description = "Your BMI indicates obesity.\nPlease consult a healthcare\nprofessional for personalized advice.";
            ringColor   = Color.FromArgb("#E57373");
            badgeColor  = Color.FromArgb("#B71C1C");
        }

        CategoryLabel.Text             = category;
        DescriptionLabel.Text          = description;
        ResultRing.Stroke              = ringColor;
        CategoryBadge.BackgroundColor  = badgeColor;

        ResultCard.IsVisible = true;
    }

    // ─────────────────────────────────────────────
    //  Animate Result Card
    // ─────────────────────────────────────────────

    private async Task AnimateResultCard()
    {
        ResultCard.Opacity     = 0;
        ResultCard.TranslationY = 20;

        await Task.WhenAll(
            ResultCard.FadeTo(1, 350, Easing.CubicOut),
            ResultCard.TranslateTo(0, 0, 350, Easing.CubicOut)
        );
    }
}