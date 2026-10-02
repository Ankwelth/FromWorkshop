// --- KONFIGURATION ---
const string LCD_NAME = "Schleusen_Monitor";
const string SCHARNIER_NAME = "TorScharnier";
const string KOLBEN_NAME = "TorKolben";
const string LICHT_GRUPPE_NAME = "TorLicht";

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource) {
    // Suchen
    IMyTextPanel lcd = FindBlock<IMyTextPanel>(LCD_NAME);
    IMyMotorStator scharnier = FindBlock<IMyMotorStator>(SCHARNIER_NAME);
    IMyPistonBase kolben = FindBlock<IMyPistonBase>(KOLBEN_NAME);
    IMyBlockGroup lichtGruppe = GridTerminalSystem.GetBlockGroupWithName(LICHT_GRUPPE_NAME);

    if (lcd != null) lcd.ContentType = ContentType.TEXT_AND_IMAGE;

    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    sb.AppendLine("=========================================");
    sb.AppendLine("    KONTROLLZENTRUM: HANGARSCHLEUSE      ");
    sb.AppendLine("=========================================");
    sb.AppendLine("");

    // --- SCHARNIER (Debugging integriert) ---
    if (scharnier != null) {
        float winkel = scharnier.Angle * (180f / 3.141592f);
        float max = scharnier.UpperLimitRad * (180f / 3.141592f);
        float prozent = (winkel / max) * 100f; // Vereinfacht für Test
        sb.AppendLine($" ◊ TOR-MECHANIK");
        sb.AppendLine($"   Status:     {(prozent <= 5 ? "GESCHLOSSEN" : prozent >= 95 ? "OFFEN" : "IN BEWEGUNG")}");
        sb.AppendLine($"   Winkel:     {winkel,6:F1}°");
        sb.AppendLine("");
    } else {
        sb.AppendLine(" ◊ TOR-MECHANIK: NICHT GEFUNDEN!");
        sb.AppendLine($"   (Suche nach: {SCHARNIER_NAME})");
        sb.AppendLine("");
    }
    
    // --- KOLBEN ---
    if (kolben != null) {
        sb.AppendLine($" ◊ TREPPEN-PLATTFORM");
        sb.AppendLine($"   Status:     {(kolben.CurrentPosition <= 0.1f ? "EINGEFAHREN" : "AUSGEFAHREN")}");
        sb.AppendLine("");
    } else {
        sb.AppendLine(" ◊ TREPPEN-PLATTFORM: NICHT GEFUNDEN!");
        sb.AppendLine("");
    }
    
    // --- LICHT ---
    if (lichtGruppe != null) {
        List<IMyLightingBlock> lichter = new List<IMyLightingBlock>();
        lichtGruppe.GetBlocksOfType(lichter);
        sb.AppendLine($" ◊ WARNSYSTEM");
        sb.AppendLine($"   Status:     {(lichter.Count > 0 && lichter[0].Enabled ? "AKTIV" : "STANDBY")}");
        sb.AppendLine("");
    }

    sb.AppendLine("=========================================");
    if (lcd != null) lcd.WriteText(sb.ToString());
}

// Robuste Suche
public T FindBlock<T>(string name) where T : class, IMyTerminalBlock {
    List<T> all = new List<T>();
    GridTerminalSystem.GetBlocksOfType<T>(all);
    foreach (var b in all) {
        if (b.CustomName.Contains(name)) return b;
    }
    return null;
}