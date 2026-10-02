string PB_NAME="Refinery Manager PB";
const string ROLE_PREFIX="RM:Role=", STATE_PREFIX="RM:State=";
string LCD_INDIVIDUAL="RM:Individual", LCD_CENTRAL="RM:Central", IGNORE_TAG="RM:Ignore", MANUAL_DISABLE_TAG="RM:Manual", AUTO_DISABLED_TAG="RM_AutoDisabled";
const string VERSION = "5.0";

// CONFIGURATIE
double DEF_REFILL=0.80; 
double OUT_FULL=0.90, MAX_TICK=400.0;
double UPD_INT=1.0, PLAN_INT=0.50, SCAN_INT=5.0, SAVE_INT=30.0, PURGE_TIME=10.0, IDLE_TIME=10.0, NO_ORE=10.0, BLINK_INTERVAL_SECONDS=0.75;
double INSTRUCTION_BUDGET=0.82, TRANSFER_RETRY_SECONDS=2.0;
int _mode=2, BATCH=40, LCD_BATCH=10, SAFE=60, REN_BATCH=5, _barPos=0, _lastCnt=-1, _lcdIdx=0, _refIdx=0, _renIdx=0, _tOff=0, _doRefillIdx=0, _priorityRefillIdx=0, _noOreOffIdx=0;
int _roleSchema=0;
bool _rename=false, _ignore=false, _barFwd=true, _noOre=false, _warn=false, _debugLcd=false;
int _dbgLcdTotal=0, _dbgLcdFree=0, _dbgLcdCand=0, _dbgLcdLinked=0, _dbgLcdPanels=0, _dbgLcdDraw=0;
double _tUpd=0, _tPlan=0.50, _tScan=10.0, _tSave=0, _runClock=0;
DateTime _blinkT=DateTime.Now;
string _lang = "EN";

Dictionary<long,DateTime> _purgeT=new Dictionary<long,DateTime>(), _lastAct=new Dictionary<long,DateTime>();
Dictionary<long,double> _purgeEl=new Dictionary<long,double>();
Dictionary<long,RefineryState> _states=new Dictionary<long,RefineryState>();
Dictionary<long, string> _helperAssignments = new Dictionary<long, string>();
Dictionary<long, string> _replacementRoles = new Dictionary<long, string>();
HashSet<int> _setups=new HashSet<int>();
List<IMyRefinery> _refs=new List<IMyRefinery>(), _actRefs=new List<IMyRefinery>();
List<IMyRefinery> _tmpRefs=new List<IMyRefinery>(), _tmpTgt=new List<IMyRefinery>();
List<string> _reusableColumnList = new List<string>();
List<IMyCargoContainer> _cargos=new List<IMyCargoContainer>();
List<IMyTextPanel> _lcds=new List<IMyTextPanel>(), _cenLcds=new List<IMyTextPanel>();
List<IMyTextPanel> _lcdWork=new List<IMyTextPanel>(), _lcdFreeWork=new List<IMyTextPanel>(), _lcdGroupWork=new List<IMyTextPanel>();
List<IMyTerminalBlock> _allInv=new List<IMyTerminalBlock>(), _sources=new List<IMyTerminalBlock>();
Dictionary<string,MyFixedPoint> _oreLog=new Dictionary<string,MyFixedPoint>();
Dictionary<string,double> _ingotLog=new Dictionary<string,double>(), _baseOre=new Dictionary<string,double>();
Dictionary<string,MyFixedPoint> _oreScanBuild=new Dictionary<string,MyFixedPoint>();
Dictionary<string,double> _ingotScanBuild=new Dictionary<string,double>();
Dictionary<long,List<IMyTextPanel>> _lcdCache=new Dictionary<long,List<IMyTextPanel>>();
StringBuilder _sb=new StringBuilder();
List<MyInventoryItem> _invItems=new List<MyInventoryItem>();
List<MyInventoryItem> _pullItems=new List<MyInventoryItem>();
List<string> _singleOreList=new List<string>(), _physicalOreList=new List<string>();
List<IMyRefinery> _helpers=new List<IMyRefinery>(), _refillBatch=new List<IMyRefinery>();
List<IMyRefinery> _priorityRefs=new List<IMyRefinery>();
List<string> _availableOres=new List<string>();
List<string> _specialistRoles=new List<string> { "STONE", "IRON", "NICKEL", "SILICON", "COBALT", "MAGNESIUM", "SILVER", "GOLD", "PLATINUM", "URANIUM" };
List<MyInventoryItem> _queuedOreItems=new List<MyInventoryItem>(), _outputItems=new List<MyInventoryItem>();
Dictionary<string,double> _estOutput=new Dictionary<string,double>();
Dictionary<string,OreProg> _oreProgress=new Dictionary<string,OreProg>();
Dictionary<string,int> _activeOreCounts=new Dictionary<string,int>();
List<KeyValuePair<string,MyFixedPoint>> _sortedOreStock=new List<KeyValuePair<string,MyFixedPoint>>();
List<KeyValuePair<string,double>> _sortedIngots=new List<KeyValuePair<string,double>>(), _sortedEstimate=new List<KeyValuePair<string,double>>();
Dictionary<Vector3I,IMyTextPanel> _lcdPosMap=new Dictionary<Vector3I,IMyTextPanel>();
HashSet<IMyTextPanel> _lcdUnused=new HashSet<IMyTextPanel>(), _lcdVisited=new HashSet<IMyTextPanel>();
Queue<IMyTextPanel> _lcdQueue=new Queue<IMyTextPanel>();

HashSet<long> _tmpLongSet = new HashSet<long>();
HashSet<long> _idleHelperIds = new HashSet<long>();
HashSet<long> _priorityRefIds = new HashSet<long>();
HashSet<string> _priorityOres = new HashSet<string>();
HashSet<string> _activeRoles = new HashSet<string>();
List<string> _tmpStrings = new List<string>();

Dictionary<string, int> _distributionCounts = new Dictionary<string, int>();
int _oreScanIdx=0, _ingotScanBlockIdx=0, _ingotScanInvIdx=0, _lastSourceCount=-1, _lastInventoryBlockCount=-1, _lastLcdCount=-1, _lastCacheRefCount=-1;

