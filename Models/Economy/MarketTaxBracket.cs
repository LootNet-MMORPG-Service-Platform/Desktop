namespace desktop_app.Models.Economy;

public class MarketTaxBracket
{
    public decimal From { get; set; }
    public decimal? To { get; set; }
    public decimal Rate { get; set; }
}
