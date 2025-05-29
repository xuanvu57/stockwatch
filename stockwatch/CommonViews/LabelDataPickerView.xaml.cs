using System.ComponentModel;

namespace stockwatch.CommonViews;

public partial class LabelDataPickerView : ContentView
{
    const string DefaultFormat = "yyyy/MM/dd";

    public static readonly BindableProperty LabelProperty = BindableProperty.Create(
        propertyName: nameof(Label),
        returnType: typeof(string),
        declaringType: typeof(LabelPickerView),
        defaultValue: default(string),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty PickerWidthProperty = BindableProperty.Create(
        propertyName: nameof(PickerWidth),
        returnType: typeof(GridLength?),
        declaringType: typeof(LabelPickerView),
        defaultValue: default(GridLength?),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
        propertyName: nameof(SelectedDate),
        returnType: typeof(DateTime?),
        declaringType: typeof(LabelPickerView),
        defaultValue: default(DateTime?),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        propertyName: nameof(Format),
        returnType: typeof(string),
        declaringType: typeof(LabelPickerView),
        defaultValue: default(string),
        defaultBindingMode: BindingMode.TwoWay);

    public string Label
    {
        get => GetValue(LabelProperty) as string ?? string.Empty;
        set => SetValue(LabelProperty, value);
    }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength PickerWidth
    {
        get => GetValue(PickerWidthProperty) as GridLength? ?? new GridLength(1, GridUnitType.Star);
        set => SetValue(PickerWidthProperty, value);
    }

    public DateTime? SelectedDate
    {
        get => GetValue(SelectedDateProperty) as DateTime?;
        set => SetValue(SelectedDateProperty, value);
    }
    public string Format
    {
        get => GetValue(FormatProperty) as string ?? DefaultFormat;
        set => SetValue(FormatProperty, value);
    }

    public LabelDataPickerView()
    {
        InitializeComponent();

        dpkDate.MaximumDate = DateTime.Now.Date;
        SelectedDate = DateTime.Now.Date;
    }
}