Dictionary<string,Dictionary<string,double>> _conv=new Dictionary<string,Dictionary<string,double>>(){
    {"Iron",new Dictionary<string,double>{{"Iron",0.7}}},{"Silicon",new Dictionary<string,double>{{"Silicon",0.7}}},
    {"Nickel",new Dictionary<string,double>{{"Nickel",0.4}}},{"Cobalt",new Dictionary<string,double>{{"Cobalt",0.3}}},
    {"Silver",new Dictionary<string,double>{{"Silver",0.1}}},{"Gold",new Dictionary<string,double>{{"Gold",0.01}}},
    {"Uranium",new Dictionary<string,double>{{"Uranium",0.01}}},{"Magnesium",new Dictionary<string,double>{{"Magnesium",0.007}}},
    {"Platinum",new Dictionary<string,double>{{"Platinum",0.005}}},{"Stone",new Dictionary<string,double>{{"Iron",0.03},{"Nickel",0.0024},{"Silicon",0.004},{"Gravel",0.014}}},
    {"Scrap",new Dictionary<string,double>{{"Iron",0.8}}}
};
List<string> O_STONE=new List<string>{"Stone","Scrap"}, O_IRON=new List<string>{"Iron"}, O_NI=new List<string>{"Nickel"},
O_SI=new List<string>{"Silicon"}, O_CO=new List<string>{"Cobalt"}, O_MG=new List<string>{"Magnesium"},
O_AG=new List<string>{"Silver"}, O_AU=new List<string>{"Gold"}, O_PT=new List<string>{"Platinum"},
O_U=new List<string>{"Uranium"}, O_MID=new List<string>{"Cobalt","Magnesium","Silver"},
O_HIGH=new List<string>{"Gold","Platinum","Uranium"}, O_BSC=new List<string>{"Stone","Scrap","Iron","Nickel","Silicon"},
O_ALL=new List<string>{"Stone","Scrap","Iron","Nickel","Silicon","Cobalt","Magnesium","Silver","Gold","Platinum","Uranium"};
Dictionary<string, string> ORE_ABBREVIATIONS = new Dictionary<string, string> {
    {"Iron", "Iron"}, {"Nickel", "Nickel"}, {"Silicon", "Silicon"}, {"Cobalt", "Cobalt"},
    {"Silver", "Silver"}, {"Gold", "Gold"}, {"Uranium", "Uranium"}, {"Magnesium", "Magnesium"},
    {"Platinum", "Platinum"}, {"Stone", "Stone"}, {"Scrap", "Scrap"}, {"Ice", "Ice"}, {"Gravel", "Gravel"}
};
Dictionary<string, string> ROLE_TO_NAME_MAP = new Dictionary<string, string> {
    {"GENERALIST", "General Refinery"}, {"HELPER", "Helper Refinery"}, {"STONE", "Stone Refinery"},
    {"IRON", "Iron Refinery"}, {"NICKEL", "Nickel Refinery"}, {"SILICON", "Silicon Refinery"},
    {"COBALT", "Cobalt Refinery"}, {"MAGNESIUM", "Magnesium Refinery"}, {"SILVER", "Silver Refinery"},
    {"GOLD", "Gold Refinery"}, {"PLATINUM", "Platinum Refinery"}, {"URANIUM", "Uranium Refinery"},
    {"BULK", "Bulk Refinery"}, {"MID_TIER", "Mid Tier Refinery"}, {"HIGH_TIER", "High Tier Refinery"}
};
List<string> LANGS = new List<string>{"EN","NL","DE","FR","ES","PT","RU","PL","TR","IT"};
Dictionary<string,Dictionary<string,string>> ORE_NAMES = new Dictionary<string,Dictionary<string,string>> {
    {"NL", new Dictionary<string,string>{{"Iron","Ijzer"},{"Nickel","Nikkel"},{"Silicon","Silicium"},{"Cobalt","Kobalt"},{"Silver","Zilver"},{"Gold","Goud"},{"Uranium","Uranium"},{"Magnesium","Magnesium"},{"Platinum","Platina"},{"Stone","Steen"},{"Scrap","Schroot"},{"Ice","Ijs"},{"Gravel","Grind"}}},
    {"DE", new Dictionary<string,string>{{"Iron","Eisen"},{"Nickel","Nickel"},{"Silicon","Silizium"},{"Cobalt","Kobalt"},{"Silver","Silber"},{"Gold","Gold"},{"Uranium","Uran"},{"Magnesium","Magnesium"},{"Platinum","Platin"},{"Stone","Stein"},{"Scrap","Schrott"},{"Ice","Eis"},{"Gravel","Kies"}}},
    {"FR", new Dictionary<string,string>{{"Iron","Fer"},{"Nickel","Nickel"},{"Silicon","Silicium"},{"Cobalt","Cobalt"},{"Silver","Argent"},{"Gold","Or"},{"Uranium","Uranium"},{"Magnesium","Magnesium"},{"Platinum","Platine"},{"Stone","Pierre"},{"Scrap","Ferraille"},{"Ice","Glace"},{"Gravel","Gravier"}}},
    {"ES", new Dictionary<string,string>{{"Iron","Hierro"},{"Nickel","Niquel"},{"Silicon","Silicio"},{"Cobalt","Cobalto"},{"Silver","Plata"},{"Gold","Oro"},{"Uranium","Uranio"},{"Magnesium","Magnesio"},{"Platinum","Platino"},{"Stone","Piedra"},{"Scrap","Chatarra"},{"Ice","Hielo"},{"Gravel","Grava"}}},
    {"PT", new Dictionary<string,string>{{"Iron","Ferro"},{"Nickel","Niquel"},{"Silicon","Silicio"},{"Cobalt","Cobalto"},{"Silver","Prata"},{"Gold","Ouro"},{"Uranium","Uranio"},{"Magnesium","Magnesio"},{"Platinum","Platina"},{"Stone","Pedra"},{"Scrap","Sucata"},{"Ice","Gelo"},{"Gravel","Cascalho"}}},
    {"RU", new Dictionary<string,string>{{"Iron","Zhelezo"},{"Nickel","Nikel"},{"Silicon","Kremniy"},{"Cobalt","Kobalt"},{"Silver","Serebro"},{"Gold","Zoloto"},{"Uranium","Uran"},{"Magnesium","Magniy"},{"Platinum","Platina"},{"Stone","Kamen"},{"Scrap","Lom"},{"Ice","Led"},{"Gravel","Graviy"}}},
    {"PL", new Dictionary<string,string>{{"Iron","Zelazo"},{"Nickel","Nikiel"},{"Silicon","Krzem"},{"Cobalt","Kobalt"},{"Silver","Srebro"},{"Gold","Zloto"},{"Uranium","Uran"},{"Magnesium","Magnez"},{"Platinum","Platyna"},{"Stone","Kamien"},{"Scrap","Zlom"},{"Ice","Lod"},{"Gravel","Zwir"}}},
    {"TR", new Dictionary<string,string>{{"Iron","Demir"},{"Nickel","Nikel"},{"Silicon","Silikon"},{"Cobalt","Kobalt"},{"Silver","Gumus"},{"Gold","Altin"},{"Uranium","Uranyum"},{"Magnesium","Magnezyum"},{"Platinum","Platin"},{"Stone","Tas"},{"Scrap","Hurda"},{"Ice","Buz"},{"Gravel","Cakil"}}},
    {"IT", new Dictionary<string,string>{{"Iron","Ferro"},{"Nickel","Nichel"},{"Silicon","Silicio"},{"Cobalt","Cobalto"},{"Silver","Argento"},{"Gold","Oro"},{"Uranium","Uranio"},{"Magnesium","Magnesio"},{"Platinum","Platino"},{"Stone","Pietra"},{"Scrap","Rottame"},{"Ice","Ghiaccio"},{"Gravel","Ghiaia"}}}
};
Dictionary<string,Dictionary<string,string>> ROLE_NAMES = new Dictionary<string,Dictionary<string,string>> {
    {"NL", new Dictionary<string,string>{{"GENERALIST","Algemene Raffinaderij"},{"HELPER","Helper Raffinaderij"},{"STONE","Steen Raffinaderij"},{"IRON","Ijzer Raffinaderij"},{"NICKEL","Nikkel Raffinaderij"},{"SILICON","Silicium Raffinaderij"},{"COBALT","Kobalt Raffinaderij"},{"MAGNESIUM","Magnesium Raffinaderij"},{"SILVER","Zilver Raffinaderij"},{"GOLD","Goud Raffinaderij"},{"PLATINUM","Platina Raffinaderij"},{"URANIUM","Uranium Raffinaderij"},{"BULK","Bulk Raffinaderij"},{"MID_TIER","Middenklasse Raffinaderij"},{"HIGH_TIER","Hoogwaardige Raffinaderij"}}},
    {"DE", new Dictionary<string,string>{{"GENERALIST","Allgemeine Raffinerie"},{"HELPER","Helfer Raffinerie"},{"STONE","Stein Raffinerie"},{"IRON","Eisen Raffinerie"},{"NICKEL","Nickel Raffinerie"},{"SILICON","Silizium Raffinerie"},{"COBALT","Kobalt Raffinerie"},{"MAGNESIUM","Magnesium Raffinerie"},{"SILVER","Silber Raffinerie"},{"GOLD","Gold Raffinerie"},{"PLATINUM","Platin Raffinerie"},{"URANIUM","Uran Raffinerie"},{"BULK","Massen Raffinerie"},{"MID_TIER","Mittelklasse Raffinerie"},{"HIGH_TIER","Hochwertige Raffinerie"}}},
    {"FR", new Dictionary<string,string>{{"GENERALIST","Raffinerie Generale"},{"HELPER","Raffinerie Aide"},{"STONE","Raffinerie Pierre"},{"IRON","Raffinerie Fer"},{"NICKEL","Raffinerie Nickel"},{"SILICON","Raffinerie Silicium"},{"COBALT","Raffinerie Cobalt"},{"MAGNESIUM","Raffinerie Magnesium"},{"SILVER","Raffinerie Argent"},{"GOLD","Raffinerie Or"},{"PLATINUM","Raffinerie Platine"},{"URANIUM","Raffinerie Uranium"},{"BULK","Raffinerie Vrac"},{"MID_TIER","Raffinerie Moyenne"},{"HIGH_TIER","Raffinerie Haute"}}},
    {"ES", new Dictionary<string,string>{{"GENERALIST","Refineria General"},{"HELPER","Refineria Ayuda"},{"STONE","Refineria Piedra"},{"IRON","Refineria Hierro"},{"NICKEL","Refineria Niquel"},{"SILICON","Refineria Silicio"},{"COBALT","Refineria Cobalto"},{"MAGNESIUM","Refineria Magnesio"},{"SILVER","Refineria Plata"},{"GOLD","Refineria Oro"},{"PLATINUM","Refineria Platino"},{"URANIUM","Refineria Uranio"},{"BULK","Refineria Masiva"},{"MID_TIER","Refineria Media"},{"HIGH_TIER","Refineria Alta"}}},
    {"PT", new Dictionary<string,string>{{"GENERALIST","Refinaria Geral"},{"HELPER","Refinaria Auxiliar"},{"STONE","Refinaria Pedra"},{"IRON","Refinaria Ferro"},{"NICKEL","Refinaria Niquel"},{"SILICON","Refinaria Silicio"},{"COBALT","Refinaria Cobalto"},{"MAGNESIUM","Refinaria Magnesio"},{"SILVER","Refinaria Prata"},{"GOLD","Refinaria Ouro"},{"PLATINUM","Refinaria Platina"},{"URANIUM","Refinaria Uranio"},{"BULK","Refinaria Massa"},{"MID_TIER","Refinaria Media"},{"HIGH_TIER","Refinaria Alta"}}},
    {"RU", new Dictionary<string,string>{{"GENERALIST","Obshchiy Zavod"},{"HELPER","Zavod Pomoshchnik"},{"STONE","Zavod Kamen"},{"IRON","Zavod Zhelezo"},{"NICKEL","Zavod Nikel"},{"SILICON","Zavod Kremniy"},{"COBALT","Zavod Kobalt"},{"MAGNESIUM","Zavod Magniy"},{"SILVER","Zavod Serebro"},{"GOLD","Zavod Zoloto"},{"PLATINUM","Zavod Platina"},{"URANIUM","Zavod Uran"},{"BULK","Zavod Massovyy"},{"MID_TIER","Zavod Sredniy"},{"HIGH_TIER","Zavod Vysokiy"}}},
    {"PL", new Dictionary<string,string>{{"GENERALIST","Rafineria Ogolna"},{"HELPER","Rafineria Pomocnicza"},{"STONE","Rafineria Kamien"},{"IRON","Rafineria Zelazo"},{"NICKEL","Rafineria Nikiel"},{"SILICON","Rafineria Krzem"},{"COBALT","Rafineria Kobalt"},{"MAGNESIUM","Rafineria Magnez"},{"SILVER","Rafineria Srebro"},{"GOLD","Rafineria Zloto"},{"PLATINUM","Rafineria Platyna"},{"URANIUM","Rafineria Uran"},{"BULK","Rafineria Masowa"},{"MID_TIER","Rafineria Srednia"},{"HIGH_TIER","Rafineria Wysoka"}}},
    {"TR", new Dictionary<string,string>{{"GENERALIST","Genel Rafineri"},{"HELPER","Yardimci Rafineri"},{"STONE","Tas Rafinerisi"},{"IRON","Demir Rafinerisi"},{"NICKEL","Nikel Rafinerisi"},{"SILICON","Silikon Rafinerisi"},{"COBALT","Kobalt Rafinerisi"},{"MAGNESIUM","Magnezyum Rafinerisi"},{"SILVER","Gumus Rafinerisi"},{"GOLD","Altin Rafinerisi"},{"PLATINUM","Platin Rafinerisi"},{"URANIUM","Uranyum Rafinerisi"},{"BULK","Toplu Rafineri"},{"MID_TIER","Orta Rafineri"},{"HIGH_TIER","Yuksek Rafineri"}}},
    {"IT", new Dictionary<string,string>{{"GENERALIST","Raffineria Generale"},{"HELPER","Raffineria Aiuto"},{"STONE","Raffineria Pietra"},{"IRON","Raffineria Ferro"},{"NICKEL","Raffineria Nichel"},{"SILICON","Raffineria Silicio"},{"COBALT","Raffineria Cobalto"},{"MAGNESIUM","Raffineria Magnesio"},{"SILVER","Raffineria Argento"},{"GOLD","Raffineria Oro"},{"PLATINUM","Raffineria Platino"},{"URANIUM","Raffineria Uranio"},{"BULK","Raffineria Massa"},{"MID_TIER","Raffineria Media"},{"HIGH_TIER","Raffineria Alta"}}}
};
Dictionary<string, string[]> TXT = new Dictionary<string, string[]> {
    {"Version", new[]{"Version","Versie","Version","Version","Version","Versao","Versiya","Wersja","Surum","Versione"}}, {"Mode", new[]{"Mode","Modus","Modus","Mode","Modo","Modo","Rezhim","Tryb","Mod","Modo"}}, {"Auto Rename", new[]{"Auto Rename","Auto Naam","Auto Name","Renommer Auto","Auto Renombrar","Auto Renomear","Avto Imya","Auto Nazwa","Oto Ad","Rinomina Auto"}}, {"Ignore New", new[]{"Ignore New","Negeer Nieuw","Neue Ignorieren","Ignorer Nouveaux","Ignorar Nuevos","Ignorar Novos","Ignor Novye","Ignoruj Nowe","Yeniyi Yoksay","Ignora Nuove"}},
    {"CENTRAL OVERVIEW", new[]{"CENTRAL OVERVIEW","CENTRAAL OVERZICHT","ZENTRALE UBERSICHT","VUE CENTRALE","VISTA CENTRAL","VISAO CENTRAL","OBSHCHIY OBZOR","PODGLAD CENTRALNY","MERKEZI OZET","PANORAMICA CENTRALE"}}, {"Total", new[]{"Total","Totaal","Gesamt","Total","Total","Total","Vsego","Razem","Toplam","Totale"}}, {"Active", new[]{"Active","Actief","Aktiv","Actif","Activo","Ativo","Aktivno","Aktywne","Aktif","Attive"}}, {"Idle", new[]{"Idle","Inactief","Leerlauf","Inactif","Inactivo","Inativo","Prostoy","Bezczynne","Bosta","Inattive"}},
    {"ORE STOCK & STATUS", new[]{"ORE STOCK & STATUS","ERTS VOORRAAD & STATUS","ERZVORRAT & STATUS","STOCK MINERAI & STATUT","STOCK MINERAL & ESTADO","ESTOQUE MINERIO & STATUS","ZAPAS RUDY & STATUS","ZAPAS RUDY I STATUS","CEVHER STOK & DURUM","SCORTE MINERALI & STATO"}}, {"SYSTEM INGOTS", new[]{"SYSTEM INGOTS","SYSTEEM INGOTS","SYSTEMBARREN","LINGOTS SYSTEME","LINGOTES SISTEMA","LINGOTES SISTEMA","SLITKI SISTEMY","SZTABKI SYSTEMU","SISTEM KULCELERI","LINGOTTI SISTEMA"}},
    {"ESTIMATED OUTPUT", new[]{"ESTIMATED OUTPUT","GESCHATTE UITVOER","GESCHATZTE AUSGABE","SORTIE ESTIMEE","SALIDA ESTIMADA","SAIDA ESTIMADA","RASCHET VYHODA","SZACOWANE WYJSCIE","TAHMINI CIKTI","USCITA STIMATA"}}, {"Role", new[]{"Role","Rol","Rolle","Role","Rol","Funcao","Rol","Rola","Rol","Ruolo"}}, {"Status", new[]{"Status","Status","Status","Statut","Estado","Status","Status","Status","Durum","Stato"}}, {"Power", new[]{"Power","Vermogen","Leistung","Puissance","Potencia","Energia","Moshchnost","Moc","Guc","Potenza"}},
    {"Progression", new[]{"Progression","Voortgang","Fortschritt","Progression","Progreso","Progresso","Progress","Postep","Ilerleme","Avanzamento"}}, {"Modules", new[]{"Modules","Modules","Module","Modules","Modulos","Modulos","Moduli","Moduly","Moduller","Moduli"}}, {"INPUT", new[]{"INPUT","INVOER","EINGABE","ENTREE","ENTRADA","ENTRADA","VHOD","WEJSCIE","GIRIS","INGRESSO"}}, {"OUTPUT", new[]{"OUTPUT","UITVOER","AUSGABE","SORTIE","SALIDA","SAIDA","VYHOD","WYJSCIE","CIKTI","USCITA"}},
    {"Now", new[]{"Now","Nu","Jetzt","Actuel","Ahora","Agora","Seychas","Teraz","Simdi","Ora"}}, {"Queue", new[]{"Queue","Wachtrij","Warteschlange","File","Cola","Fila","Ochered","Kolejka","Kuyruk","Coda"}}, {"Empty", new[]{"Empty","Leeg","Leer","Vide","Vacio","Vazio","Pusto","Puste","Bos","Vuoto"}}, {"No ore", new[]{"No ore","Geen erts","Kein Erz","Pas de minerai","Sin mineral","Sem minerio","Net rudy","Brak rudy","Cevher yok","Nessun minerale"}},
    {"WARNING", new[]{"WARNING","WAARSCHUWING","WARNUNG","ATTENTION","ADVERTENCIA","AVISO","VNIMANIE","OSTRZEZENIE","UYARI","ATTENZIONE"}}, {"OUTPUT FULL", new[]{"OUTPUT FULL","UITVOER VOL","AUSGABE VOLL","SORTIE PLEINE","SALIDA LLENA","SAIDA CHEIA","VYHOD POLON","WYJSCIE PELNE","CIKTI DOLU","USCITA PIENA"}}, {"ON", new[]{"ON","AAN","AN","ON","ON","ON","VKL","WL","ACIK","ON"}}, {"OFF", new[]{"OFF","UIT","AUS","OFF","OFF","OFF","VYKL","WYL","KAPALI","OFF"}}, {"N/A", new[]{"N/A","N/B","N/A","N/A","N/A","N/A","N/A","N/D","YOK","N/D"}},
    {"X WAITING", new[]{"X WAITING","X WACHTEN","X WARTET","X ATTENTE","X ESPERA","X ESPERA","X ZHDET","X CZEKA","X BEKLIYOR","X ATTESA"}}, {"X IDLE HELPER", new[]{"X IDLE HELPER","X HELPER WACHT","X HELFER LEERLAUF","X AIDE INACTIVE","X AYUDA INACTIVA","X AUXILIAR INATIVO","X POMOSHCHNIK PROSTOY","X POMOC BEZCZYNNA","X YARDIMCI BOSTA","X AIUTO INATTIVO"}}, {"X IDLE ASSIST", new[]{"X IDLE ASSIST","X ASSIST WACHT","X ASSIST LEERLAUF","X ASSIST INACTIF","X ASISTE INACTIVO","X ASSIST INATIVO","X ASSIST PROSTOY","X ASYSTA BEZCZYNNA","X DESTEK BOSTA","X ASSIST INATTIVO"}}, 
    {"> ACTIVE HELPER", new[]{"> ACTIVE HELPER","> ACTIEVE HELPER","> AKTIVER HELFER","> AIDE ACTIVE","> AYUDA ACTIVA","> AUXILIAR ATIVO","> POMOSHCHNIK AKTIVEN","> POMOC AKTYWNA","> YARDIMCI AKTIF","> AIUTO ATTIVO"}}, {"> ASSISTING", new[]{"> ASSISTING","> ASSISTEERT","> ASSISTIERT","> ASSISTE","> ASISTIENDO","> ASSISTINDO","> POMOGAET","> ASYSTUJE","> DESTEKLIYOR","> ASSISTE"}}, {"> ACTIVE", new[]{"> ACTIVE","> ACTIEF","> AKTIV","> ACTIF","> ACTIVO","> ATIVO","> AKTIVNO","> AKTYWNE","> AKTIF","> ATTIVO"}},
    {"> SWITCHING TO SPECIALIST", new[]{"> SWITCHING TO SPECIALIST","> WISSELT NAAR SPECIALIST","> WECHSEL ZU SPEZIALIST","> PASSE AU SPECIALISTE","> CAMBIA A ESPECIALISTA","> TROCA PARA ESPECIALISTA","> PEREHOD K SPECIALISTU","> ZMIANA NA SPECJALISTE","> UZMANA GECIYOR","> PASSA A SPECIALISTA"}}, {"PURGING", new[]{"PURGING","LEEGMAKEN","ENTLEEREN","PURGE","PURGANDO","PURGANDO","OCHISTKA","CZYSZCZENIE","TEMIZLIYOR","SVUOTAMENTO"}}, {"NO POWER", new[]{"NO POWER","GEEN STROOM","KEIN STROM","PAS D'ENERGIE","SIN ENERGIA","SEM ENERGIA","NET ENERGII","BRAK MOCY","GUC YOK","SENZA ENERGIA"}}
};

class RefineryState { 
    public long Id; 
    public string Name, Status, PrimOre, JobType; 
    public bool IsAst, IsFilling, IsSwitching; 
    public double JobStart, LastIn, Prog, RetryAt;
}

class OreProg { public double Tot, Rem; }

public Program() {
    Me.CustomName = PB_NAME;
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    _tUpd = UPD_INT; 
    _tScan = SCAN_INT;
    InitCD(); 
    ParseCD(); 
    UpdCD(); 
    ParseStr(); 
    UpdStates();
    Save();
}

public void Save() {
    _sb.Clear();
    _sb.Append("v:"+VERSION+";m:"+_mode+";c:"+string.Join(",",_setups)+";s:");
    foreach(var k in _states)_sb.Append(k.Key+"="+k.Value.JobStart.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)+"="+(k.Value.JobType??"n")+"="+k.Value.LastIn.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)+"="+k.Value.Prog.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)+"|");
    _sb.Append(";b:");
    foreach(var k in _baseOre)_sb.Append(k.Key+"="+k.Value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)+"|");
    _sb.Append(";a:");
    foreach(var k in _helperAssignments)_sb.Append(k.Key+"="+k.Value+"|");
    _sb.Append(";r:");
    foreach(var k in _replacementRoles)_sb.Append(k.Key+"="+k.Value+"|");
    _sb.Append(";f:");
    foreach(var k in _states) if(k.Value.IsAst || k.Value.IsSwitching)_sb.Append(k.Key+"="+(k.Value.IsAst?"1":"0")+"="+(k.Value.IsSwitching?"1":"0")+"|");
    _sb.Append(";p:"+_refIdx+","+_doRefillIdx+","+_priorityRefillIdx);
    _sb.Append(";rs:2");
    _sb.Append(";");
    Storage=string.Join("",_sb);
}

