using System.ComponentModel;

namespace stockwatch.CommonViews;

public partial class LabelEntryView : ContentView
{
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

    public static readonly BindableProperty LabelWidthProperty = BindableProperty.Create(
        propertyName: nameof(LabelWidth),
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
        get => GetValue(TextProperty) as string ?? string.Empty;
        set => SetValue(TextProperty, value);
    }

    public string NumericText
    {
        get => (Keyboard)GetValue(KeyboardProperty) == Keyboard.Numeric
            ? GetNumericText()
            : Text;
    }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength TextWidth
    {
        get => GetValue(TextWidthProperty) as GridLength? ?? new GridLength(1, GridUnitType.Star);
        set => SetValue(TextWidthProperty, value);
    }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength LabelWidth
    {
        get => GetValue(LabelWidthProperty) as GridLength? ?? new GridLength(1, GridUnitType.Auto);
        set => SetValue(LabelWidthProperty, value);
    }

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public LabelEntryView()
    {
        InitializeComponent();
    }

    private string GetNumericText()
    {
        if (Keyboard != Keyboard.Numeric || string.IsNullOrEmpty(Text))
            return "0";

        var number = decimal.Parse(Text, Thread.CurrentThread.CurrentCulture.NumberFormat);
        return number.ToString();
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        if (Keyboard == Keyboard.Numeric)
        {
            Text = GetNumericText();
        }
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        if (Keyboard == Keyboard.Numeric)
        {
            var number = decimal.Parse(GetNumericText());

            Text = number.ToString("N2", Thread.CurrentThread.CurrentCulture.NumberFormat);
        }
    }
}