using System.ComponentModel;

namespace stockwatch.CommonViews;

public partial class LabelEntryView : ContentView
{
    const string DefaultNumericText = "0";
    const int DefaultDecimalPlace = 2;
    const decimal DefaultMaxNumberValue = decimal.MaxValue;
    const decimal DefaultMinNumberValue = decimal.MinValue;

    public static readonly BindableProperty LabelProperty = BindableProperty.Create(
        propertyName: nameof(Label),
        returnType: typeof(string),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(string),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty EntryPlaceholderProperty = BindableProperty.Create(
        propertyName: nameof(EntryPlaceholder),
        returnType: typeof(string),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(string),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty EntryTextTransfomProperty = BindableProperty.Create(
        propertyName: nameof(EntryTextTransform),
        returnType: typeof(TextTransform),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(TextTransform),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty UnitProperty = BindableProperty.Create(
        propertyName: nameof(Unit),
        returnType: typeof(string),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(string),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        propertyName: nameof(Text),
        returnType: typeof(string),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(string),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty TextWidthProperty = BindableProperty.Create(
        propertyName: nameof(TextWidth),
        returnType: typeof(GridLength?),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(GridLength?),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        propertyName: nameof(Keyboard),
        returnType: typeof(Keyboard),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(Keyboard),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty DecimalPlaceProperty = BindableProperty.Create(
        propertyName: nameof(DecimalPlace),
        returnType: typeof(int?),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(int?),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty MinNumberValueProperty = BindableProperty.Create(
        propertyName: nameof(MinNumberValue),
        returnType: typeof(decimal?),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(decimal?),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty MaxNumberValueProperty = BindableProperty.Create(
        propertyName: nameof(MaxNumberValue),
        returnType: typeof(decimal?),
        declaringType: typeof(LabelEntryView),
        defaultValue: default(decimal?),
        defaultBindingMode: BindingMode.TwoWay);

    public string Label
    {
        get => GetValue(LabelProperty) as string ?? string.Empty;
        set => SetValue(LabelProperty, value);
    }

    public string EntryPlaceholder
    {
        get => GetValue(EntryPlaceholderProperty) as string ?? string.Empty;
        set => SetValue(EntryPlaceholderProperty, value);
    }

    public TextTransform EntryTextTransform
    {
        get => (TextTransform)GetValue(EntryTextTransfomProperty);
        set => SetValue(EntryTextTransfomProperty, value);
    }

    public string Unit
    {
        get => GetValue(UnitProperty) as string ?? string.Empty;
        set => SetValue(UnitProperty, value);
    }

    public string Text
    {
        get => GetFormatText();
        set => SetValue(TextProperty, value);
    }

    public string NumericText
    {
        get => GetNumericText();
    }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength TextWidth
    {
        get => GetValue(TextWidthProperty) as GridLength? ?? new GridLength(1, GridUnitType.Star);
        set => SetValue(TextWidthProperty, value);
    }

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public int DecimalPlace
    {
        get => GetValue(DecimalPlaceProperty) as int? ?? DefaultDecimalPlace;
        set => SetValue(DecimalPlaceProperty, value);
    }

    public decimal MaxNumberValue
    {
        get => GetValue(MaxNumberValueProperty) as decimal? ?? DefaultMaxNumberValue;
        set => SetValue(MaxNumberValueProperty, value);
    }

    public decimal MinNumberValue
    {
        get => GetValue(MinNumberValueProperty) as decimal? ?? DefaultMinNumberValue;
        set => SetValue(MinNumberValueProperty, value);
    }

    public LabelEntryView()
    {
        InitializeComponent();
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        if (Keyboard == Keyboard.Numeric)
        {
            Text = GetNumericText();
        }
    }

    private void Entry_Unfocused(object sendern, FocusEventArgs e)
    {
        if (Keyboard == Keyboard.Numeric)
        {
            if (entEntry.Behaviors.Contains(numericValidationBehavior))
            {
                var number = decimal.Parse(NumericText, Thread.CurrentThread.CurrentCulture.NumberFormat);
                if (number < MinNumberValue)
                {
                    SetValue(TextProperty, MinNumberValue);
                }
                else if (number > MaxNumberValue)
                {
                    SetValue(TextProperty, MaxNumberValue);
                }
            }
            Text = GetFormatText();
        }
    }

    private void labelEntryView_Loaded(object sender, EventArgs e)
    {
        AttachNumericValidationBehavior();
    }

    private string GetFormatText()
    {
        var text = GetValue(TextProperty) as string ?? string.Empty;
        text = text.Trim();
        if (Keyboard != Keyboard.Numeric)
            return text;

        if (entEntry.IsFocused)
            return text;

        if (string.IsNullOrEmpty(text))
            return DefaultNumericText;

        var number = decimal.Parse(text, Thread.CurrentThread.CurrentCulture.NumberFormat);
        return number.ToString($"N{DecimalPlace}", Thread.CurrentThread.CurrentCulture.NumberFormat);
    }

    private string GetNumericText()
    {
        var text = GetValue(TextProperty) as string ?? string.Empty;
        text = text.Trim();
        if (Keyboard != Keyboard.Numeric || string.IsNullOrEmpty(text))
            return DefaultNumericText;

        var number = decimal.Parse(text, Thread.CurrentThread.CurrentCulture.NumberFormat);
        return number.ToString();
    }

    private void AttachNumericValidationBehavior()
    {
        if (Keyboard == Keyboard.Numeric)
        {
            var hasCustomizeMinAndMaxValue = MinNumberValue != DefaultMinNumberValue || MaxNumberValue != DefaultMaxNumberValue;
            if (hasCustomizeMinAndMaxValue)
            {
                entEntry.Behaviors.Add(numericValidationBehavior);
            }
        }
    }
}