void ParseStr() {
    _setups.Clear();
    if(string.IsNullOrEmpty(Storage))return;
    var p=Storage.Split(';');
    foreach(var x in p){
        var k=x.Split(':');
        if(k.Length<2)continue;
        if(k[0]=="m"){int m;if(int.TryParse(k[1],out m))if(m>=1&&m<=3)_mode=m;}
        else if(k[0]=="c"){var c=k[1].Split(',');foreach(var s in c){int i;if(int.TryParse(s,out i))_setups.Add(i);}}
        else if(k[0]=="s"){var e=k[1].Split('|');foreach(var y in e){var c=y.Split('=');if(c.Length>=3){long id;double v;if(long.TryParse(c[0],out id)&&TryParseStoredDouble(c[1],out v)){RefineryState s;if(!_states.TryGetValue(id,out s)){s=new RefineryState{Id=id};_states[id]=s;}s.JobStart=v;s.JobType=c[2]=="n"?null:c[2];if(c.Length>=4)TryParseStoredDouble(c[3],out s.LastIn);if(c.Length>=5)TryParseStoredDouble(c[4],out s.Prog);}}}}
        else if(k[0]=="b"){var e=k[1].Split('|');foreach(var y in e){var c=y.Split('=');if(c.Length==2){double v;if(TryParseStoredDouble(c[1],out v))_baseOre[c[0]]=v;}}}
        else if(k[0]=="a"){var e=k[1].Split('|');foreach(var y in e){var c=y.Split('=');long id;if(c.Length==2&&long.TryParse(c[0],out id))_helperAssignments[id]=c[1];}}
        else if(k[0]=="r"){var e=k[1].Split('|');foreach(var y in e){var c=y.Split('=');long id;if(c.Length==2&&long.TryParse(c[0],out id))_replacementRoles[id]=c[1];}}
        else if(k[0]=="f"){var e=k[1].Split('|');foreach(var y in e){var c=y.Split('=');long id;if(c.Length==3&&long.TryParse(c[0],out id)){RefineryState s;if(!_states.TryGetValue(id,out s)){s=new RefineryState{Id=id};_states[id]=s;}s.IsAst=c[1]=="1";s.IsSwitching=c[2]=="1";}}}
        else if(k[0]=="p"){var e=k[1].Split(',');if(e.Length==3){int.TryParse(e[0],out _refIdx);int.TryParse(e[1],out _doRefillIdx);int.TryParse(e[2],out _priorityRefillIdx);}}
        else if(k[0]=="rs"){int.TryParse(k[1],out _roleSchema);}
    }
}

bool TryParseStoredDouble(string value, out double result) {
    var style = System.Globalization.NumberStyles.Float;
    if(double.TryParse(value, style, System.Globalization.CultureInfo.InvariantCulture, out result)) return true;
    return double.TryParse(value, style, System.Globalization.CultureInfo.CurrentCulture, out result);
}

bool BudgetLow() {
    return Runtime.CurrentInstructionCount >= Runtime.MaxInstructionCount * INSTRUCTION_BUDGET;
}

string GetLcdDebug(bool lcdMode, int lim) {
    _sb.Clear();
    _sb.AppendLine("LCD DEBUG");
    _sb.AppendLine("Mode: " + GetModeDisplay(_mode) + " lcd=" + (lcdMode ? "ON" : "OFF"));
    if(_mode == 2) _sb.AppendLine("LCD drawing skipped: AI ONLY");
    _sb.AppendLine("Refs active/limit: " + _actRefs.Count + "/" + lim);
    _sb.AppendLine("LCD total/free/candidates: " + _dbgLcdTotal + "/" + _dbgLcdFree + "/" + _dbgLcdCand);
    _sb.AppendLine("Central panels: " + _cenLcds.Count);
    _sb.AppendLine("Linked refs/panels: " + _dbgLcdLinked + "/" + _dbgLcdPanels);
    _sb.AppendLine("Drawn this tick: " + _dbgLcdDraw);
    _sb.AppendLine("Cache entries: " + _lcdCache.Count);
    _sb.AppendLine("Run 'setmode:3' or 'setmode:1' for LCD output.");
    return _sb.ToString();
}

void ResetInventoryScans() {
    _oreScanIdx = 0;
    _ingotScanBlockIdx = 0;
    _ingotScanInvIdx = 0;
    _oreScanBuild.Clear();
    _ingotScanBuild.Clear();
}

public void Main(string arg, UpdateType src) {
    if(DateTime.Now >= _blinkT) {
        _warn = !_warn;
        _blinkT = DateTime.Now.AddSeconds(BLINK_INTERVAL_SECONDS);
    }
    string argLower = arg.ToLower();
    if(argLower == "changemode") {
        _mode++; if(_mode > 3) _mode = 1;
        UpdCD(); UpdPB(); Save();
        Echo("Modus:" + GetModeName(_mode));
    }
    else if(argLower.StartsWith("setmode:")) {
        string m = argLower.Substring(8);
        int n; if(int.TryParse(m, out n) && n >= 1 && n <= 3) { _mode = n; UpdCD(); UpdPB(); Save(); }
    }
    if(argLower == "debuglcd") { _debugLcd = true; Echo("LCD DEBUG ON"); }
    else if(argLower == "debugoff") { _debugLcd = false; Echo("LCD DEBUG OFF"); }
    bool rescan = argLower == "rescan" || argLower == "reset" || argLower == "debuglcd";
    if(argLower == "rename") { _rename = !_rename; UpdCD(); UpdPB(); }
    else if(argLower == "ignorenew") { _ignore = !_ignore; UpdCD(); UpdPB(); }
    bool ai = (_mode == 2 || _mode == 3);
    bool lcd = (_mode == 1 || _mode == 3);
    int lim = _actRefs.Count; 
    if(_mode == 3) lim = Math.Min(lim, SAFE);
    double dt = Math.Max(0.016, Runtime.TimeSinceLastRun.TotalSeconds);
    _runClock += dt;
    _tUpd += dt; 
    _tPlan += dt;
    _tScan += dt;
    _tSave += dt;
    try {
        if(_tScan >= SCAN_INT || rescan) {
            _tScan = 0; 
            _refs.Clear(); _cargos.Clear(); _lcds.Clear(); _allInv.Clear();
            GridTerminalSystem.GetBlocksOfType(_refs, b => b.IsSameConstructAs(Me) && b.IsFunctional);
            GridTerminalSystem.GetBlocksOfType(_cargos, b => b.IsSameConstructAs(Me) && b.IsFunctional);
            GridTerminalSystem.GetBlocksOfType(_lcds, l => l.IsSameConstructAs(Me) && l.IsFunctional);
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_allInv, b => b.IsSameConstructAs(Me) && b.HasInventory);
            if(_ignore) {
                foreach(var r in _refs) {
                    string c = r.CustomData ?? "";
                    if(!c.Contains(ROLE_PREFIX) && !c.Contains(IGNORE_TAG)) r.CustomData = IGNORE_TAG + "\n" + c;
                }
            }
            _actRefs.Clear();
            foreach(var r in _refs) {
                if(!r.CustomName.Contains("RESERVED") && !(r.CustomData ?? "").Contains(IGNORE_TAG)) _actRefs.Add(r);
            }
            int refCount = _actRefs.Count;
            if(_roleSchema < 2 || !_setups.Contains(refCount) || argLower == "reset") {
                UpdRoles(_actRefs);
                _roleSchema = 2;
                _setups.Add(refCount);
                Save();
                _lastCnt = refCount;
            } else if(refCount != _lastCnt) {
                UpdRoles(_actRefs);
                _lastCnt = refCount;
            }
            UpdStates();
            if(rescan || _lcds.Count != _lastLcdCount || _actRefs.Count != _lastCacheRefCount) {
                _lastLcdCount = _lcds.Count;
                _lastCacheRefCount = _actRefs.Count;
                CacheLcd();
            }
            _cargos.Clear();
            GridTerminalSystem.GetBlocksOfType(_cargos, b => b.IsSameConstructAs(Me) && b.IsFunctional);
            lim = _actRefs.Count;
            if(_mode == 3) lim = Math.Min(lim, SAFE);
        }
        if(_rename && _actRefs.Count > 0) {
            int p = 0;
            int renameCount = Math.Min(REN_BATCH, _actRefs.Count);
            while(p < renameCount) {
                if(_renIdx >= _actRefs.Count) _renIdx = 0;
                var r = _actRefs[_renIdx];
                if(r != null && !r.Closed) RenRef(r, _actRefs);
                _renIdx++; p++;
            }
        }
        bool hvy = false;
        if(_tUpd >= UPD_INT) {
            _tUpd = 0;
            ParseCD();
            AdvBar();
            _sources.Clear();
            foreach(var cargo in _cargos) if(!cargo.CustomName.Contains("[NoOre]")) _sources.Add(cargo);
            foreach(var r in _actRefs) if(!r.Closed && !r.CustomName.Contains("[NoOre]")) _sources.Add(r);
            if(_sources.Count != _lastSourceCount || _allInv.Count != _lastInventoryBlockCount) {
                _lastSourceCount = _sources.Count;
                _lastInventoryBlockCount = _allInv.Count;
                ResetInventoryScans();
            }
            ContinueOreScan(_sources);
            if(BudgetLow()) return;
            ContinueIngotScan(_allInv);
            if(BudgetLow()) return;
            double totalOre = 0;
            foreach(var oreAmount in _oreLog.Values) totalOre += (double)oreAmount;
            _noOre = totalOre <= NO_ORE;
            if(ai && _noOre) DoNoOreShutdown(_actRefs, _actRefs.Count);
            
            for(int i = 0; i < lim; i++) {
                var r = _actRefs[i];
                if(r.Closed) continue;
                if(r.IsProducing || GetInMass(r) > 0) {
                    if(_lastAct.ContainsKey(r.EntityId)) _lastAct.Remove(r.EntityId);
                } else if(!_lastAct.ContainsKey(r.EntityId)) {
                    _lastAct[r.EntityId] = DateTime.Now;
                }
            }
            UpdCen(_actRefs, _cenLcds, GetProg(_actRefs, _oreLog), _oreLog, _ingotLog);
            UpdPB();
            if(_tSave >= SAVE_INT && !BudgetLow()) { _tSave = 0; Save(); }
            hvy = true;
        }
        if(!hvy && _actRefs.Count > 0) {
            _tmpRefs.Clear();
            int tot = _actRefs.Count, cnt = 0;
            int monitorCount = Math.Min(BATCH, tot);
            while(cnt < monitorCount) {
                _refIdx++;
                if(_refIdx >= tot) _refIdx = 0;
                var r = _actRefs[_refIdx];
                if(r != null && !r.Closed) {
                    try {
                        MonRef(r, dt);
                        if(ai && (_mode == 2 || (_mode == 3 && _refIdx < lim))) _tmpRefs.Add(r);
                    } catch(Exception e) {
                        Echo("MON ERR: " + e.Message);
                    }
                }
                cnt++;
            }
            if(_tmpRefs.Count > 0) {
                _tOff += BATCH;
                int targetTot = Math.Min(lim, tot);
                if(_tOff >= targetTot) _tOff = 0;
                int end = Math.Min(_tOff + BATCH, targetTot);
                _tmpTgt.Clear();
                for(int i = _tOff; i < end; i++) _tmpTgt.Add(_actRefs[i]);
                DoPurge(_tmpRefs, _cargos, _tmpTgt);
                if(!_noOre) {
                     double dts = Runtime.TimeSinceLastRun.TotalSeconds;
                    DoMan(_tmpRefs, _oreLog, dts);
                }
            }

            if (ai && !_noOre && _tPlan >= PLAN_INT) {
                _tPlan = 0;
                FindSpecialistPriorities(_actRefs, lim);
                if(BudgetLow()) return;

                _availableOres.Clear();
                foreach(var ore in O_ALL) {
                    if(_oreLog.ContainsKey(ore) && _oreLog[ore] > 0 && !_priorityOres.Contains(ore)) _availableOres.Add(ore);
                }

                _helpers.Clear();
                for(int i = 0; i < lim; i++) {
                    var r = _actRefs[i];
                    if(_priorityRefIds.Contains(r.EntityId)) continue;
                    RefineryState s;
                    bool isHelper = GetRole(r) == "HELPER" || GetRole(r) == "GENERALIST";
                    if (_states.TryGetValue(r.EntityId, out s) && (isHelper || s.IsAst)) {
                        _helpers.Add(r);
                    }
                }
                AssignHelperTasks(_helpers, _availableOres);
                if(BudgetLow()) return;
                BuildDistributionCounts(_actRefs, lim);
                if(BudgetLow()) return;
                if(_priorityRefs.Count > 0) DoPriorityRefill(_priorityRefs, _sources);
                if(BudgetLow()) return;

                _refillBatch.Clear();
                cnt = 0;
                tot = lim;
                int refillCount = Math.Min(BATCH, tot);
                while(cnt < refillCount && tot > 0) {
                    if (_doRefillIdx >= tot) _doRefillIdx = 0;
                    _refillBatch.Add(_actRefs[_doRefillIdx]);
                    _doRefillIdx++;
                    cnt++;
                }
                // Gebruik cargo en refinery-inputs als bron, zodat bestaande grote batches breed verdeeld worden.
                if(_refillBatch.Count > 0) DoRefill(_refillBatch, _sources, _oreLog);

                // Pull verzorgt de startverdeling. DoShare blijft uit om herhaald heen-en-weer verplaatsen te voorkomen.
            } else if(ai && _noOre) {
                 DoNoOreShutdown(_actRefs, _actRefs.Count);
            }
        }
        if(lcd && _mode != 2) DoLcd(lim);
        if(_debugLcd) Echo(GetLcdDebug(lcd, lim));
    } catch(Exception e) {
        Echo("ERR: " + e.Message + "\n" + e.StackTrace);
    }
}

