/*
 * ============================================================================
 * SCRIPT DE TELEMETRIA E DIAGNÓSTICO DE GRID (Versão Otimizada)
 * ============================================================================
 * DESCRIÇÃO:
 * Este script varre a sua nave/estação e gera um relatório completo de 
 * engenharia no 'Custom Data' (Dados Personalizados) deste bloco.
 * É a ferramenta ideal para extrair o perfil exato da sua grid e passar 
 * para uma IA planejar melhorias, refatorações ou scripts de combate.
 * * CAPACIDADES E FUNCIONALIDADES:
 * - Execução Automática e Leve: Roda silenciosamente a cada 15 segundos.
 * - Orientação Inteligente: Identifica automaticamente a "Frente".
 * - Física e Massa: Calcula a coordenada do Centro de Massa Local.
 * - Energia e Suporte: Geração (MW), Baterias (MWh) e Hidrogênio (Litros).
 * - Propulsão Vetorial: Força de empuxo exata (kN / MN) em 6 direções.
 * - Mapeamento Tático e Lógico (Posições X, Y, Z).
 * * COMO USAR:
 * 1. Apenas compile o código ("Check Code"). Ele começa a rodar sozinho.
 * 2. Para forçar uma leitura imediata, clique em "Run" (Executar).
 * 3. Abra o botão "Custom Data" deste bloco para copiar o texto.
 * ============================================================================
 */

int contadorTempo = 0;

public Program()
{
    // Configura o script para "acordar" a cada 100 ticks (aprox. 1.66 segundos)
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    
    // Faz a primeira varredura assim que o script é compilado
    GerarRelatorio();
}

public void Main(string argument, UpdateType updateSource)
{
    // Se o usuário clicou em "Run" manualmente, força a atualização na hora
    if ((updateSource & (UpdateType.Trigger | UpdateType.Terminal)) != 0)
    {
        GerarRelatorio();
        contadorTempo = 0; // Reseta o relógio
        return;
    }

    // Se foi acionado automaticamente pelo Update100, conta o tempo
    contadorTempo++;
    
    // 9 ciclos de 1.66s dão aproximadamente 15 segundos
    if (contadorTempo >= 9) 
    {
        GerarRelatorio();
        contadorTempo = 0; // Zera para contar mais 15s
    }
}

