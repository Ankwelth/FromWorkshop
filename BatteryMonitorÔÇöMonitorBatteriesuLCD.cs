// ╔══════════════════════════════════════════════════════╗
//  ⚡  BATTERY MONITOR  —  Space Engineers
//  Mostra lo stato delle batterie su un LCD
// ╚══════════════════════════════════════════════════════╝
//
//  SETUP:
//  1. Crea un gruppo batterie in-game (G → Gruppi)
//     e dagli un nome (es. "Batterie")
//  2. Metti un LCD nel terminale e dagli un nome
//     (es. "LCD Batteria")
//  3. Modifica BATTERY_GROUP e LCD_NAME qui sotto
//  4. Carica lo script nel Blocco Programmabile → OK

// ── Configurazione ─────────────────────────────────────
const string BATTERY_GROUP = "Batterie";   // nome del gruppo
const string LCD_NAME      = "LCD Batteria"; // nome dell'LCD
// ───────────────────────────────────────────────────────

public Program()
{
    // Aggiorna ogni 100 tick (~1,6 secondi)
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    // ── Trova LCD ──────────────────────────────────────
    var lcd = GridTerminalSystem.GetBlockWithName(LCD_NAME) as IMyTextPanel;
    if (lcd == null)
    {
        Echo($"⚠ LCD non trovato: '{LCD_NAME}'");
        return;
    }

    // ── Trova gruppo batterie ──────────────────────────
    var group = GridTerminalSystem.GetBlockGroupWithName(BATTERY_GROUP);
    if (group == null)
    {
        lcd.WriteText($"\n  ⚠ Gruppo '{BATTERY_GROUP}'\n     non trovato!");
        Echo($"⚠ Gruppo non trovato: '{BATTERY_GROUP}'");
        return;
    }

    var batteries = new List<IMyBatteryBlock>();
    group.GetBlocksOfType(batteries);

    if (batteries.Count == 0)
    {
        lcd.WriteText($"\n  ⚠ Nessuna batteria\n  nel gruppo '{BATTERY_GROUP}'");
        return;
    }

    // ── Calcola totali ─────────────────────────────────
    float totalCurrent = 0f, totalMax = 0f;
    float totalInput   = 0f, totalOutput = 0f;
    int   charging = 0, discharging = 0, idle = 0;

    foreach (var b in batteries)
    {
        totalCurrent += b.CurrentStoredPower;
        totalMax     += b.MaxStoredPower;
        totalInput   += b.CurrentInput;
        totalOutput  += b.CurrentOutput;

        switch (b.ChargeMode)
        {
            case ChargeMode.Recharge:    charging++;    break;
            case ChargeMode.Discharge:   discharging++; break;
            default:                     idle++;        break;
        }
    }

    float pct     = totalMax > 0 ? (totalCurrent / totalMax * 100f) : 0f;
    float netFlow = totalInput - totalOutput; // positivo = sta caricando

    // ── Barra colorata (22 caratteri) ──────────────────
    int   barLen = 22;
    int   filled = (int)Math.Round(pct / 100f * barLen);
    filled = Math.Max(0, Math.Min(barLen, filled));
    string bar = new string('█', filled) + new string('░', barLen - filled);

    // ── Colore e stato in base alla % ─────────────────
    Color  fontColor;
    string statusLabel;

    if (pct > 70f)      { fontColor = new Color( 80, 255, 140); statusLabel = "OTTIMO  ✔"; }
    else if (pct > 40f) { fontColor = new Color(255, 210,  40); statusLabel = "NORMALE ◆"; }
    else if (pct > 15f) { fontColor = new Color(255, 130,  20); statusLabel = "BASSO   ▲"; }
    else                { fontColor = new Color(255,  60,  60); statusLabel = "CRITICO ✖"; }

    // ── Freccia flusso ─────────────────────────────────
    string flowArrow;
    string flowVal;
    if      (netFlow >  0.001f) { flowArrow = "↑ Carica   "; flowVal = $"+{netFlow * 1000f:F0} kW"; }
    else if (netFlow < -0.001f) { flowArrow = "↓ Scarica  "; flowVal = $"-{Math.Abs(netFlow) * 1000f:F0} kW"; }
    else                        { flowArrow = "◆ Stabile  "; flowVal = "  0 kW"; }

    // ── Compone il testo LCD ───────────────────────────
    string text =
        "\n" +
        "  ⚡ BATTERY MONITOR\n" +
        "  ─────────────────────────\n\n" +
        $"  [{bar}]\n" +
        $"           {pct,5:F1}%\n\n" +
        $"  Stato    {statusLabel}\n" +
        $"  Flusso   {flowArrow}{flowVal}\n\n" +
        $"  Carica   {FormatEnergy(totalCurrent)}\n" +
        $"  Massima  {FormatEnergy(totalMax)}\n\n" +
        $"  Input    {totalInput  * 1000f,6:F1} kW\n" +
        $"  Output   {totalOutput * 1000f,6:F1} kW\n\n" +
        $"  Batterie {batteries.Count} blk\n" +
        "  ─────────────────────────";

    // ── Applica all'LCD ────────────────────────────────
    lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    lcd.Font        = "Monospace";
    lcd.FontSize    = 0.85f;
    lcd.FontColor   = fontColor;
    lcd.BackgroundColor = new Color(5, 10, 20); // sfondo quasi nero
    lcd.Alignment   = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
    lcd.WriteText(text);
}

// Converti MWh in unità leggibili
string FormatEnergy(float mwh)
{
    if (mwh >= 1f) return $"{mwh:F2} MWh";
    else           return $"{mwh * 1000f:F1} kWh";
}