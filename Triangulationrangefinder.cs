//v1.0.1, 2019-05-14, (c) Andrew
//Все настройки производятся в Свои данные. При необходимочти измените язык и перекомпилируйте скрипт.
//All settings are made in Custom Data. At first, change language, then recompile script.

private string CameraName = "Камера";
private string DisplayName = "Кокпит(0)";
private string InputCustomDataBlockName = "Кокпит";

IMyCameraBlock Camera;
IMyTextSurface TextSurface;
IMyTerminalBlock InputCustomDataBlock;

Triangulation Triang = new Triangulation();

private string[] curLang;

private bool initedFlag = false;
private int initTicksCounter = 0;
private int initTicksWait;

public Program()
{
        
    Random rnd = new Random();
    initTicksWait = 6 + rnd.Next(6);
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

}
private bool ModuleInited()
{
    if (initedFlag) return true;
    if (initTicksCounter < initTicksWait) { initTicksCounter++; return false; }
    InitModule();
    initedFlag = true;

    return true;
}
private void InitModule()
{

    StringBuilder sb = new StringBuilder();

    bool ready = true;
    try
    {
        bool firstLaunch;
        if (!ReadConfig(out firstLaunch))
        {
            if (firstLaunch)
            {
                sb.AppendLine(curLang[msgNewCfg]);
            }
            else
            {
                sb.AppendLine(curLang[msgBrokenCfg]);
            }
        }

        TextSurface = FindDisplay(DisplayName, GridTerminalSystem, Me);
        if (TextSurface == null) { sb.AppendLine(curLang[msgDisplayNotFound]); ready = false; }

        Camera = FindBlockOfType<IMyCameraBlock>(CameraName, GridTerminalSystem, Me);
        if (Camera == null) {sb.AppendLine(curLang[msgCameraNotfound]); ready = false; };

        InputCustomDataBlock = FindBlockOfType<IMyTerminalBlock>(InputCustomDataBlockName, GridTerminalSystem, Me);

        TextSurface.ContentType = ContentType.TEXT_AND_IMAGE;

        sb.AppendLine(curLang[msgInitComplete]);
    }
    catch
    {
        sb.AppendLine(curLang[msgInitError]);
        ready = false;
    }
    Echo(sb.ToString());

    if (ready)
    {
        TextSurface.WriteText("=== " + curLang[msgRangefinder] + " ===");
        TextSurface.WriteText("\n" + curLang[msgReady], true);
    }

    Runtime.UpdateFrequency = UpdateFrequency.None;

}

public void Main(string argument, UpdateType updateSource)
{

    if (!ModuleInited()) return;

    Runtime.UpdateFrequency = UpdateFrequency.None;
    if (string.IsNullOrEmpty(argument)) return;

    switch (argument)
    {
        case "addray":
            AddRayFromCam();
            break;
        case "clear": 
            Triang.Clear();
            TextSurface.WriteText("=== " + curLang[msgRangefinder] + " ===");
            TextSurface.WriteText("\n" + curLang[msgReady], true);
            return;
        case "refresh":
            Triang.Calculate();
            break;
        case "addpoint":
            AddPointOnRay();
            return;

        default:
            return;
    }

    TextSurface.WriteText("=== " + curLang[msgRangefinder] + " ===");
    TextSurface.WriteText("\n" + curLang[msgRaysCount] + ": " + Triang.Rays.Count.ToString() + ", " + curLang[msgPointsCount] + ": " + Triang.UsedPoints.ToString(), true);
    if (Triang.Rays.Count < 2) return;
        
    if (!Triang.TargetDefined) return;

    //дистанция
    Vector3D curPos = Camera.GetPosition();
    double dist = (Triang.Target - curPos).Length();

    TextSurface.WriteText("\n" + curLang[msgDistance] + ": " + dist.ToString("#0"), true);
    TextSurface.WriteText("\n" + curLang[msgScatter] + ": " + Triang.Scatter.ToString("#0.0"), true);
    TextSurface.WriteText("\n\n" + GPSPoint(curLang[msgGPSTARGET], Triang.Target), true);

    TextSurface.WriteText("\n\n" + curLang[msgInstrCount] + ": " + Runtime.CurrentInstructionCount.ToString(), true);
    TextSurface.WriteText("\n" + curLang[msgRunTime] + ": " + Runtime.LastRunTimeMs.ToString("#0.0000"), true);

    //TextSurface.WriteText("\n\n", true);
    //int i = 0;
    //foreach (Triangulation.PointInfo p in Triang.Points)
    //{
    //    i++;
    //    TextSurface.WriteText(string.Format("\n{0}: откл. {1:#0.0},  {2}", i, p.Scatter, p.Skipped ? "-" : "+"), true);
    //}
    //TextSurface.WriteText(string.Format("\nВсего точек: {0:#0}", Triang.Points.Count), true);
    //TextSurface.WriteText(string.Format("\nГрубый разброс: {0:#0.0}", Triang.RoughScatter), true);
    //TextSurface.WriteText("\n" + GPSPoint("Грубая ЦЕЛЬ", Triang.RoughTarget), true);
}

