using System.Collections.Generic;

namespace desktop_app.Models.Economy;

public class EconomySettings
{
    public decimal DailyCurrencyReward { get; set; }
    public decimal BotBasePrice { get; set; }
    public decimal BotStatMultiplier { get; set; }
    public decimal BotElementMultiplier { get; set; }
    public bool IsPlayerToPlayerTaxEnabled { get; set; }
    public bool IsPlayerToBotTaxEnabled { get; set; }
    public string BotSaleFormula { get; set; } = "";
    public List<MarketTaxBracket> ProgressiveTaxBrackets { get; set; } = new();
}