// DoMan: Schoonmaker nu met "Role-Based Immunity" om ping-pong te voorkomen
void DoMan(List<IMyRefinery> refs, Dictionary<string, MyFixedPoint> log, double dt)
{
    for (int r_idx = 0; r_idx < refs.Count; r_idx++)
    {
        if(BudgetLow()) return;
        try {
        var r = refs[r_idx];
        if(r.Closed) continue;
        if (GetSt(r) == "PURGING") continue;
        
        RefineryState s;
        if (!_states.TryGetValue(r.EntityId, out s)) continue;

        var roleOres = GetAllowed(r);
        var allowedOres = (s.IsAst && !IsBas(r)) ? O_ALL : GetPhysicalAllowed(r, roleOres);
        var inputInv = r.GetInventory(0);
        var inputItems = GetItems(inputInv);
        
        string jobLeader = null;
        if (inputItems.Count > 0) {
            foreach(var it in inputItems) if(it.Type.TypeId.EndsWith("Ore")) { jobLeader = it.Type.SubtypeId; break; }
        }

        bool isHelper = GetRole(r) == "HELPER" || GetRole(r) == "GENERALIST";
        bool assistBatch = s.IsAst && !isHelper && jobLeader != null && !roleOres.Contains(jobLeader);
        if(s.IsSwitching || (assistBatch && ChkAvail(roleOres, log))) {
            if(_runClock < s.RetryAt) continue;
            // Eigen specialist-erts heeft altijd voorrang: stop, werp de assistentbatch uit en schakel terug.
            r.Enabled = false;
            s.IsSwitching = true;
            bool emptied = true;
            for(int i = inputItems.Count - 1; i >= 0; i--) {
                var item = inputItems[i];
                if(item.Type.TypeId.EndsWith("Ore") && !ToCargo(item, inputInv, _cargos)) emptied = false;
            }
            if(emptied || GetInMass(r) <= 0.001) {
                s.IsAst = false;
                s.IsFilling = false;
                s.IsSwitching = false;
                s.JobType = null;
                _helperAssignments.Remove(r.EntityId);
                SafeOn(r);
            } else {
                s.RetryAt = _runClock + TRANSFER_RETRY_SECONDS;
            }
            continue;
        }

        // PING-PONG STOP: Als we aan het vullen zijn OF produceren, raakt DoMan NIETS aan dat bij de rol past.
        if (inputItems.Count > 0 && _cargos.Count > 0) {
            for (int i = inputItems.Count - 1; i >= 0; i--) {
                var item = inputItems[i];
                if (!item.Type.TypeId.EndsWith("Ore")) continue;
                
                string subtype = item.Type.SubtypeId;
                bool roleViolation = !allowedOres.Contains(subtype);
                
                // Belangrijk: Alleen items die NIET bij de rol horen OF die achter de leider liggen worden gepusht.
                // De "Job Leader" (het actieve erts) is HEILIG zolang het binnen de rol past.
                if (roleViolation || (jobLeader != null && subtype != jobLeader)) {
                    // Alleen pushen als we niet vullen of als het echt een role violation is
                    if (!s.IsFilling || roleViolation) {
                        ToCargo(item, inputInv, _cargos);
                    }
                }
            }
        }

        if (isHelper) { s.IsAst = false; SafeOn(r); continue; }

        // Verander specialist/assistent-status nooit midden in een lopende batch.
        // Dit voorkomt dat rollen, kleuren en ore-keuzes iedere cyclus wisselen.
        if(inputItems.Count > 0) { SafeOn(r); continue; }

        if (ChkAvail(roleOres, log)) {
            s.IsAst = false; SafeOn(r);
        } else {
            if (!s.IsAst) s.IsAst = true;
            if (s.IsAst) {
                if (!r.IsProducing && inputItems.Count == 0 && _lastAct.ContainsKey(r.EntityId)) {
                    if ((DateTime.Now - _lastAct[r.EntityId]).TotalSeconds > IDLE_TIME) {
                        if(r.Enabled && (r.CustomData ?? "").IndexOf(MANUAL_DISABLE_TAG, StringComparison.OrdinalIgnoreCase) < 0) r.Enabled = false;
                    } else SafeOn(r);
                } else SafeOn(r);
            }
        }
        } catch(Exception e) {
            Echo("REF ERR: " + e.Message);
        }
    }
}

void AssignHelperTasks(List<IMyRefinery> helpers, List<string> availableOres) {
    _tmpLongSet.Clear();
    _idleHelperIds.Clear();
    foreach(var helper in helpers) {
        if(BudgetLow()) return;
        _tmpLongSet.Add(helper.EntityId);
        if(!helper.IsProducing && helper.InputInventory.CurrentVolume.RawValue == 0) _idleHelperIds.Add(helper.EntityId);
    }
    _tmpStrings.Clear();
    foreach(var assignment in _helperAssignments) {
        // Alleen een lopende batch houdt zijn taak vast. Een lege refinery mag opnieuw verdeeld worden.
        bool remove = !_tmpLongSet.Contains(assignment.Key) || _idleHelperIds.Contains(assignment.Key);
        if(remove) _tmpStrings.Add(assignment.Key.ToString());
    }
    foreach(var keyText in _tmpStrings) {
        long key;
        if(long.TryParse(keyText, out key)) _helperAssignments.Remove(key);
    }

    if (helpers.Count == 0 || availableOres.Count == 0) return;
    int oreTypeCount = availableOres.Count;
    for(int helperIdx = 0; helperIdx < helpers.Count; helperIdx++) {
        if(BudgetLow()) return;
        var helper = helpers[helperIdx];
        string existingOre;
        var physicallyAllowed = GetPhysicalAllowed(helper, O_ALL);
        if(_helperAssignments.TryGetValue(helper.EntityId, out existingOre)) {
            if(physicallyAllowed.Contains(existingOre)) continue;
            _helperAssignments.Remove(helper.EntityId);
        }
        int oreIdx = helperIdx % oreTypeCount;
        for(int attempt = 0; attempt < oreTypeCount; attempt++) {
            string ore = availableOres[oreIdx];
            oreIdx++; if (oreIdx >= oreTypeCount) oreIdx = 0;
            if(physicallyAllowed.Contains(ore)) {
                _helperAssignments[helper.EntityId] = ore;
                break;
            }
        }
    }
}

void FindSpecialistPriorities(List<IMyRefinery> refineries, int limit) {
    _priorityRefs.Clear();
    _priorityRefIds.Clear();
    _priorityOres.Clear();

    for(int i = 0; i < limit; i++) {
        if(BudgetLow()) return;
        var r = refineries[i];
        if(r.Closed || GetSt(r) == "PURGING") continue;

        string role = GetRole(r);
        if(role == "HELPER" || role == "GENERALIST" || role == "BULK") continue;
        if(r.InputInventory.CurrentVolume.RawValue > 0 || r.IsProducing) continue;

        var roleOres = GetPhysicalAllowed(r, GetAllowed(r));
        bool hasOwnOre = false;
        foreach(var ore in roleOres) {
            if(_oreLog.ContainsKey(ore) && _oreLog[ore] > 0) {
                _priorityOres.Add(ore);
                hasOwnOre = true;
            }
        }
        if(hasOwnOre) {
            _priorityRefs.Add(r);
            _priorityRefIds.Add(r.EntityId);
            _helperAssignments.Remove(r.EntityId);
        }
    }
}

void DoPriorityRefill(List<IMyRefinery> specialists, List<IMyTerminalBlock> sources) {
    if(specialists.Count == 0) return;
    int processed = 0;
    while(processed < Math.Min(BATCH, specialists.Count) && !BudgetLow()) {
        if(_priorityRefillIdx >= specialists.Count) _priorityRefillIdx = 0;
        var specialist = specialists[_priorityRefillIdx];
        _priorityRefillIdx++;
        processed++;
        RefineryState state;
        if(!_states.TryGetValue(specialist.EntityId, out state)) continue;
        state.IsAst = false;
        Pull(specialist.InputInventory, GetPhysicalAllowed(specialist, GetAllowed(specialist)), sources, DEF_REFILL);
    }
}

void BuildDistributionCounts(List<IMyRefinery> refineries, int limit) {
    _distributionCounts.Clear();
    for(int i = 0; i < limit; i++) {
        if(BudgetLow()) return;
        var r = refineries[i];
        if(r.Closed || GetSt(r) == "PURGING") continue;

        RefineryState state;
        if(!_states.TryGetValue(r.EntityId, out state)) continue;

        string role = GetRole(r);
        bool isHelperRole = role == "HELPER" || role == "GENERALIST";
        string assignedOre;
        bool hasAssignment = _helperAssignments.TryGetValue(r.EntityId, out assignedOre);
        if(isHelperRole || (state.IsAst && hasAssignment)) {
            if(hasAssignment && GetPhysicalAllowed(r, O_ALL).Contains(assignedOre)) AddDistributionCount(assignedOre);
            continue;
        }

        var allowed = GetPhysicalAllowed(r, GetAllowed(r));
        foreach(var ore in allowed) {
            if(_oreLog.ContainsKey(ore) && _oreLog[ore] > 0) AddDistributionCount(ore);
        }
    }
}

void AddDistributionCount(string ore) {
    int count;
    if(_distributionCounts.TryGetValue(ore, out count)) _distributionCounts[ore] = count + 1;
    else _distributionCounts[ore] = 1;
}

void DoRefill(List<IMyRefinery> refineries, List<IMyTerminalBlock> sources, Dictionary<string, MyFixedPoint> oreBacklog) {
    for(int i=0; i<refineries.Count; i++) {
        if(BudgetLow()) return;
        var r = refineries[i];
        if (r.Closed) continue;
        RefineryState s; if (!_states.TryGetValue(r.EntityId, out s)) continue;
        if (GetSt(r) == "PURGING") continue;

        bool isHelper = GetRole(r) == "HELPER" || GetRole(r) == "GENERALIST";
        List<string> oresToPull;
        if(isHelper || s.IsAst) {
            _singleOreList.Clear();
            string assignedOre;
            if(_helperAssignments.TryGetValue(r.EntityId, out assignedOre)) {
                if(GetPhysicalAllowed(r, O_ALL).Contains(assignedOre)) _singleOreList.Add(assignedOre);
                else _helperAssignments.Remove(r.EntityId);
            }
            oresToPull = _singleOreList;
        } else {
            oresToPull = GetPhysicalAllowed(r, GetAllowed(r));
        }

        if (oresToPull.Count > 0) {
            Pull(r.InputInventory, oresToPull, sources, DEF_REFILL);
        }
    }
}

void UpdStates() {
    _tmpRefs.Clear(); GridTerminalSystem.GetBlocksOfType(_tmpRefs, r => r.IsSameConstructAs(Me));
    _tmpLongSet.Clear();
    foreach(var r in _tmpRefs) if(!r.CustomName.Contains("RESERVED") && !(r.CustomData ?? "").Contains(IGNORE_TAG)) _tmpLongSet.Add(r.EntityId);
    _tmpStrings.Clear();
    foreach(var kv in _states) if(!_tmpLongSet.Contains(kv.Key)) _tmpStrings.Add(kv.Key.ToString());
    foreach(var keyStr in _tmpStrings) {
        long key;
        if(long.TryParse(keyStr, out key)) {
            _states.Remove(key);
            _lastAct.Remove(key);
            _purgeT.Remove(key);
            _purgeEl.Remove(key);
            _helperAssignments.Remove(key);
            _replacementRoles.Remove(key);
        }
    }

    foreach(var r in _tmpRefs) {
        if(!_tmpLongSet.Contains(r.EntityId)) continue;
        RefineryState s;
        if(!_states.TryGetValue(r.EntityId, out s)) {
            s = new RefineryState { Id = r.EntityId, Name = r.CustomName, PrimOre = GetReadRole(GetRole(r)) };
            _states[r.EntityId] = s;
        } else s.Name = r.CustomName;
        RestoreProgress(r, s);
    }
}

void RestoreProgress(IMyRefinery r, RefineryState s) {
    if(r == null || s == null || s.JobStart <= 0) return;
    double k = GetInMass(r);
    if(k <= 0.001) { s.JobStart = 0; s.JobType = null; s.Prog = 0; s.LastIn = 0; return; }
    string t = GetFirstOre(r);
    if(s.JobType == null && t != null) s.JobType = t;
    if(t != null && s.JobType != null && t != s.JobType) return;
    if(k > s.JobStart) s.JobStart = k;
    s.LastIn = k;
    s.Prog = Math.Max(0, Math.Min(1, (s.JobStart - k) / s.JobStart));
}

string GetFirstOre(IMyRefinery r) {
    var items = GetItems(r.GetInventory(0));
    foreach(var it in items) if(it.Type.TypeId.EndsWith("Ore")) return it.Type.SubtypeId;
    return null;
}

void MonRef(IMyRefinery r, double dt) {
    RefineryState s; if(!_states.TryGetValue(r.EntityId, out s)) return;
    double k = GetInMass(r);
    var i = GetItems(r.GetInventory(0));
    string t = null;
    foreach(var it in i) if(it.Type.TypeId.EndsWith("Ore")) { t = it.Type.SubtypeId; break; }
    if(s.JobStart > 0 && k > 0 && (s.JobType == null || t == s.JobType) && s.LastIn == 0) {
        RestoreProgress(r, s);
    } else if(s.LastIn == 0 && k > 0 && t != null) {
        s.JobStart = k; s.JobType = t;
    } else if(k > s.LastIn && t == s.JobType) {
        s.JobStart += (k - s.LastIn);
    } else if(t != s.JobType || (k > 0 && s.JobStart == 0)) {
        s.JobStart = k; s.JobType = t;
    } else if(k < s.LastIn && t == s.JobType) {
        double d = s.LastIn - k;
        if(d > MAX_TICK) s.JobStart = Math.Max(k, s.JobStart - d);
    }
    if(k <= 0.001) {
        s.JobStart = 0; s.JobType = null; s.Prog = 0;
    } else if(s.JobStart > 0) s.Prog = Math.Max(0, Math.Min(1, (s.JobStart - k) / s.JobStart));
    else s.Prog = 0;
    s.LastIn = k;
    if(GetSt(r) == "PURGING") s.Status = "PURGING";
    else if(!r.Enabled) s.Status = "OFF";
    else if(!r.IsWorking) s.Status = "NO POWER";
    else if(k <= 0.0001 || !r.IsProducing) s.Status = "WAITING";
    else s.Status = "WORKING";
}

double GetInMass(IMyRefinery r) {
    var i = r.GetInventory(0); if(i == null) return 0;
    var l = GetItems(i); double t = 0;
    foreach(var x in l) if(x.Type.TypeId.EndsWith("Ore")) t += (double)x.Amount;
    return t;
}

void UpdRoles(List<IMyRefinery> r) {
    _activeRoles.Clear();
    _tmpRefs.Clear();
    foreach(var x in r) {
        if(BudgetLow()) return;
        if(IsBas(x)) {
            if(GetRole(x).ToUpper() != "GENERALIST") SetRole(x, "GENERALIST");
            _helperAssignments.Remove(x.EntityId);
            _replacementRoles.Remove(x.EntityId);
        } else _tmpRefs.Add(x);
    }
    int ordinaryCount = _tmpRefs.Count;
    for(int i = 0; i < ordinaryCount; i++) {
        if(BudgetLow()) return;
        var x = _tmpRefs[i];
        string wanted = GetOrdinaryRole(i, ordinaryCount);
        if(GetRole(x).ToUpper() != wanted) {
            if(x.InputInventory.CurrentVolume.RawValue == 0 && !x.IsProducing) SetRole(x, wanted);
            else _replacementRoles[x.EntityId] = wanted;
        }
        _activeRoles.Add(wanted);
    }
    foreach(var x in r) {
        string repl;
        if(_replacementRoles.TryGetValue(x.EntityId, out repl) && x.InputInventory.CurrentVolume.RawValue == 0 && !x.IsProducing) {
            SetRole(x, repl);
            _replacementRoles.Remove(x.EntityId);
        }
        RefineryState st; if(_states.TryGetValue(x.EntityId, out st)) st.PrimOre = GetReadRole(GetRole(x));
    }
}