// Transformamos todo o código de varredura em uma função isolada
void GerarRelatorio()
{
    IMyTextSurface lcd = Me.GetSurface(0);
    lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    System.Text.StringBuilder rel = new System.Text.StringBuilder();

    rel.AppendLine("=== DIAGNÓSTICO DE GRID E TELEMETRIA ===");
    rel.AppendLine($"Atualizado em: {System.DateTime.Now.ToString("HH:mm:ss")}\n");

    List<IMyTerminalBlock> todosBlocos = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(todosBlocos, b => b.CubeGrid == Me.CubeGrid);

    IMyShipController controleRef = null;
    
    float geracaoMW = 0f;
    float bateriaMWh = 0f;
    float hidrogenioL = 0f;

    List<IMyThrust> propulsores = new List<IMyThrust>();
    List<IMyGyro> giroscopios = new List<IMyGyro>();
    List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
    List<IMyUserControllableGun> armasFixas = new List<IMyUserControllableGun>();
    List<IMyLargeTurretBase> torretas = new List<IMyLargeTurretBase>();
    List<IMyShipConnector> conectores = new List<IMyShipConnector>();
    List<IMyTerminalBlock> antenasSinalizadores = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> logica = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> ias = new List<IMyTerminalBlock>();

    foreach (var b in todosBlocos)
    {
        if (b is IMyShipController) 
        {
            var ctrl = (IMyShipController)b;
            if (controleRef == null || ctrl.IsMainCockpit) controleRef = ctrl;
            logica.Add(b); 
        }
        
        if (b is IMyBatteryBlock) bateriaMWh += ((IMyBatteryBlock)b).MaxStoredPower;
        if (b is IMyPowerProducer && !(b is IMyBatteryBlock)) geracaoMW += ((IMyPowerProducer)b).MaxOutput;
        if (b is IMyGasTank && b.BlockDefinition.SubtypeId.IndexOf("Hydro", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            hidrogenioL += ((IMyGasTank)b).Capacity;
        }

        if (b is IMyThrust) propulsores.Add((IMyThrust)b);
        else if (b is IMyGyro) giroscopios.Add((IMyGyro)b);
        else if (b is IMyCameraBlock) cameras.Add((IMyCameraBlock)b);
        else if (b is IMyLargeTurretBase) torretas.Add((IMyLargeTurretBase)b);
        else if (b is IMyUserControllableGun) armasFixas.Add((IMyUserControllableGun)b);
        else if (b is IMyShipConnector) conectores.Add((IMyShipConnector)b);
        else if (b is IMyRadioAntenna || b is IMyBeacon || b is IMyLaserAntenna) antenasSinalizadores.Add(b);
        else if (b is IMyTimerBlock || b is IMyProgrammableBlock || b is IMyEventControllerBlock) logica.Add(b);
        // CORREÇÃO AQUI: Removemos o bloco fantasma IMyBasicBehaviorBlock da verificação
        else if (b is IMyFlightMovementBlock || b is IMyPathRecorderBlock || 
                 b is IMyDefensiveCombatBlock || b is IMyOffensiveCombatBlock)
        {
            ias.Add(b);
        }
    }

    IMyTerminalBlock blocoRef = controleRef != null ? (IMyTerminalBlock)controleRef : Me;
    rel.AppendLine($"Frente baseada em: {blocoRef.CustomName}");

    if (controleRef != null)
    {
        VRageMath.Vector3D comWorld = controleRef.CenterOfMass;
        VRageMath.Vector3D comLocal = VRageMath.Vector3D.Transform(comWorld, VRageMath.MatrixD.Invert(Me.CubeGrid.WorldMatrix));
        rel.AppendLine($"Centro de Massa (Local): [X:{comLocal.X:F1}, Y:{comLocal.Y:F1}, Z:{comLocal.Z:F1}]");
    }
    else rel.AppendLine("Centro de Massa: Desconhecido");
    rel.AppendLine("");

    rel.AppendLine("--- SUPORTE À VIDA E ENERGIA ---");
    rel.AppendLine($"- Geração Máx: {geracaoMW:F2} MW");
    rel.AppendLine($"- Cap. Baterias: {bateriaMWh:F2} MWh");
    rel.AppendLine($"- Cap. Hidrogênio: {hidrogenioL:N0} L\n");

    float forceFrente = 0, forceTras = 0, forceCima = 0, forceBaixo = 0, forceEsq = 0, forceDir = 0;
    foreach (var prop in propulsores) 
    {
        var dir = prop.WorldMatrix.Backward; 
        float pot = prop.MaxEffectiveThrust; 
        
        if (VRageMath.Vector3D.Dot(dir, blocoRef.WorldMatrix.Forward) > 0.9) forceFrente += pot;
        else if (VRageMath.Vector3D.Dot(dir, blocoRef.WorldMatrix.Backward) > 0.9) forceTras += pot;
        else if (VRageMath.Vector3D.Dot(dir, blocoRef.WorldMatrix.Up) > 0.9) forceCima += pot;
        else if (VRageMath.Vector3D.Dot(dir, blocoRef.WorldMatrix.Down) > 0.9) forceBaixo += pot;
        else if (VRageMath.Vector3D.Dot(dir, blocoRef.WorldMatrix.Left) > 0.9) forceEsq += pot;
        else if (VRageMath.Vector3D.Dot(dir, blocoRef.WorldMatrix.Right) > 0.9) forceDir += pot;
    }
    rel.AppendLine($"--- VETORES DE PROPULSÃO ({propulsores.Count} blocos) ---");
    rel.AppendLine($"Frente: {FormatarForca(forceFrente)} | Trás: {FormatarForca(forceTras)}");
    rel.AppendLine($"Cima: {FormatarForca(forceCima)} | Baixo: {FormatarForca(forceBaixo)}");
    rel.AppendLine($"Esq: {FormatarForca(forceEsq)} | Dir: {FormatarForca(forceDir)}\n");

    rel.AppendLine($"--- INTELIGÊNCIA ARTIFICIAL ({ias.Count} blocos) ---");
    foreach (var ia in ias) rel.AppendLine($"- {ia.CustomName}: Coord {ia.Position}");

    rel.AppendLine($"\n--- LÓGICA E CONTROLE ({logica.Count} blocos) ---");
    foreach (var lg in logica) rel.AppendLine($"- {lg.CustomName} (Coord {lg.Position})");

    rel.AppendLine($"\n--- COMUNICAÇÃO E ACOPLAMENTO ---");
    foreach (var comm in antenasSinalizadores) rel.AppendLine($"- {comm.CustomName}");
    foreach (var con in conectores) rel.AppendLine($"- {con.CustomName}: Coord {con.Position}");

    rel.AppendLine($"\n--- NAVEGAÇÃO E COMBATE ---");
    rel.AppendLine($"Giroscópios: {giroscopios.Count} | Câmeras: {cameras.Count}");
    rel.AppendLine($"Armas Fixas: {armasFixas.Count} | Torretas: {torretas.Count}");

    rel.AppendLine($"\n==============================");
    rel.AppendLine($"Pronto para leitura da IA.");

    string textoFinal = rel.ToString();
    Me.CustomData = textoFinal;
    
    string anim = new string('.', contadorTempo % 4);
    lcd.WriteText($"Processando varredura a cada 15s{anim}\n>> RELATÓRIO NO CUSTOM DATA <<\n\n{textoFinal}");
}

string FormatarForca(float newtons) 
{
    if (newtons >= 1000000) return (newtons / 1000000).ToString("0.00") + " MN";
    if (newtons >= 1000) return (newtons / 1000).ToString("0.00") + " kN";
    return newtons.ToString("0") + " N";
}