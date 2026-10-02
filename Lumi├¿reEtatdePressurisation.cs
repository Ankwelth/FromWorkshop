// ============================================================
//  VENTILATION LIGHT CONTROLLER  —  Multi-blocs
//
//  SETUP :
//    1. Placez ce script dans un Programmable Block
//    2. Définissez vos paires ventilation/lumière dans la liste
//       PAIRS ci-dessous (autant de paires que vous voulez)
//    3. Lancez / Run le script
//
//  FORMAT D'UNE PAIRE :
//    new PairConfig("Nom Air Vent", "Nom Lumière"),
//
//  PLUSIEURS VENTS → UNE LUMIÈRE (pression = moyenne du groupe) :
//    new PairConfig(new[]{"Vent 1","Vent 2"}, "Status Light"),
// ============================================================

static readonly PairConfig[] PAIRS = new PairConfig[]
{
    // --- Modifiez / ajoutez vos paires ici ---
    new PairConfig("Air Vent",        "Status Light"),
    new PairConfig("Air Vent Sas",    "Status Light Sas"),
    new PairConfig("Air Vent Cargo",  "Status Light Cargo"),

    // Exemple multi-vents pour une même zone :
    // new PairConfig(new[]{"Air Vent A","Air Vent B"}, "Status Light Zone"),
};

// ── Couleurs ────────────────────────────────────────────────
static readonly Color COLOR_OK      = new Color(0,   255, 0);   // Vert  : pressurisé
static readonly Color COLOR_BAD     = new Color(255, 0,   0);   // Rouge : dépressurisé / venting
static readonly Color COLOR_PENDING = new Color(255, 0, 0);   // Orange: pressurisation en cours

// ── Paramètres lumière ───────────────────────────────────────
const float LIGHT_INTENSITY = 4f;
const float LIGHT_RADIUS    = 5f;

// ── Fréquence de mise à jour ─────────────────────────────────
// Update10 = appelé toutes les 10 ticks (~1/6 s)
// UPDATE_INTERVAL × 10 ticks = durée réelle entre deux lectures
const int UPDATE_INTERVAL = 15 ;  // 30 × ~1/6 s ≈ 5 secondes

// ════════════════════════════════════════════════════════════
//  Code interne — inutile de modifier en dessous
// ════════════════════════════════════════════════════════════

class PairConfig
{
    public string[] VentNames;
    public string   LightName;

    // Constructeur une seule ventilation
    public PairConfig(string vent, string light)
    {
        VentNames = new[] { vent };
        LightName = light;
    }

    // Constructeur plusieurs ventilations
    public PairConfig(string[] vents, string light)
    {
        VentNames = vents;
        LightName = light;
    }
}

class Pair
{
    public IMyAirVent[]     Vents;
    public IMyLightingBlock Light;
    public string           Label; // nom affiché dans l'echo
}

Pair[] _pairs;
int    _tick = 0;

// ── Initialisation ───────────────────────────────────────────
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    LoadPairs();
}

void LoadPairs()
{
    _pairs = new Pair[PAIRS.Length];
    for (int i = 0; i < PAIRS.Length; i++)
    {
        var cfg = PAIRS[i];
        var p   = new Pair();

        // Résolution des blocs de ventilation
        var ventList = new List<IMyAirVent>();
        foreach (var name in cfg.VentNames)
        {
            var v = GridTerminalSystem.GetBlockWithName(name) as IMyAirVent;
            if (v != null)
                ventList.Add(v);
            else
                Echo($"[WARN] Ventilation introuvable : \"{name}\"");
        }
        p.Vents = ventList.ToArray();

        // Résolution de la lumière
        p.Light = GridTerminalSystem.GetBlockWithName(cfg.LightName) as IMyLightingBlock;
        if (p.Light == null)
            Echo($"[WARN] Lumière introuvable : \"{cfg.LightName}\"");

        p.Label = cfg.LightName;
        _pairs[i] = p;
    }
}

// ── Boucle principale ────────────────────────────────────────
public void Main(string argument, UpdateType updateSource)
{
    // Rechargement manuel (bouton Run ou trigger)
    if ((updateSource & UpdateType.Terminal) != 0 ||
        (updateSource & UpdateType.Trigger)  != 0)
    {
        LoadPairs();
        Echo("Blocs rechargés.\n");
    }

    // Throttle : on n'agit que tous les UPDATE_INTERVAL cycles
    _tick++;
    if (_tick < UPDATE_INTERVAL) return;
    _tick = 0;

    var sb = new System.Text.StringBuilder();
    sb.AppendLine("[Ventilation Monitor]");

    for (int i = 0; i < _pairs.Length; i++)
    {
        var pair = _pairs[i];

        // Blocs manquants
        if (pair.Vents.Length == 0 || pair.Light == null)
        {
            sb.AppendLine($"  {pair.Label} → ⚠ blocs manquants");
            continue;
        }

        // Calcul : moyenne de pression + flag dépressurisation
        float totalOxy = 0f;
        bool  anyVenting = false;
        foreach (var v in pair.Vents)
        {
            totalOxy   += v.GetOxygenLevel();
            if (v.Depressurize) anyVenting = true;
        }
        float avgOxy = totalOxy / pair.Vents.Length;

        // Sélection de la couleur et du statut
        Color  col;
        string status;

        if (anyVenting)
        {
            col    = COLOR_BAD;
            status = $"Dépressurisation ({avgOxy * 100f:0.0} %)";
        }
        else if (avgOxy >= 0.99f)
        {
            col    = COLOR_OK;
            status = "Pressurisé  ✓";
        }
        else
        {
            col    = COLOR_PENDING;
            status = $"Pressurisation… ({avgOxy * 100f:0.0} %)";
        }

        // Application à la lumière
        pair.Light.Color     = col;
        pair.Light.Intensity = LIGHT_INTENSITY;
        pair.Light.Radius    = LIGHT_RADIUS;
        pair.Light.Enabled   = true;

        // Log dans l'echo
        string ventNames = string.Join(" + ", Array.ConvertAll(pair.Vents, v => v.CustomName));
        sb.AppendLine($"  [{pair.Label}]");
        sb.AppendLine($"    Vents  : {ventNames}");
        sb.AppendLine($"    Statut : {status}");
    }

    Echo(sb.ToString());
}

public void Save() { }