string GetOrdinaryRole(int index, int total) {
    if(total <= 0) return "HELPER";
    if(total == 1) return index == 0 ? "GENERALIST" : "HELPER";
    if(total == 2) return index == 0 ? "GENERALIST" : index == 1 ? "BULK" : "HELPER";
    if(total < 10) {
        if(index == 0) return "BULK";
        if(index == 1) return "MID_TIER";
        if(index == 2) return "HIGH_TIER";
    } else {
        if(index == 0) return "GOLD";
        if(index == 1) return "PLATINUM";
        if(index == 2) return "URANIUM";
    }
    switch(index) {
        case 3: return "STONE";
        case 4: return "IRON";
        case 5: return "NICKEL";
        case 6: return "SILICON";
        case 7: return "COBALT";
        case 8: return "MAGNESIUM";
        case 9: return "SILVER";
    }
    return "HELPER";
}

void SafeOn(IMyRefinery r) {
    string cd = r.CustomData ?? "";
    if(cd.IndexOf(MANUAL_DISABLE_TAG, StringComparison.OrdinalIgnoreCase) < 0 &&
       cd.IndexOf(AUTO_DISABLED_TAG, StringComparison.OrdinalIgnoreCase) < 0 &&
       !r.Enabled) r.Enabled = true;
}

void DoNoOreShutdown(List<IMyRefinery> refs, int limit) {
    if(limit <= 0) return;
    int done = 0;
    int count = Math.Min(BATCH, limit);
    while(done < count && !BudgetLow()) {
        if(_noOreOffIdx >= limit) _noOreOffIdx = 0;
        var r = refs[_noOreOffIdx++];
        done++;
        if(r.Closed || r.IsProducing || GetSt(r) == "PURGING") continue;
        if((r.CustomData ?? "").IndexOf(MANUAL_DISABLE_TAG, StringComparison.OrdinalIgnoreCase) >= 0) continue;
        if(r.Enabled) r.Enabled = false;
    }
}

bool ChkAvail(List<string> o, Dictionary<string, MyFixedPoint> b) {
    foreach(var x in o) if(b.ContainsKey(x) && (double)b[x] > 0) return true;
    return false;
}

List<string> GetAllowed(IMyRefinery r) {
    string ro = GetRole(r);
    if(string.IsNullOrEmpty(ro) || ro == "GENERALIST" || ro == "HELPER") return O_ALL;
    switch(ro.ToUpper()) {
        case "BULK": return O_BSC; case "STONE": return O_STONE; case "IRON": return O_IRON;
        case "NICKEL": return O_NI; case "SILICON": return O_SI; case "COBALT": return O_CO;
        case "MAGNESIUM": return O_MG; case "SILVER": return O_AG; case "GOLD": return O_AU;
        case "PLATINUM": return O_PT; case "URANIUM": return O_U; case "MID_TIER": return O_MID;
        case "HIGH_TIER": return O_HIGH;
    }
    return O_ALL;
}

List<string> GetPhysicalAllowed(IMyRefinery r, List<string> roleAllowed) {
    if(!IsBas(r)) return roleAllowed;
    if(roleAllowed == O_ALL || roleAllowed == O_BSC) return O_BSC;
    _physicalOreList.Clear();
    foreach(var ore in roleAllowed) if(O_BSC.Contains(ore)) _physicalOreList.Add(ore);
    return _physicalOreList;
}

bool IsBas(IMyRefinery r) { return r.BlockDefinition.SubtypeId.Contains("Blast") || r.BlockDefinition.SubtypeId.Contains("Basic"); }

string GetReadRole(string r) {
    switch(r) {
        case "BULK": return "Bulk Basic"; case "MID_TIER": return "Cobalt/Ag"; case "HIGH_TIER": return "Gold/Uranium";
        case "STONE": return "Stone"; case "IRON": return "Iron"; case "NICKEL": return "Nickel";
        case "SILICON": return "Silicon"; case "COBALT": return "Cobalt"; case "MAGNESIUM": return "Magnesium";
        case "SILVER": return "Silver"; case "GOLD": return "Gold"; case "PLATINUM": return "Platinum";
        case "URANIUM": return "Uranium"; default: return null;
    }
}

void DoPurge(List<IMyRefinery> r, List<IMyCargoContainer> c, List<IMyRefinery> t) {
    double dt = Runtime.TimeSinceLastRun.TotalSeconds;
    foreach(var rf in r) {
        if(BudgetLow()) return;
        if(rf.Closed || GetSt(rf) != "PURGING") continue;
        rf.Enabled = false; rf.UseConveyorSystem = true;
        var inv = rf.GetInventory(0); var items = GetItems(inv);
        bool ok = true;
        for(int j = items.Count - 1; j >= 0; j--) {
            var it = items[j]; if(!it.Type.TypeId.EndsWith("Ore")) continue;
            if(!ToCargo(it, inv, c)) if(!ToRef(it, inv, t, rf)) ok = false;
        }
        if(!_purgeEl.ContainsKey(rf.EntityId)) _purgeEl[rf.EntityId] = 0.0;
        _purgeEl[rf.EntityId] += Math.Max(0, dt);
        if(ok || _purgeEl[rf.EntityId] > PURGE_TIME) {
            SetSt(rf, ""); SafeOn(rf); rf.UseConveyorSystem = false;
        }
    }
}

bool ToCargo(MyInventoryItem i, IMyInventory s, List<IMyCargoContainer> c) {
    foreach(var x in c) {
        if(BudgetLow()) return false;
        var t = x.GetInventory(0);
        if(t != null && t.CanItemsBeAdded(i.Amount, i.Type) && s.TransferItemTo(t, i)) return true;
    }
    return false;
}

bool ToRef(MyInventoryItem i, IMyInventory s, List<IMyRefinery> t, IMyRefinery src) {
    foreach(var x in t) {
        if(BudgetLow()) return false;
        if (x == src || x.Closed) continue;
        if(GetPhysicalAllowed(x, GetAllowed(x)).Contains(i.Type.SubtypeId)) {
            var inv = x.GetInventory(0);
            if(inv.CanItemsBeAdded(i.Amount, i.Type) && s.TransferItemTo(inv, i)) return true;
        }
    }
    return false;
}

// BATCH-CONTROL: FLOODGATE LOGICA VOOR MILJOENEN TONNEN ORE
void Pull(IMyInventory t, List<string> p, List<IMyTerminalBlock> s, double f) {
    RefineryState state; IMyTerminalBlock owner = (IMyTerminalBlock)t.Owner;
    if (!_states.TryGetValue(owner.EntityId, out state)) return;

    double currentVolumeRatio = (double)t.CurrentVolume / (double)t.MaxVolume;

    // Start vullen alleen als we nagenoeg leeg zijn (< 10% om sneller op te pakken)
    if (currentVolumeRatio < 0.10) state.IsFilling = true;

    // Lock erop bij 80%
    if (currentVolumeRatio >= f) { state.IsFilling = false; state.JobType = null; }

    if (!state.IsFilling) return;

    _invItems.Clear(); t.GetItems(_invItems);
    string currentOreInInventory = null;
    foreach(var item in _invItems) if(item.Type.TypeId.EndsWith("Ore")) { currentOreInInventory = item.Type.SubtypeId; break; }

    // Een eenmaal gestarte batch blijft volledig met rust tot de refinery leeg is.
    // Geen kleine top-ups en geen nieuwe taak tijdens het verwerken.
    if(currentOreInInventory != null) {
        state.IsFilling = false;
        return;
    }

    foreach(var y in p) {
        if(BudgetLow()) return;
        MyFixedPoint availableAmt;
        if(!_oreLog.TryGetValue(y, out availableAmt) || availableAmt <= 0) continue;

        int distributionCount = 1;
        _distributionCounts.TryGetValue(y, out distributionCount);
        if(distributionCount < 1) distributionCount = 1;

        double currentOreAmount = 0;
        foreach(var item in _invItems) {
            if(item.Type.TypeId.EndsWith("Ore") && item.Type.SubtypeId == y) currentOreAmount += (double)item.Amount;
        }

        // Verdeel eerst eerlijk over alle toegewezen refineries. Zo krijgt elke idle refinery werk.
        double fairQuota = (double)availableAmt / distributionCount;
        double remainingQuota = Math.Max(0, fairQuota - currentOreAmount);
        if(remainingQuota <= 0.001) continue;

        bool pulledSomething = false;
        foreach(var x in s) {
            if(BudgetLow()) return;
            if(((double)t.CurrentVolume / (double)t.MaxVolume) >= f) { state.IsFilling = false; return; }
            if(remainingQuota <= 0.001) { state.IsFilling = false; return; }
            if(x.EntityId == owner.EntityId) continue;
            // Zodra een refinery een batch heeft, wordt die batch niet meer uit andere refineries aangevuld.
            // Dit voorkomt ping-pong terwijl het totale resterende erts tijdens productie afneemt.
            RefineryState sourceState;
            bool sourceIsRefinery = _states.TryGetValue(x.EntityId, out sourceState);
            if(currentOreInInventory != null && sourceIsRefinery) continue;
            var si = x.GetInventory(0); if(si == null) continue;
            _pullItems.Clear(); si.GetItems(_pullItems);
            double sourceTransferLimit = double.MaxValue;
            if(sourceIsRefinery) {
                double sourceOreAmount = 0;
                foreach(var sourceItem in _pullItems) {
                    if(sourceItem.Type.TypeId.EndsWith("Ore") && sourceItem.Type.SubtypeId == y) sourceOreAmount += (double)sourceItem.Amount;
                }
                sourceTransferLimit = Math.Max(0, sourceOreAmount - fairQuota);
                if(sourceTransferLimit <= 0.001) continue;
            }
            for(int i = _pullItems.Count - 1; i >= 0; i--) {
                var m = _pullItems[i];
                if(m.Type.TypeId.EndsWith("Ore") && m.Type.SubtypeId == y) {
                    double freeVolume = Math.Max(0, (double)t.MaxVolume * f - (double)t.CurrentVolume);
                    double unitVolume = (double)m.Type.GetItemInfo().Volume;
                    double maxAmount = Math.Min(Math.Min((double)m.Amount, remainingQuota), sourceTransferLimit);
                    if(unitVolume > 0) maxAmount = Math.Min(maxAmount, freeVolume / unitVolume);
                    MyFixedPoint amount = (MyFixedPoint)maxAmount;
                    if(amount > 0 && t.TransferItemFrom(si, m, amount)) {
                        pulledSomething = true;
                        state.JobType = y;
                        remainingQuota -= (double)amount;
                        sourceTransferLimit -= (double)amount;
                    }
                    if(((double)t.CurrentVolume / (double)t.MaxVolume) >= f) { state.IsFilling = false; return; }
                    if(remainingQuota <= 0.001) { state.IsFilling = false; return; }
                }
            }
        }
        if (pulledSomething) break; 
    }
}

void ContinueOreScan(List<IMyTerminalBlock> sources) {
    while(_oreScanIdx < sources.Count && !BudgetLow()) {
        var block = sources[_oreScanIdx++];
        var inv = block.GetInventory(0);
        if(inv == null) continue;
        _pullItems.Clear();
        inv.GetItems(_pullItems);
        foreach(var item in _pullItems) if(item.Type.TypeId.EndsWith("Ore")) {
            MyFixedPoint amount;
            if(_oreScanBuild.TryGetValue(item.Type.SubtypeId, out amount)) _oreScanBuild[item.Type.SubtypeId] = amount + item.Amount;
            else _oreScanBuild[item.Type.SubtypeId] = item.Amount;
        }
    }
    if(_oreScanIdx >= sources.Count) {
        var old = _oreLog;
        _oreLog = _oreScanBuild;
        _oreScanBuild = old;
        _oreScanBuild.Clear();
        _oreScanIdx = 0;
    }
}

void ContinueIngotScan(List<IMyTerminalBlock> blocks) {
    while(_ingotScanBlockIdx < blocks.Count && !BudgetLow()) {
        var block = blocks[_ingotScanBlockIdx];
        while(_ingotScanInvIdx < block.InventoryCount && !BudgetLow()) {
            var inv = block.GetInventory(_ingotScanInvIdx++);
            if(inv == null) continue;
            _pullItems.Clear();
            inv.GetItems(_pullItems);
            foreach(var item in _pullItems) if(item.Type.TypeId.EndsWith("Ingot")) {
                double amount;
                if(_ingotScanBuild.TryGetValue(item.Type.SubtypeId, out amount)) _ingotScanBuild[item.Type.SubtypeId] = amount + (double)item.Amount;
                else _ingotScanBuild[item.Type.SubtypeId] = (double)item.Amount;
            }
        }
        if(_ingotScanInvIdx >= block.InventoryCount) {
            _ingotScanInvIdx = 0;
            _ingotScanBlockIdx++;
        }
    }
    if(_ingotScanBlockIdx >= blocks.Count) {
        var old = _ingotLog;
        _ingotLog = _ingotScanBuild;
        _ingotScanBuild = old;
        _ingotScanBuild.Clear();
        _ingotScanBlockIdx = 0;
        _ingotScanInvIdx = 0;
    }
}

List<MyInventoryItem> GetItems(IMyInventory i) {
    _invItems.Clear(); if(i != null) i.GetItems(_invItems); return _invItems;
}

string Fmt(double k) { return k < 1 ? string.Format("{0:N0} g", k * 1000) : k < 1000 ? string.Format("{0:N1} kg", k) : k < 1000000 ? string.Format("{0:N2} t", k / 1000) : string.Format("{0:N2} Mt", k / 1000000); }
string FmtPower(double mw) { return mw >= 1000 ? string.Format("{0:F2}GW", mw / 1000.0) : string.Format("{0:F1}MW", mw); }
void SetLanguage(string value) { string v = (value ?? "").Trim().ToUpper(); if(v == "BR" || v == "PT-BR") v = "PT"; _lang = LANGS.Contains(v) ? v : "EN"; }
int LangIdx() { int i = LANGS.IndexOf(_lang); return i >= 0 ? i : 0; }
string T(string key) { string[] value; int i = LangIdx(); return TXT.TryGetValue(key, out value) && i < value.Length ? value[i] : key; }
string OreName(string subtype) { string name; Dictionary<string,string> map; if(ORE_NAMES.TryGetValue(_lang, out map) && map.TryGetValue(subtype, out name)) return name; return ORE_ABBREVIATIONS.TryGetValue(subtype, out name) ? name : subtype; }
string RoleName(string role) { string name; Dictionary<string,string> map; if(ROLE_NAMES.TryGetValue(_lang, out map) && map.TryGetValue(role, out name)) return name; return ROLE_TO_NAME_MAP.TryGetValue(role, out name) ? name : role; }
string Fit(string text, int width) { if(text == null) text = ""; return text.Length > width ? text.Substring(0, width) : text.PadRight(width); }
bool IsWideLcd(IMyTextPanel p) {
    string subtype = (p.BlockDefinition.SubtypeId ?? "").ToLower();
    if(subtype.Contains("wide")) return true;
    return p.SurfaceSize.X > p.SurfaceSize.Y * 1.35f;
}

void DoLcd(int lim) {
    if(_actRefs.Count == 0) return;
    int c = 0, safe = 0; _dbgLcdDraw = 0;
    while(c < LCD_BATCH && safe < _actRefs.Count) {
        if(BudgetLow()) return;
        if(_lcdIdx >= _actRefs.Count) _lcdIdx = 0;
        if(_lcdIdx < lim) {
            var r = _actRefs[_lcdIdx];
            if(r != null && !r.Closed) {
                if(_lcdCache.ContainsKey(r.EntityId)) {
                    var s = _lcdCache[r.EntityId]; DrwInd(r, s); c++; _dbgLcdDraw += s.Count;
                }
            }
        }
        _lcdIdx++; safe++;
    }
}

