using System.ComponentModel;

namespace stockwatch.CommonViews;

public partial class LabelPickerView : ContentView
{

    public static readonly BindableProperty LabelProperty = BindableProperty.Create(
        propertyName: nameof(Label),
        returnType: typeof(string),
        declaringType: typeof(LabelPickerView),
        defaultValue: default(string),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty UnitProperty = BindableProperty.Create(
        propertyName: nameof(Unit),
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

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(ItemsSource),
        returnType: typeof(IList<string>),
        declaringType: typeof(LabelPickerView),
        defaultValue: default(IList<string>),
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        propertyName: nameof(SelectedItem),
        returnType: typeof(object),
        declaringType: typeof(LabelPickerView),
        defaultValue: default,
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty SelectedIndex1Property = BindableProperty.Create(
        propertyName: nameof(SelectedIndex),
        returnType: typeof(int),
        declaringType: typeof(LabelPickerView),
        defaultValue: default(int),
        defaultBindingMode: BindingMode.TwoWay);

    public string Label
    {
        get => GetValue(LabelProperty) as string ?? string.Empty;
        set => SetValue(LabelProperty, value);
    }

    public string Unit
    {
        get => GetValue(UnitProperty) as string ?? string.Empty;
        set => SetValue(UnitProperty, value);
    }

    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength PickerWidth
    {
        get => GetValue(PickerWidthProperty) as GridLength? ?? new GridLength(1, GridUnitType.Star);
        set => SetValue(PickerWidthProperty, value);
    }

    public IList<string> ItemsSource
    {
        get { return (IList<string>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public int SelectedIndex
    {
        get { return pckPicker.SelectedIndex; }
        set
        {
            pckPicker.SelectedIndex = value;
            SetValue(SelectedIndex1Property, value);
        }
    }

    public object SelectedItem
    {
        get { return pckPicker.SelectedItem; }
        private set { SetValue(SelectedItemProperty, value); }
    }

    public LabelPickerView()
    {
        InitializeComponent();
    }

    private void PckPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedItem = pckPicker.SelectedItem;
        SelectedIndex = pckPicker.SelectedIndex;
    }
}