//==========================================================================================================

void AddRayFromCam()
{

    Vector3D pnt = Camera.GetPosition();
    Vector3D ray = Camera.WorldMatrix.Forward;
        
    Triang.AddRay(pnt, ray);
    Triang.Calculate();

}
void AddPointOnRay()
{
    
    TextSurface.WriteText("=== " + curLang[msgRangefinder] + " ===");
    if (InputCustomDataBlock == null)
    {
        TextSurface.WriteText("\n" + curLang[msgInputBlockNotSet], true);
        return;
    }

    string dataStr = InputCustomDataBlock.CustomData.Trim();
    if (dataStr == string.Empty)
    {
        TextSurface.WriteText("\n" + curLang[msgPointRangeNotSet], true);
        return;
    }

    int rng;
    if (!int.TryParse(dataStr, out rng))
    {
        TextSurface.WriteText("\n" + curLang[msgPointRangeNotSet], true);
        return;
    }

    Vector3D pnt = Camera.GetPosition() + Camera.WorldMatrix.Forward * rng;
    string pntName = string.Format(curLang[msgPointNameTemplate], rng);
    TextSurface.WriteText("\n" + pntName, true);
    TextSurface.WriteText("\n\n" + GPSPoint(pntName, pnt), true);
}

public class Triangulation
{

    public List<RayD> Rays = new List<RayD>();
    public List<PointInfo> Points = new List<PointInfo>();
    public Vector3D Target = Vector3D.Zero;
    public double Scatter = 0;
    public double RoughScatter = 0;
    public Vector3D RoughTarget = Vector3D.Zero;
    public int UsedPoints = 0;
    public bool TargetDefined = false;

    public void Clear()
    {
        Rays.Clear();
        ClearResaults();
    }
    private void ClearResaults()
    {
        Target = Vector3D.Zero;
        Points.Clear();
        Scatter = 0;
        RoughScatter = 0;
        RoughTarget = Vector3D.Zero;
        UsedPoints = 0;
        TargetDefined = false;
    }

    public void AddRay(Vector3D pos, Vector3D dir)
    {
        RayD r = new RayD(pos, dir);
        Rays.Add(r);
    }