void SetRole(IMyRefinery r, string x) {
    string c = r.CustomData ?? ""; var l = c.Split('\n'); _sb.Clear();
    foreach(var z in l) if(!z.StartsWith(ROLE_PREFIX) && !z.StartsWith("RefineryManager:Role=") && z.Length > 0) _sb.AppendLine(z);
    if(!string.IsNullOrEmpty(x)) _sb.AppendLine(ROLE_PREFIX + x);
    r.CustomData = _sb.ToString().TrimEnd('\n', '\r');
    if(_rename) RenRef(r, _actRefs);
}

void RenRef(IMyRefinery r, List<IMyRefinery> a) {
    string ro = GetRole(r); if(string.IsNullOrEmpty(ro)) return;
    string gn = r.CubeGrid.CustomName; string rn = "Refinery";
    if(ROLE_TO_NAME_MAP.ContainsKey(ro)) rn = ROLE_TO_NAME_MAP[ro];
    if(ro == "HELPER") {
        double rd = Vector3D.DistanceSquared(Me.GetPosition(), r.GetPosition());
        int idx = 1;
        for(int i = 0; i < a.Count; i++) {
            var x = a[i];
            if(x == r || x.Closed || GetRole(x) != "HELPER") continue;
            double xd = Vector3D.DistanceSquared(Me.GetPosition(), x.GetPosition());
            if(xd < rd || (Math.Abs(xd - rd) < 0.001 && x.EntityId < r.EntityId)) idx++;
        }
        rn += " " + idx.ToString("00");
    }
    string nn = !string.IsNullOrWhiteSpace(gn) ? (gn + " / " + rn) : rn;
    if(r.CustomName != nn) r.CustomName = nn;
}

string GetRole(IMyTerminalBlock b) {
    string c = b.CustomData ?? ""; var l = c.Split('\n');
    foreach(var x in l) {
        if(x.StartsWith(ROLE_PREFIX)) return x.Substring(ROLE_PREFIX.Length);
        if(x.StartsWith("RefineryManager:Role=")) return x.Substring(22);
    }
    return "";
}

void SetSt(IMyRefinery r, string s) {
    string c = r.CustomData ?? ""; var l = c.Split('\n'); _sb.Clear();
    foreach(var x in l) if(!x.StartsWith(STATE_PREFIX) && !x.StartsWith("RefineryManager:State=") && x.Length > 0) _sb.AppendLine(x);
    if(!string.IsNullOrEmpty(s)) {
        _sb.AppendLine(STATE_PREFIX + s);
        if(s == "PURGING") { _purgeT[r.EntityId] = DateTime.Now; _purgeEl[r.EntityId] = 0.0; }
    } else {
        if(_purgeT.ContainsKey(r.EntityId)) _purgeT.Remove(r.EntityId);
        if(_purgeEl.ContainsKey(r.EntityId)) _purgeEl.Remove(r.EntityId);
    }
    r.CustomData = _sb.ToString().TrimEnd('\n', '\r');
}

string GetSt(IMyRefinery r) {
    string c = r.CustomData ?? ""; var l = c.Split('\n');
    foreach(var x in l) {
        if(x.StartsWith(STATE_PREFIX)) return x.Substring(STATE_PREFIX.Length);
        if(x.StartsWith("RefineryManager:State=")) return x.Substring(22);
    }
    return "";
}

void ParseCD() {
    string c = Me.CustomData ?? ""; var l = c.Split('\n');
    foreach(var x in l) {
        string line = x.Trim();
        if(line.StartsWith("Language:", StringComparison.OrdinalIgnoreCase)) {
            var p = line.Split(':'); if(p.Length > 1) SetLanguage(p[1]);
        } else if(line.StartsWith("AutoRename:")) {
            var p = line.Split(':'); if(p.Length > 1) _rename = p[1].Trim().Equals("On", StringComparison.OrdinalIgnoreCase);
        } else if(line.StartsWith("IgnoreNewRefineries:")) {
            var p = line.Split(':'); if(p.Length > 1) _ignore = p[1].Trim().Equals("On", StringComparison.OrdinalIgnoreCase);
        } else if(line.StartsWith("Mode:")) {
            var p = line.Split(':');
            if(p.Length > 1) {
                int mode;
                if(TryParseMode(p[1], out mode)) _mode = mode;
            }
        }
    }
}

void UpdCD() {
    _sb.Clear();
    _sb.AppendLine("Language: " + _lang);
    _sb.AppendLine("Mode: " + GetMode(_mode));
    _sb.AppendLine("AutoRename: " + (_rename ? "On" : "Off"));
    _sb.AppendLine("IgnoreNewRefineries: " + (_ignore ? "On" : "Off"));
    Me.CustomData = _sb.ToString().TrimEnd('\n', '\r');
}

void InitCD() { string c = Me.CustomData ?? ""; ParseCD(); if(!c.Contains("Language:") || !c.Contains("Mode:") || !c.Contains("AutoRename:") || !c.Contains("IgnoreNewRefineries:")) UpdCD(); }
void AdvBar() { int w = 12 - 1; if(_barFwd) { _barPos++; if(_barPos >= w) { _barFwd = false; _barPos = w - 1; } } else { _barPos--; if(_barPos <= 0) { _barFwd = true; _barPos = 1; } } }
void UpdPB() { DrawPbSprite(Me.GetSurface(0)); }
string GetMode(int m) { return m == 1 ? "DISPLAY" : m == 2 ? "AI ONLY" : "HYBRID"; }
string GetModeName(int m) { return GetMode(m); }
bool TryParseMode(string value, out int mode) {
    string m = (value ?? "").Trim().ToUpper();
    mode = 0;
    if(m == "DISPLAY" || m == "SCHERM" || m == "ANZEIGE" || m == "ECRAN" || m == "PANTALLA" || m == "TELA" || m == "EKRAN" || m == "SCHERMO") { mode = 1; return true; }
    if(m == "AI ONLY" || m == "AI ALLEEN" || m == "NUR AI" || m == "IA SEULE" || m == "SOLO IA" || m == "SO IA" || m == "TOLKO AI" || m == "TYLKO AI" || m == "SADECE AI") { mode = 2; return true; }
    if(m == "HYBRID" || m == "HYBRIDE" || m == "HIBRIDO" || m == "GIBRID" || m == "HYBRYDA" || m == "HIBRIT" || m == "IBRIDO") { mode = 3; return true; }
    return false;
}
string GetModeDisplay(int m) {
    if(_lang == "NL") return m == 1 ? "SCHERM" : m == 2 ? "AI ALLEEN" : "HYBRIDE";
    if(_lang == "DE") return m == 1 ? "ANZEIGE" : m == 2 ? "NUR AI" : "HYBRID";
    if(_lang == "FR") return m == 1 ? "ECRAN" : m == 2 ? "IA SEULE" : "HYBRIDE";
    if(_lang == "ES") return m == 1 ? "PANTALLA" : m == 2 ? "SOLO IA" : "HIBRIDO";
    if(_lang == "PT") return m == 1 ? "TELA" : m == 2 ? "SO IA" : "HIBRIDO";
    if(_lang == "RU") return m == 1 ? "EKRAN" : m == 2 ? "TOLKO AI" : "GIBRID";
    if(_lang == "PL") return m == 1 ? "EKRAN" : m == 2 ? "TYLKO AI" : "HYBRYDA";
    if(_lang == "TR") return m == 1 ? "EKRAN" : m == 2 ? "SADECE AI" : "HIBRIT";
    if(_lang == "IT") return m == 1 ? "SCHERMO" : m == 2 ? "SOLO IA" : "IBRIDO";
    return GetMode(m);
}

double ParsMod(string d, string s) {
    var l = d.Split('\n');
    foreach(var x in l) {
        if(x.StartsWith(s)) {
            int p = x.IndexOf('%');
            if(p != -1) {
                int n = p; while(n > 0 && (char.IsDigit(x[n - 1]) || x[n - 1] == '.' || x[n - 1] == ',')) n--;
                double v; if(double.TryParse(x.Substring(n, p - n), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out v)) return v / 100.0;
            }
        }
    }
    return 1.0;
}

float GetPwr(string d) {
    var l = d.Split('\n');
    foreach(var x in l) if(x.StartsWith("Required Input:")) { var p = x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); float v; if(p.Length >= 3 && float.TryParse(p[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out v)) return p.Length >= 4 && p[3] == "MW" ? v : p.Length >= 4 && p[3] == "kW" ? v / 1000f : v / 1000000f; }
    return 0;
}

List<IMyTextPanel> DetCen(List<IMyTextPanel> l) {
    _lcdGroupWork.Clear();
    for(int i = 0; i < l.Count; i++) {
        var x = l[i];
        if((x.CustomData ?? "").Contains(LCD_CENTRAL)) _lcdGroupWork.Add(x);
    }
    if(_lcdGroupWork.Count == 0) {
        for(int i = 0; i < l.Count; i++) {
            var x = l[i];
            if(IsStandardFreeWideLcd(x) && !x.CustomName.ToLower().Contains("power")) {
                x.CustomData = LCD_CENTRAL;
                _lcdGroupWork.Add(x);
                break;
            }
        }
    }
    if(_lcdGroupWork.Count > 0) {
        _lcdGroupWork.Sort((x, y) => y.Position.Y.CompareTo(x.Position.Y));
        foreach(var x in _lcdGroupWork) {
            if(x.CustomName != "Central Refinery Display") x.CustomName = "Central Refinery Display";
            if(!(x.CustomData ?? "").Contains(LCD_CENTRAL)) x.CustomData += "\n" + LCD_CENTRAL;
        }
    }
    return _lcdGroupWork;
}

bool IsStandardFreeWideLcd(IMyTextPanel lcd) {
    if(lcd == null || !string.IsNullOrWhiteSpace(lcd.CustomData)) return false;
    string name = (lcd.CustomName ?? "").Trim();
    return name.Equals("Wide LCD Panel", StringComparison.OrdinalIgnoreCase) ||
           name.StartsWith("Wide LCD Panel ", StringComparison.OrdinalIgnoreCase);
}

bool IsStandardFreeOneByOneLcd(IMyTextPanel lcd) {
    if(lcd == null || !string.IsNullOrWhiteSpace(lcd.CustomData)) return false;
    string name = (lcd.CustomName ?? "").Trim();
    if(name.IndexOf("Wide", StringComparison.OrdinalIgnoreCase) >= 0) return false;
    return name.IndexOf("LCD Panel", StringComparison.OrdinalIgnoreCase) >= 0;
}

bool IsIndividualLcdCandidate(IMyTextPanel lcd) {
    if(lcd == null) return false;
    string data = lcd.CustomData ?? "";
    if(data.Contains(LCD_INDIVIDUAL)) return true;
    string name = lcd.CustomName ?? "";
    if(name.StartsWith("LCD - ", StringComparison.OrdinalIgnoreCase)) return true;
    return IsStandardFreeOneByOneLcd(lcd);
}

bool IsAdjacentToRefinery(IMyTextPanel lcd, IMyRefinery refinery) {
    if(lcd == null || refinery == null || lcd.CubeGrid != refinery.CubeGrid) return false;
    Vector3I p = lcd.Position, mn = refinery.Min, mx = refinery.Max;
    int dx = p.X < mn.X ? mn.X - p.X : p.X > mx.X ? p.X - mx.X : 0;
    int dy = p.Y < mn.Y ? mn.Y - p.Y : p.Y > mx.Y ? p.Y - mx.Y : 0;
    int dz = p.Z < mn.Z ? mn.Z - p.Z : p.Z > mx.Z ? p.Z - mx.Z : 0;
    return dx + dy + dz == 1;
}

void CacheLcd() {
    _lcdCache.Clear(); _lcdWork.Clear();
    GridTerminalSystem.GetBlocksOfType(_lcdWork, x => x.IsSameConstructAs(Me) && x.IsFunctional);
    _dbgLcdTotal = _lcdWork.Count; _dbgLcdCand = 0; _dbgLcdLinked = 0; _dbgLcdPanels = 0;
    var c = DetCen(_lcdWork);
    _cenLcds.Clear(); _cenLcds.AddRange(c);
    _lcdFreeWork.Clear();
    for(int i = 0; i < _lcdWork.Count; i++) if(!c.Contains(_lcdWork[i])) _lcdFreeWork.Add(_lcdWork[i]);
    _dbgLcdFree = _lcdFreeWork.Count;
    for(int i = 0; i < _lcdFreeWork.Count; i++) if(IsIndividualLcdCandidate(_lcdFreeWork[i])) _dbgLcdCand++;
    _lcdPosMap.Clear(); foreach(var x in _lcdFreeWork) _lcdPosMap[x.Position] = x;
    _lcdUnused.Clear(); foreach(var x in _lcdFreeWork) _lcdUnused.Add(x);
    foreach(var r in _actRefs) {
        if(r.Closed) continue;
        var s = new List<IMyTextPanel>();
        _lcdQueue.Clear(); _lcdVisited.Clear();
        foreach(var x in _lcdUnused) { if((x.CustomData ?? "").Contains(LCD_INDIVIDUAL) && IsAdjacentToRefinery(x, r)) { _lcdQueue.Enqueue(x); _lcdVisited.Add(x); } }
        foreach(var x in _lcdUnused) { if(!_lcdVisited.Contains(x) && IsIndividualLcdCandidate(x) && IsAdjacentToRefinery(x, r)) { _lcdQueue.Enqueue(x); _lcdVisited.Add(x); } }
        if(_lcdQueue.Count == 0) continue;
        while(_lcdQueue.Count > 0) {
            var cur = _lcdQueue.Dequeue(); s.Add(cur); IMyTextPanel n;
            if(_lcdPosMap.TryGetValue(cur.Position + Vector3I.Up, out n) && _lcdUnused.Contains(n) && !_lcdVisited.Contains(n) && IsIndividualLcdCandidate(n)) { _lcdQueue.Enqueue(n); _lcdVisited.Add(n); }
            if(_lcdPosMap.TryGetValue(cur.Position + Vector3I.Down, out n) && _lcdUnused.Contains(n) && !_lcdVisited.Contains(n) && IsIndividualLcdCandidate(n)) { _lcdQueue.Enqueue(n); _lcdVisited.Add(n); }
        }
        _lcdCache[r.EntityId] = s; _dbgLcdLinked++; _dbgLcdPanels += s.Count; foreach(var x in s) _lcdUnused.Remove(x);
    }
    foreach(var kv in _lcdCache) {
        var s = kv.Value; s.Sort((x, y) => y.Position.Y.CompareTo(x.Position.Y));
        IMyRefinery r = null; for(int i = 0; i < _actRefs.Count; i++) if(_actRefs[i].EntityId == kv.Key) { r = _actRefs[i]; break; }
        if(r == null) continue;
        for(int i = 0; i < s.Count; i++) {
            s[i].CustomData = LCD_INDIVIDUAL; string n = "LCD - " + r.CustomName + " (" + (i + 1) + "/" + s.Count + ")";
            if(s[i].CustomName != n) s[i].CustomName = n;
        }
    }
}

