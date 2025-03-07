using Application.Constants;
using Application.Services.Interfaces;
using Domain.Constants;
using Domain.Entities;
using stockwatch.Services.Providers;

namespace stockwatch.ChildViews;

public partial class MySymbolSettingView : ContentView
{
    public readonly IMessageService messageService;

    public int CurrencyDecimalPlace { get; } = ApplicationConsts.CurrencyDecimalPlace;

    public string SymbolId
    {
        get => entSymbol.Text;
        private set => entSymbol.Text = value;
    }
    public decimal ReferencePrice
    {
        get => decimal.Parse(entReferencePrice.NumericText);
        private set => entReferencePrice.Text = value.ToString();
    }
    public decimal CeilingPrice
    {
        get => decimal.Parse(entCeilingPrice.NumericText);
        private set => entCeilingPrice.Text = value.ToString();
    }
    public decimal FloorPrice
    {
        get => decimal.Parse(entFloorPrice.NumericText);
        private set => entFloorPrice.Text = value.ToString();
    }

    public MySymbolSettingView()
    {
        InitializeComponent();
        messageService = PlatformsServiceProvider.ServiceProvider.GetRequiredService<IMessageService>();
    }

    public bool AreValidInputs(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(SymbolId))
        {
            errorMessage = messageService.GetMessage(MessageConstants.MSG_PleaseInputSymbol);
            return false;
        }

        if (ReferencePrice <= 0)
        {
            errorMessage = messageService.GetMessage(MessageConstants.MSG_PleaseInputValidPrice);
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    public void InitReferenceSymbolInfo(ReferenceSymbolEntity symbol)
    {
        SymbolId = symbol.Id;
        ReferencePrice = symbol.InitializedPrice;
        CeilingPrice = symbol.CeilingPricePercentage;
        FloorPrice = symbol.FloorPricePercentage;
    }

    public ReferenceSymbolEntity CreateReferenceSymbolInfo()
    {
        return new()
        {
            Id = SymbolId,
            InitializedPrice = ReferencePrice,
            CeilingPricePercentage = CeilingPrice,
            FloorPricePercentage = FloorPrice
        };
    }
}