    public void Calculate()
    {
        ClearResaults();

        if (Rays.Count < 2) return;

        int i, j;
        RayD r1, r2;
        double A, B, C, alpha, beta, gamma, invA;
        double sinAlpha, sinBeta, sinGamma, cosAlpha, cosBeta;
        Vector3D vectA, invVectA, point1, point2, point;

        for (i = 0; i < Rays.Count - 1; i++)
        {
            r1 = Rays[i];
            for (j = i + 1; j < Rays.Count; j++)
            {
                r2 = Rays[j];

                //решаем треугольник по основанию и двум углам, допуская что лучи сходятся.
                //Основание A, прилегающие углы alpha и beta. Противоположный угол gamma известен 180-(..).
                //Неизвестные катеты B и C (угол alpha между A и B, beta медлу A и C).
                //Решаем для луча B: высота hC = A*sin(beta) = B*sin(gamma) => B = A*sin(beta)/sin(gamma) 
                //Для луча C: C = A*sin(alpha)/sin(gamma) 

                vectA = (r2.Position - r1.Position);
                invVectA = Vector3D.Negate(vectA);
                A = vectA.Length();
                //слишком короткое основание?
                if (A < 10.0) continue;

                invA = 1.0 / A;

                //получим углы
                cosAlpha = Vector3D.Dot(r1.Direction, vectA) * invA;
                cosBeta = Vector3D.Dot(r2.Direction, invVectA) * invA;
                alpha = Math.Acos(cosAlpha);
                beta = Math.Acos(cosBeta);
                gamma = Math.PI - alpha - beta;
                //расходящиеся лучи?
                if (gamma < 0.001) continue;

                sinAlpha = Math.Sin(alpha);
                sinBeta = Math.Sin(beta);
                sinGamma = Math.Sin(gamma);

                B = A * sinBeta / sinGamma;
                C = A * sinAlpha / sinGamma;

                point1 = r1.Position + r1.Direction * B;
                point2 = r2.Position + r2.Direction * C;
                point = (point1 + point2) * 0.5;

                RoughTarget += point;
                Points.Add(new PointInfo { Point = point });
            }
        }

        UsedPoints = Points.Count;
        if (Points.Count == 0)
        {
            return;
        }

        TargetDefined = true;
        if (Points.Count == 1)
        {
            Target = RoughTarget;
            return;
        }

        //разброс
        RoughTarget /= UsedPoints;
        foreach (PointInfo p in Points)
        {
            p.Scatter = Vector3D.Distance(RoughTarget, p.Point);
            RoughScatter += p.Scatter * p.Scatter;
        }
        RoughScatter = Math.Sqrt(RoughScatter);
        Scatter = RoughScatter;

        if (UsedPoints <= 3)
        {
            Target = RoughTarget;
            return;
        }

        //исключение точек с большим разбросом
        bool wasSkipped = true;
        while (wasSkipped && UsedPoints > 3)
        {
            //сортируем по убыванию отклонения
            Points.Sort((a, b) => b.Scatter.CompareTo(a.Scatter));

            double goodScatter = 0.577 * Scatter;
            Target = Vector3D.Zero;
            wasSkipped = false;
            foreach (PointInfo p in Points)
            {
                if (p.Skipped) continue;

                if (UsedPoints > 3 && p.Scatter > goodScatter)
                {
                    //исключаем эту точку
                    UsedPoints--;
                    p.Skipped = true;
                    wasSkipped = true;
                }
                else
                {
                    Target += p.Point;
                }
            }

            Target /= UsedPoints;
            Scatter = 0;
            foreach (PointInfo p in Points)
            {
                if (p.Skipped) continue;

                p.Scatter = Vector3D.Distance(Target, p.Point);
                Scatter += p.Scatter * p.Scatter;
            }
            Scatter = Math.Sqrt(Scatter);
        }

    }

    public class PointInfo
    {
        public Vector3D Point;
        public double Scatter;
        public bool Skipped = false;
    }

}

private bool ReadConfig(out bool firstLaunch)
{

    bool resault = false;
    firstLaunch = false;

    const string iniSection = "General";

    string cfgText = Me.CustomData;
    MyIni ini = new MyIni();
    if (string.IsNullOrWhiteSpace(cfgText))
    {
        firstLaunch = true;
    }
    else
    {
        if (ini.TryParse(cfgText))
        {
            lang = ini.Get(iniSection, "Language").ToString(lang).ToLower();
            CameraName = ini.Get(iniSection, "Camera").ToString(CameraName);
            DisplayName = ini.Get(iniSection, "Display").ToString(DisplayName);
            InputCustomDataBlockName = ini.Get(iniSection, "InputCustomDataBlockName").ToString(InputCustomDataBlockName);

            resault = true;
        }
    }

    switch (lang)
    {
        case "ru":
            curLang = langMsgRu;
            break;
        case "en":
            curLang = langMsgEn;
            break;
        default:
            lang = "ru";
            curLang = langMsgRu;
            break;
    }

    ini.Clear();
    ini.Set(iniSection, "Language", lang);
    ini.Set(iniSection, "Camera", CameraName);
    ini.Set(iniSection, "Display", DisplayName);
    ini.Set(iniSection, "InputCustomDataBlockName", InputCustomDataBlockName);

    ini.SetComment(iniSection, "Language", "Одно из (one of): ru, en");
    ini.SetComment(iniSection, "Camera", curLang[msgCommentCamera]);
    ini.SetComment(iniSection, "Display", curLang[msgCommentDisplay]);
    ini.SetComment(iniSection, "InputCustomDataBlockName", curLang[msgCommentInputCustomData]);

    Me.CustomData = ini.ToString();

    return resault;
}

#region helpers
string VectToStr(Vector3D v)
{
    string s = string.Format("{0:#0.0}, {1:#0.0}, {2:#0.0}", v.X, v.Y, v.Z);
    return s;
}
private static string GPSPoint(string pntName, Vector3D pnt)
{
    string s = "GPS:" + pntName + string.Format(":{0:#0.0}:{1:#0.0}:{2:#0.0}:", pnt.X, pnt.Y, pnt.Z);
    return s;
}