void DrwInd(IMyRefinery r, List<IMyTextPanel> s) {
    if(s.Count == 0) return;
    _sb.Clear(); var d = r.DetailedInfo;
    double p = GetPwr(d) * 1000, sm = ParsMod(d, "Refine Speed:"), ym = ParsMod(d, "Yield Rate:"), em = ParsMod(d, "Power Efficiency:");
    _invItems.Clear(); r.InputInventory.GetItems(_invItems); var inp = _invItems;
    MyInventoryItem? cur = null; _queuedOreItems.Clear();
    if(r.IsProducing && inp.Count > 0 && inp[0].Type.TypeId.EndsWith("Ore")) {
        cur = inp[0]; for (int i = 1; i < inp.Count; i++) _queuedOreItems.Add(inp[i]);
    } else for (int i = 0; i < inp.Count; i++) if (inp[i].Type.TypeId.EndsWith("Ore")) _queuedOreItems.Add(inp[i]); 
    string eh = T("ESTIMATED OUTPUT"); if(cur.HasValue) eh += " (" + OreName(cur.Value.Type.SubtypeId) + ")";
    _estOutput.Clear();
    for (int ii = 0; ii < inp.Count; ii++) {
        var it = inp[ii]; if (!it.Type.TypeId.EndsWith("Ore")) continue;
        Dictionary<string, double> rt;
        if (_conv.TryGetValue(it.Type.SubtypeId, out rt)) foreach(var kv in rt) {
                double a = (double)it.Amount * kv.Value * ym;
                if(_estOutput.ContainsKey(kv.Key)) _estOutput[kv.Key] += a; else _estOutput[kv.Key] = a;
            }
    }
    string role = GetRole(r), st = T("X WAITING"); Color col = new Color(139, 139, 0);
    RefineryState rs; _states.TryGetValue(r.EntityId, out rs);
    if(rs != null) RestoreProgress(r, rs);
    bool ast = rs != null && rs.IsAst;
    double of = (double)r.OutputInventory.CurrentVolume / (double)r.OutputInventory.MaxVolume;
    bool full = of > OUT_FULL;
    if (role == "HELPER") { st = T("X IDLE HELPER"); col = new Color(0, 150, 150); } else if (ast) { st = T("X IDLE ASSIST"); col = new Color(0, 0, 139); } else { st = T("X WAITING"); col = new Color(139, 139, 0); }
    if (r.IsProducing) {
        if (role == "HELPER") { st = T("> ACTIVE HELPER"); } else if (ast) { st = T("> ASSISTING"); col = new Color(0, 0, 139); } else { st = T("> ACTIVE"); col = new Color(0, 100, 0); }
    }
    if (_noOre) { st = T("OFF"); col = new Color(139, 0, 0); p = 0; } else if (rs != null && rs.IsSwitching) {
        st = T("> SWITCHING TO SPECIALIST"); col = new Color(139, 0, 139);
    } else if (GetSt(r) == "PURGING") {
        double rem = PURGE_TIME; if(_purgeEl.ContainsKey(r.EntityId)) rem = Math.Max(0, PURGE_TIME - _purgeEl[r.EntityId]);
        st = string.Format("> " + T("PURGING") + "... ({0:F0}s)", rem); col = new Color(139, 0, 139);
    } else if (!r.Enabled) { st = T("OFF"); col = new Color(139, 0, 0); } else if (!r.IsWorking) { st = T("NO POWER"); col = new Color(139, 0, 0); }
    if (full) col = new Color(139, 0, 0);
    string dn = r.CustomName;
    if (!string.IsNullOrEmpty(role) && ROLE_TO_NAME_MAP.ContainsKey(role)) {
        dn = RoleName(role); if(role == "HELPER") dn = r.CustomName.Substring(r.CustomName.LastIndexOf('/') + 1).Trim();
    }
    double ifill = (double)r.InputInventory.CurrentVolume / (double)r.InputInventory.MaxVolume;
    if (_estOutput.Count > 0) {
        _sortedEstimate.Clear();
        foreach(var kv in _estOutput) _sortedEstimate.Add(kv);
        _sortedEstimate.Sort((a, b) => b.Value.CompareTo(a.Value));
    }
    _invItems.Clear(); r.OutputInventory.GetItems(_invItems); _outputItems.Clear();
    for(int i=0;i<_invItems.Count;i++) if(!_invItems[i].Type.TypeId.EndsWith("Ore")) _outputItems.Add(_invItems[i]);
    if (_outputItems.Count > 0) _outputItems.Sort((a, b) => ((double)b.Amount).CompareTo((double)a.Amount));
    DrawIndividualSprites(s, dn, role, st, col, p, sm, ym, em, rs, cur, ifill, of, full);
}

Dictionary<string, OreProg> GetProg(List<IMyRefinery> r, Dictionary<string, MyFixedPoint> b) {
    _oreProgress.Clear();
    foreach(var k in b) {
        if(k.Key == "Ice") continue;
        string o = k.Key; double v = (double)k.Value;
        if(!_baseOre.ContainsKey(o) || v > _baseOre[o] || v <= 10.0) _baseOre[o] = v; 
        _oreProgress[o] = new OreProg { Tot = _baseOre[o], Rem = v };
    }
    return _oreProgress;
}

void UpdCen(List<IMyRefinery> r, List<IMyTextPanel> c, Dictionary<string, OreProg> p, Dictionary<string, MyFixedPoint> b, Dictionary<string, double> ing) {
    if(c.Count == 0) return;
    int tr = r.Count, ar = 0; double tp = 0;
    foreach(var x in r) {
        if(BudgetLow()) return;
        if(x.IsProducing) ar++;
        tp += GetPwr(x.DetailedInfo);
    }
    int ir = tr - ar;
    _sortedOreStock.Clear(); foreach(var kv in b) if(kv.Key != "Ice") _sortedOreStock.Add(kv); _sortedOreStock.Sort((a, z) => ((double)z.Value).CompareTo((double)a.Value));
    _activeOreCounts.Clear();
    foreach(var x in r) {
        if(BudgetLow()) return;
        if(x.IsProducing) { var i = GetItems(x.InputInventory); if(i.Count > 0 && i[0].Type.TypeId.EndsWith("Ore")) { string o = i[0].Type.SubtypeId; if(_activeOreCounts.ContainsKey(o)) _activeOreCounts[o]++; else _activeOreCounts[o] = 1; } }
    }
    _sortedIngots.Clear(); foreach(var kv in ing) _sortedIngots.Add(kv); _sortedIngots.Sort((a, z) => z.Value.CompareTo(a.Value));
    DrawCentralSprites(c, p, tr, ar, ir, tp);
}

void DrawPbSprite(IMyTextSurface lcd) {
    lcd.ContentType = ContentType.SCRIPT;
    lcd.ScriptBackgroundColor = Color.Black;
    lcd.ScriptForegroundColor = new Color(179, 237, 255);
    float vx, vy, vw, vh; GetViewport(lcd, out vx, out vy, out vw, out vh);
    float pad = Math.Min(vw, vh) * 0.018f;
    float x1 = vx + pad, y1 = vy + pad, x2 = vx + vw - pad, y2 = vy + vh - pad;
    float w = x2 - x1, h = y2 - y1, fs = Math.Max(0.25f, Math.Min(vw * 0.00155f, vh * 0.0042f));
    var frame = lcd.DrawFrame();
    Color fg = new Color(232, 251, 255), cyan = new Color(101, 245, 255), dim = new Color(115, 145, 155);
    Color green = new Color(88, 255, 139), yellow = new Color(255, 222, 117);
    AddBox(frame, vx, vy, vw, vh, Color.Black);
    AddFrame(frame, x1, y1, w, h, cyan, Math.Max(1f, pad * 0.16f));
    AddText(frame, "REFINERY MANAGER", vx + vw * 0.50f, y1 + h * 0.08f, fg, fs * 1.28f, TextAlignment.CENTER);
    AddText(frame, "VERSION " + VERSION, vx + vw * 0.50f, y1 + h * 0.19f, cyan, fs * 0.86f, TextAlignment.CENTER);
    AddLine(frame, x1 + w * 0.05f, y1 + h * 0.28f, x2 - w * 0.05f, y1 + h * 0.28f, cyan, Math.Max(1f, pad * 0.10f));
    AddText(frame, T("Mode"), x1 + w * 0.08f, y1 + h * 0.39f, dim, fs * 0.82f, TextAlignment.LEFT);
    AddText(frame, GetModeDisplay(_mode), x2 - w * 0.08f, y1 + h * 0.39f, fg, fs * 0.82f, TextAlignment.RIGHT);
    AddText(frame, T("Auto Rename"), x1 + w * 0.08f, y1 + h * 0.51f, dim, fs * 0.82f, TextAlignment.LEFT);
    AddText(frame, _rename ? T("ON") : T("OFF"), x2 - w * 0.08f, y1 + h * 0.51f, _rename ? green : yellow, fs * 0.82f, TextAlignment.RIGHT);
    AddText(frame, T("Ignore New"), x1 + w * 0.08f, y1 + h * 0.63f, dim, fs * 0.82f, TextAlignment.LEFT);
    AddText(frame, _ignore ? T("ON") : T("OFF"), x2 - w * 0.08f, y1 + h * 0.63f, _ignore ? green : yellow, fs * 0.82f, TextAlignment.RIGHT);
    AddText(frame, "REFINERIES", x1 + w * 0.08f, y1 + h * 0.77f, dim, fs * 0.72f, TextAlignment.LEFT);
    AddText(frame, _actRefs.Count.ToString(), x1 + w * 0.48f, y1 + h * 0.77f, fg, fs * 0.82f, TextAlignment.RIGHT);
    AddText(frame, "LANG", x1 + w * 0.58f, y1 + h * 0.77f, dim, fs * 0.72f, TextAlignment.LEFT);
    AddText(frame, _lang, x2 - w * 0.08f, y1 + h * 0.77f, fg, fs * 0.82f, TextAlignment.RIGHT);
    AddText(frame, "RUN", vx + vw * 0.50f, y1 + h * 0.89f, green, fs * 0.88f, TextAlignment.CENTER);
    AddProgressBar(frame, x1 + w * 0.12f, y1 + h * 0.94f, w * 0.76f, Math.Max(8f, h * 0.055f), (_barPos + 1) / 12.0, new Color(18, 28, 30), cyan);
    frame.Dispose();
}

void DrawIndividualSprites(List<IMyTextPanel> panels, string title, string role, string status, Color statusColor, double powerKw, double speedMod, double yieldMod, double powerMod, RefineryState state, MyInventoryItem? currentOre, double inputFill, double outputFill, bool outputFull) {
    if(panels.Count == 0) return;
    for(int pi = 0; pi < panels.Count; pi++) {
        var lcd = panels[pi];
        SetupSpriteSurface(lcd);
        float vx, vy, vw, vh; GetViewport(lcd, out vx, out vy, out vw, out vh);
        float pad = Math.Min(vw, vh) * 0.018f;
        float x1 = vx + pad, y1 = vy + pad, x2 = vx + vw - pad, y2 = vy + vh - pad;
        float w = x2 - x1, h = y2 - y1, fs = Math.Max(0.25f, Math.Min(vw * 0.00150f, vh * 0.00150f));
        float frameLine = Math.Max(1f, pad * 0.14f);
        float innerL = x1 + w * 0.065f, innerR = x2 - w * 0.055f, innerW = innerR - innerL;
        float lx = innerL + w * 0.025f, bx = innerL + w * 0.330f, vxr = innerR - w * 0.015f, bw = innerW * 0.47f;
        var frame = lcd.DrawFrame();
        Color fg = new Color(232, 251, 255), cyan = new Color(101, 245, 255), dim = new Color(126, 158, 166);
        Color line = outputFull ? Color.Red : statusColor, bg = Color.Black, barBg = new Color(15, 27, 30), panelBg = new Color(6, 15, 18);
        AddBox(frame, vx, vy, vw, vh, bg);
        AddFrame(frame, x1, y1, w, h, line, frameLine);
        if(outputFull && _warn) {
            AddText(frame, "!! " + T("WARNING") + " !!", vx + vw * 0.5f, y1 + h * 0.22f, Color.Red, fs * 1.55f, TextAlignment.CENTER);
            AddText(frame, T("OUTPUT FULL"), vx + vw * 0.5f, y1 + h * 0.38f, Color.Red, fs * 1.70f, TextAlignment.CENTER);
            AddProgressBar(frame, x1 + w * 0.12f, y1 + h * 0.52f, w * 0.76f, h * 0.07f, outputFill, barBg, Color.Red);
            AddText(frame, T("OUTPUT") + " " + string.Format("{0:F0}%", outputFill * 100), vx + vw * 0.5f, y1 + h * 0.66f, fg, fs, TextAlignment.CENTER);
            frame.Dispose();
            continue;
        }
        AddText(frame, Fit(title, 18), lx, y1 + h * 0.060f, fg, fs * 0.98f, TextAlignment.LEFT);
        AddText(frame, Fit(status.Replace(">", "").Trim(), 14), lx, y1 + h * 0.105f, statusColor, fs * 0.62f, TextAlignment.LEFT);
        AddLine(frame, innerL, y1 + h * 0.148f, innerR, y1 + h * 0.148f, cyan, Math.Max(1f, pad * 0.08f));
        AddBox(frame, innerL, y1 + h * 0.180f, innerW, h * 0.270f, panelBg);
        AddText(frame, T("Role"), lx, y1 + h * 0.225f, dim, fs * 0.66f, TextAlignment.LEFT);
        AddText(frame, Fit(RoleName(role), 17).TrimEnd(), vxr, y1 + h * 0.225f, fg, fs * 0.68f, TextAlignment.RIGHT);
        AddText(frame, T("Power"), lx, y1 + h * 0.285f, dim, fs * 0.66f, TextAlignment.LEFT);
        AddText(frame, string.Format("{0:N0} kW", powerKw), vxr, y1 + h * 0.285f, cyan, fs * 0.70f, TextAlignment.RIGHT);
        AddText(frame, T("Modules"), lx, y1 + h * 0.345f, dim, fs * 0.66f, TextAlignment.LEFT);
        AddText(frame, string.Format("S:{0:F0}%  Y:{1:F0}%  E:{2:F0}%", speedMod * 100, yieldMod * 100, powerMod * 100), vxr, y1 + h * 0.345f, fg, fs * 0.56f, TextAlignment.RIGHT);
        double progress = state != null ? state.Prog : 0;
        AddText(frame, T("Progression"), lx, y1 + h * 0.405f, dim, fs * 0.64f, TextAlignment.LEFT);
        AddProgressBar(frame, bx, y1 + h * 0.405f, bw, h * 0.030f, progress, barBg, cyan);
        AddText(frame, state != null && state.JobStart > 0 ? string.Format("{0:F0}%", progress * 100) : T("N/A"), vxr, y1 + h * 0.398f, fg, fs * 0.62f, TextAlignment.RIGHT);
        AddBox(frame, innerL, y1 + h * 0.475f, innerW, h * 0.215f, panelBg);
        AddText(frame, T("INPUT"), lx, y1 + h * 0.515f, fg, fs * 0.72f, TextAlignment.LEFT);
        AddProgressBar(frame, bx, y1 + h * 0.515f, bw, h * 0.038f, inputFill, barBg, line);
        AddText(frame, string.Format("{0:F0}%", inputFill * 100), vxr, y1 + h * 0.508f, fg, fs * 0.62f, TextAlignment.RIGHT);
        string now = currentOre.HasValue ? OreName(currentOre.Value.Type.SubtypeId) + " " + Fmt((double)currentOre.Value.Amount) : "(" + T("Empty") + ")";
        AddText(frame, T("Now") + ": " + Fit(now, 20), lx, y1 + h * 0.585f, fg, fs * 0.62f, TextAlignment.LEFT);
        int qMax = Math.Min(3, _queuedOreItems.Count);
        for(int qi = 0; qi < qMax; qi++) AddText(frame, Fit(OreName(_queuedOreItems[qi].Type.SubtypeId), 11) + " " + Fmt((double)_queuedOreItems[qi].Amount), lx + w * 0.04f, y1 + h * (0.635f + qi * 0.038f), dim, fs * 0.52f, TextAlignment.LEFT);
        AddBox(frame, innerL, y1 + h * 0.735f, innerW, h * 0.210f, panelBg);
        AddText(frame, T("OUTPUT"), lx, y1 + h * 0.780f, fg, fs * 0.72f, TextAlignment.LEFT);
        AddProgressBar(frame, bx, y1 + h * 0.780f, bw, h * 0.038f, outputFill, barBg, outputFull ? Color.Red : cyan);
        AddText(frame, string.Format("{0:F0}%", outputFill * 100), vxr, y1 + h * 0.773f, outputFull ? Color.Red : fg, fs * 0.62f, TextAlignment.RIGHT);
        int oMax = Math.Min(2, _outputItems.Count);
        for(int oi = 0; oi < oMax; oi++) {
            string nm = OreName(_outputItems[oi].Type.SubtypeId == "Stone" ? "Gravel" : _outputItems[oi].Type.SubtypeId);
            AddText(frame, Fit(nm, 11), lx + w * 0.04f, y1 + h * (0.850f + oi * 0.040f), dim, fs * 0.54f, TextAlignment.LEFT);
            AddText(frame, Fmt((double)_outputItems[oi].Amount), bx + w * 0.20f, y1 + h * (0.850f + oi * 0.040f), dim, fs * 0.54f, TextAlignment.RIGHT);
        }
        frame.Dispose();
    }
}