//поиск блоков на том же гриде, что и anyBlock
//дисплей может быть текстовой панелью ("Дисплей 1х1") или дисплеем в кабине или в другом блоке: "Кокпит(3)" или "Медблок(0)"
private static IMyTextSurface FindDisplay(string DisplayName, IMyGridTerminalSystem gts, IMyTerminalBlock anyBlock)
{
    IMyTextSurface TextSurface = null;

    int x = DisplayName.IndexOf('(');
    if (x == -1)
    {
        TextSurface = FindBlockOfType<IMyTextSurface>(DisplayName, gts, anyBlock);
    }
    else
    {
        string mName = DisplayName.Substring(0, x);
        IMyTextSurfaceProvider sp = FindBlockOfType<IMyTextSurfaceProvider>(mName, gts, anyBlock);
        if (sp != null)
        {
            int y = DisplayName.IndexOf(')');
            string indexStr = DisplayName.Substring(x + 1, y - x - 1).Trim();
            int surfIndex = int.Parse(indexStr);
            TextSurface = sp.GetSurface(surfIndex);
        }
    }

    return TextSurface;
}
private static T FindBlockOfType<T>(string CustomName, IMyGridTerminalSystem gts, IMyTerminalBlock anyBlock) where T : class
{
    List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
    gts.GetBlocksOfType<IMyTerminalBlock>(allBlocks, block => block.IsSameConstructAs(anyBlock) && block is T && block.CustomName == CustomName);

    if (allBlocks.Count == 0) return null;

    return allBlocks[0] as T;
}
#endregion

#region Language set
private string lang = "ru";

private const int msgNewCfg = 0;
private const int msgBrokenCfg = 1;
private const int msgDisplayNotFound = 2;
private const int msgCameraNotfound = 3;
private const int msgInitComplete = 4;
private const int msgInitError = 5;
private const int msgRangefinder = 6;
private const int msgReady = 7;
private const int msgRaysCount = 8;
private const int msgPointsCount = 9;
private const int msgDistance = 10;
private const int msgScatter = 11;
private const int msgGPSTARGET = 12;
private const int msgInstrCount = 13;
private const int msgRunTime = 14;
private const int msgCommentCamera = 15;
private const int msgCommentDisplay = 16;
private const int msgCommentInputCustomData = 17;
private const int msgInputBlockNotSet = 18;
private const int msgPointRangeNotSet = 19;
private const int msgPointNameTemplate = 20;

private string[] langMsgRu =
{
"Была сформирована новая конфигурация. Отредактируйте \"Свои данные (Custom Data)\" и перекомпилируйте скрипт.",
"Ошибка чтения конфигурации из пользовательских данных. Конфигурация была сформирована заново. Отредактируйте \"Свои данные (Custom Data)\" и перекомпилируйте скрипт.",
"Дисплей не определен!",
"Камера не определена!",
"Инициализация завершена.",
"Ошибка инициализации скрипта!",
"Дальномер",
"Готов",
"Лучей",
"точек",
"Дистанция",    //10
"Разброс",
"ЦЕЛЬ",
"Кол. инструкций",
"Время выполн, мс",
"Назване камеры",
"Назване текстовой панели. Это может быть дисплей или любой другой блок,\nимеющий дисплей, например: \"Дисплей 1х1\" или \"Кокпит(0)\",\nгде индекс указывает номер дисплея в блоке, нумерация начинается с 0.",
"Имя блока, в котором \"Свои данные (Custom Data)\" будут использоваться\nдля пользовательского ввода. Удобно будет указать текущий кокпит.",
"В настройках не задано\nимя блока для ввода данных.",
"Нужно указать расстояние до точки\nв \"Свои данные\" блока для ввода данных.",
"Точка {0} м"   //20
};

private string[] langMsgEn =
{
"A new configuration has been formed. Edit the \"Custom Data\" and recompile the script.",
"Error reading configuration from custom data. The configuration has been generated anew. Edit the \"Custom Data\" and recompile the script.",
"Display not defined!",
"Camera not defined!",
"Initialization complete.",
"Script initialization failed!",
"Rangefinder",
"Ready",
"Rays",
"points",
"Distance",     //10
"Scatter",
"TARGET",
"Instruct. count",
"Run time, ms",
"Name of camera",
"Name the text panel. This can be a display or any other block,\nhaving a LCD, for example: \"Display 1x1\" or \"Cockpit(0)\",\nwhere the index indicates the number of the display in the block,\nthe numbering starts from 0.",
"The name of the block in which the \"Custom Data\" will be used\nfor user input. It will be convenient to set the current cockpit.",
"The settings do not specify\nthe name of the block\nfor data entry.",
"You need to specify the distance\nto the point in the \"Custom Data\"\nof block for data entry.",
"Point {0} m"   //20
};
#endregion