void DrawCentralSprites(List<IMyTextPanel> panels, Dictionary<string, OreProg> prog, int total, int active, int idle, double powerMw) {
    int specialists = CountSpecialists(), helpers = CountHelpers(), assists = CountAssists();
    int waiting = 0, purging = 0, noPower = 0, outputFull = 0, manual = 0;
    for(int ri = 0; ri < _actRefs.Count; ri++) {
        var rf = _actRefs[ri]; if(rf == null || rf.Closed) continue;
        if((rf.CustomData ?? "").IndexOf(MANUAL_DISABLE_TAG, StringComparison.OrdinalIgnoreCase) >= 0) manual++;
        if(GetSt(rf) == "PURGING") purging++;
        if(rf.Enabled && !rf.IsWorking) noPower++;
        if(!rf.IsProducing && rf.Enabled) waiting++;
        double of = (double)rf.OutputInventory.CurrentVolume / (double)rf.OutputInventory.MaxVolume;
        if(of > OUT_FULL) outputFull++;
    }
    for(int i = 0; i < panels.Count; i++) {
        var lcd = panels[i]; SetupSpriteSurface(lcd);
        float vx, vy, vw, vh; GetViewport(lcd, out vx, out vy, out vw, out vh);
        float sx = vw / 2048f, sy = vh / 1024f, sc = Math.Min(sx, sy);
        float ox = vx + (vw - 2048f * sc) * 0.5f, oy = vy + (vh - 1024f * sc) * 0.5f;
        var frame = lcd.DrawFrame();
        Color fg = new Color(232, 251, 255), dim = new Color(155, 186, 195), line = new Color(217, 248, 255), bg = Color.Black, barBg = new Color(17, 26, 28);
        Color cyan = new Color(101, 245, 255), green = new Color(88, 255, 139), yellow = new Color(255, 222, 117), mag = new Color(255, 140, 255);
        bool hasOreRows = _sortedOreStock.Count > 0;
        AddBox(frame, ox + 18f * sc, oy + 18f * sc, 2012f * sc, 988f * sc, bg);
        AddFrame(frame, ox + 18f * sc, oy + 18f * sc, 2012f * sc, 988f * sc, line, 4f * sc);
        AddText(frame, "[ " + T("CENTRAL OVERVIEW") + " ]", ox + 1024f * sc, oy + 48f * sc, fg, 1.40f * sc, TextAlignment.CENTER);
        AddText(frame, DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), ox + 1024f * sc, oy + 93f * sc, fg, 0.98f * sc, TextAlignment.CENTER);
        AddText(frame, string.Format("{0} {1} | {2} {3} | {4} {5} | {6}", total, T("Total"), active, T("Active"), idle, T("Idle"), FmtPower(powerMw)), ox + 1024f * sc, oy + 132f * sc, fg, 1.02f * sc, TextAlignment.CENTER);
        AddText(frame, string.Format("{0}: {1} | Specialists: {2}/10 | Helpers: {3} | Assist: {4}", T("Mode"), GetModeDisplay(_mode), specialists, helpers, assists), ox + 1024f * sc, oy + 170f * sc, dim, 0.72f * sc, TextAlignment.CENTER);
        AddLine(frame, ox + 80f * sc, oy + 205f * sc, ox + 1968f * sc, oy + 205f * sc, line, 3f * sc);
        AddText(frame, "ORE        REFS  PROGRESS                 STOCK       STATUS", ox + 105f * sc, oy + 232f * sc, dim, 0.72f * sc, TextAlignment.LEFT);
        for(int n = 0; n < _sortedOreStock.Count && n < 10; n++) {
            var kv = _sortedOreStock[n]; double pct = 0;
            OreProg d; if(prog.TryGetValue(kv.Key, out d) && d.Tot > 0) pct = 1.0 - (d.Rem / d.Tot);
            pct = Clamp01(pct);
            float x = ox + 105f * sc, y = oy + (280f + n * 54f) * sc;
            int cnt = _activeOreCounts.ContainsKey(kv.Key) ? _activeOreCounts[kv.Key] : 0;
            AddText(frame, Fit(OreName(kv.Key), 10), x, y, fg, 0.83f * sc, TextAlignment.LEFT);
            AddText(frame, cnt.ToString().PadLeft(3), x + 195f * sc, y, cyan, 0.82f * sc, TextAlignment.LEFT);
            AddProgressBar(frame, x + 275f * sc, y + 7f * sc, 360f * sc, 24f * sc, pct, barBg, line);
            AddText(frame, string.Format("{0,3:F0}%", pct * 100), x + 650f * sc, y + 1f * sc, dim, 0.75f * sc, TextAlignment.LEFT);
            AddText(frame, Fmt((double)kv.Value).PadLeft(9), x + 735f * sc, y, fg, 0.82f * sc, TextAlignment.LEFT);
            string oreStatus = cnt > 0 ? T("Active").ToUpper() : T("Queue").ToUpper();
            AddText(frame, oreStatus, x + 900f * sc, y, cnt > 0 ? green : yellow, 0.72f * sc, TextAlignment.LEFT);
        }
        if(!hasOreRows) AddText(frame, T("No ore").ToUpper() + " - refinery fleet standing by", ox + 580f * sc, oy + 445f * sc, dim, 0.92f * sc, TextAlignment.CENTER);
        AddFrame(frame, ox + 1220f * sc, oy + 225f * sc, 710f * sc, 400f * sc, line, 3f * sc);
        AddText(frame, "REFINERY STATUS", ox + 1250f * sc, oy + 248f * sc, fg, 0.96f * sc, TextAlignment.LEFT);
        AddStatusLine(frame, "Specialists", specialists + " / 10", ox, oy, 1260f, 305f, fg, green, sc);
        AddStatusLine(frame, "Helpers", helpers.ToString(), ox, oy, 1260f, 350f, fg, cyan, sc);
        AddStatusLine(frame, "Assists", assists.ToString(), ox, oy, 1260f, 395f, fg, yellow, sc);
        AddStatusLine(frame, "Waiting", waiting.ToString(), ox, oy, 1260f, 440f, fg, waiting > 0 ? yellow : green, sc);
        AddStatusLine(frame, "Purging", purging.ToString(), ox, oy, 1260f, 485f, fg, purging > 0 ? mag : green, sc);
        AddStatusLine(frame, "No power", noPower.ToString(), ox, oy, 1260f, 530f, fg, noPower > 0 ? Color.Red : green, sc);
        AddStatusLine(frame, "Output full", outputFull.ToString(), ox, oy, 1260f, 575f, fg, outputFull > 0 ? Color.Red : green, sc);
        AddFrame(frame, ox + 1220f * sc, oy + 650f * sc, 710f * sc, 205f * sc, line, 3f * sc);
        AddText(frame, "ENERGY STATUS", ox + 1250f * sc, oy + 672f * sc, fg, 0.90f * sc, TextAlignment.LEFT);
        AddText(frame, "Current draw", ox + 1260f * sc, oy + 720f * sc, fg, 0.76f * sc, TextAlignment.LEFT);
        AddText(frame, FmtPower(powerMw), ox + 1645f * sc, oy + 720f * sc, cyan, 0.76f * sc, TextAlignment.LEFT);
        AddText(frame, "Per active", ox + 1260f * sc, oy + 762f * sc, fg, 0.76f * sc, TextAlignment.LEFT);
        AddText(frame, active > 0 ? FmtPower(powerMw / active) : "0.0MW", ox + 1645f * sc, oy + 762f * sc, cyan, 0.76f * sc, TextAlignment.LEFT);
        AddText(frame, "Manual locked", ox + 1260f * sc, oy + 804f * sc, fg, 0.76f * sc, TextAlignment.LEFT);
        AddText(frame, manual.ToString(), ox + 1645f * sc, oy + 804f * sc, manual > 0 ? yellow : green, 0.76f * sc, TextAlignment.LEFT);
        AddText(frame, ". " + T("SYSTEM INGOTS") + " .", ox + 105f * sc, oy + 830f * sc, fg, 0.86f * sc, TextAlignment.LEFT);
        for(int n = 0; n < _sortedIngots.Count && n < 6; n++) {
            int col = n < 3 ? 0 : 1, row = n % 3;
            float x = ox + (105f + col * 520f) * sc, y = oy + (870f + row * 30f) * sc;
            AddText(frame, Fit(OreName(_sortedIngots[n].Key), 10) + " " + Fmt(_sortedIngots[n].Value).PadLeft(10), x, y, fg, 0.58f * sc, TextAlignment.LEFT);
        }
        frame.Dispose();
    }
}

void SetupSpriteSurface(IMyTextPanel lcd) {
    lcd.ContentType = ContentType.SCRIPT;
    lcd.ScriptBackgroundColor = Color.Black;
    lcd.ScriptForegroundColor = new Color(179, 237, 255);
}

void GetViewport(IMyTextSurface lcd, out float x, out float y, out float w, out float h) {
    Vector2 s = lcd.SurfaceSize, t = lcd.TextureSize;
    x = (t.X - s.X) * 0.5f; y = (t.Y - s.Y) * 0.5f; w = s.X; h = s.Y;
}

void AddText(MySpriteDrawFrame frame, string text, float x, float y, Color color, float scale, TextAlignment align) {
    var s = MySprite.CreateText(text, "Monospace", color, scale, align);
    s.Position = new Vector2(x, y);
    frame.Add(s);
}

void AddBox(MySpriteDrawFrame frame, float x, float y, float w, float h, Color color) {
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(x + w / 2f, y + h / 2f), new Vector2(w, h), color));
}

void AddFrame(MySpriteDrawFrame frame, float x, float y, float w, float h, Color color, float t) {
    AddBox(frame, x, y, w, t, color); AddBox(frame, x, y + h - t, w, t, color);
    AddBox(frame, x, y, t, h, color); AddBox(frame, x + w - t, y, t, h, color);
}

void AddLine(MySpriteDrawFrame frame, float x1, float y1, float x2, float y2, Color color, float t) {
    float w = Math.Abs(x2 - x1), h = Math.Abs(y2 - y1);
    if(w >= h) AddBox(frame, Math.Min(x1, x2), y1 - t / 2f, w, t, color);
    else AddBox(frame, x1 - t / 2f, Math.Min(y1, y2), t, h, color);
}

void AddProgressBar(MySpriteDrawFrame frame, float x, float y, float w, float h, double pct, Color bg, Color border) {
    pct = Clamp01(pct);
    AddBox(frame, x, y, w, h, bg);
    AddFrame(frame, x, y, w, h, border, Math.Max(1f, h * 0.08f));
    int seg = 28; float gap = 1f, inner = w - 6f, sw = (inner - (seg - 1) * gap) / seg;
    for(int i = 0; i < seg; i++) {
        double t = (i + 1) / (double)seg;
        if(t <= pct) AddBox(frame, x + 3f + i * (sw + gap), y + 3f, sw, h - 6f, ProgressColor(t));
    }
}

Color ProgressColor(double pct) {
    pct = Clamp01(pct);
    Color a, b; double t;
    if(pct < 0.33) { a = new Color(255, 64, 64); b = new Color(255, 158, 44); t = pct / 0.33; }
    else if(pct < 0.66) { a = new Color(255, 158, 44); b = new Color(255, 233, 92); t = (pct - 0.33) / 0.33; }
    else { a = new Color(255, 233, 92); b = new Color(66, 255, 122); t = (pct - 0.66) / 0.34; }
    return MixColor(a, b, t);
}

double Clamp01(double v) { if(double.IsNaN(v) || double.IsInfinity(v)) return 0; return Math.Max(0, Math.Min(1, v)); }

Color MixColor(Color a, Color b, double t) {
    t = Clamp01(t);
    return new Color((int)(a.R + (b.R - a.R) * t), (int)(a.G + (b.G - a.G) * t), (int)(a.B + (b.B - a.B) * t));
}

void AddStatusLine(MySpriteDrawFrame frame, string label, string value, float ox, float oy, float x, float y, Color labelColor, Color valueColor, float sc) {
    AddText(frame, label, ox + x * sc, oy + y * sc, labelColor, 0.82f * sc, TextAlignment.LEFT);
    AddText(frame, value, ox + (x + 385f) * sc, oy + y * sc, valueColor, 0.82f * sc, TextAlignment.LEFT);
}

int CountSpecialists() {
    int c = 0; for(int i = 0; i < _actRefs.Count; i++) { string r = GetRole(_actRefs[i]); if(_specialistRoles.Contains(r)) c++; }
    return c;
}

int CountHelpers() {
    int c = 0; for(int i = 0; i < _actRefs.Count; i++) if(GetRole(_actRefs[i]) == "HELPER") c++;
    return c;
}

int CountAssists() {
    int c = 0;
    for(int i = 0; i < _actRefs.Count; i++) {
        var r = _actRefs[i];
        if(r == null || r.Closed || !r.IsProducing) continue;
        RefineryState s;
        if(!_states.TryGetValue(r.EntityId, out s)) continue;
        if(s.IsAst) c++;
    }
    return c;
}
