// ThrustCalcLCD Script Made By Marcus V 1.82
// Version Francaise 1.1

// Sur le Deuxieme Paragraphe du LCD vous trouverez la somme de la Poussée des Propulseurs ,
// ainsi qu'une valeur ( celle entre crochet ) vous indiquant la poussée necessaire contre la gravitée actuel dans chaque Sens .
// Ne Compte que les Propulseurs sur la même grille que le Bloc Programmable

// Valeur que vous pouvez editer :

    string CockpitTag = "Cockpit";          // Nom de votre Cockpit .
    string lcdTag = "ThrustCalcLCD";    // Nom de l'ecran LCD .
    Single fontsize = 0.84f;                 // Taille de Police des Ecran LCD ( pour les cockpit , c'est en manuel )
    
    double overrideGravity = 0;          // Entrer une valeur pour passer outre la gravité ( en G ) .
    
    bool Show_in_Cockpit = false;          // Determine si Oui ou non vous voulez afficher le Script dans un Cockpit .
                                                              // Vous pouvez Modifier le chiffre pour Changer d'afficheur a la Ligne "182" .

    bool Enable_Auto_Refresh = true;  // Active ou non le rafraichissement automatique du Script .
                                                           //  ( utiliser en argument "refresh" si vous le voulez en rafraichissement manuel )

// Variables
    IMyTextSurface content_display;
    IMyShipController cockpit;
    List<IMyThrust> thrusters;
    int ThrustFWD;
    int ThrustBWD;
    int ThrustLeft;
    int ThrustRight;
    int ThrustUp;
    int ThrustDown;
    int cntUp;
    int cntDown;
    int cntLeft;
    int cntRight;
    int cntFWD;
    int cntBWD;
    float DryMass;
    float TotalMass;
    float PhysMass;
    MyShipMass Masses;
    double Accel;

    public Program()
    {
        if (Enable_Auto_Refresh == true)
        {
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        cockpit = GridTerminalSystem.GetBlockWithName(CockpitTag) as IMyShipController;  
        thrusters = new List<IMyThrust>();     
    }
    
    int GetDirection(IMyTerminalBlock block, IMyTerminalBlock reference)
    {
        Matrix refm;
        Matrix bmat;
        reference.Orientation.GetMatrix(out refm);
        block.Orientation.GetMatrix(out bmat);
        bmat = bmat * Matrix.Transpose(refm);

        int dir = (int)bmat.Forward.Dot(new Vector3(1, 2, 3));
        dir = (2 * Math.Abs(dir) - 2) + (Math.Sign(dir) + 1) / 2;
        return dir;
    }

    public void Main(string argument)
    {
        GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters, filterThis);
        Accel = cockpit.GetNaturalGravity().Length() + ( overrideGravity * 10 );
        Masses = cockpit.CalculateShipMass();
        DryMass = Masses.BaseMass / 1000;
        TotalMass = Masses.TotalMass / 1000;
        PhysMass = Masses.PhysicalMass / 1000;

        ThrustFWD = 0;
        ThrustBWD = 0;
        ThrustUp = 0;
        ThrustDown = 0;
        ThrustLeft = 0;
        ThrustRight = 0;
        cntUp = 0;
        cntDown = 0;
        cntFWD = 0;
        cntBWD = 0;
        cntLeft = 0;
        cntRight = 0;

//0) Right, 1) Left, 2) Up, 3) Down, 4) Backward, 5) Forward
        for (int i = 0; i < thrusters.Count; i++)
        {
            if ((thrusters[i] as IMyFunctionalBlock).Enabled == false) continue;
            if (GetDirection(thrusters[i], cockpit) == 2)
            {
                ThrustUp += (int)(thrusters[i].MaxEffectiveThrust) / 1000;
                cntUp++;
            }
            else if (GetDirection(thrusters[i], cockpit) == 3)
            {
                ThrustDown += (int)(thrusters[i].MaxEffectiveThrust) / 1000;
                cntDown++;
            }
            else if (GetDirection(thrusters[i], cockpit) == 5)
            {
                ThrustFWD += (int)(thrusters[i].MaxEffectiveThrust) / 1000;
                cntFWD++;
            }
            else if (GetDirection(thrusters[i], cockpit) == 4)
            {
                ThrustBWD += (int)(thrusters[i].MaxEffectiveThrust) / 1000;
                cntBWD++;
            }
            else if (GetDirection(thrusters[i], cockpit) == 1)
            {
                ThrustLeft += (int)(thrusters[i].MaxEffectiveThrust) / 1000;
                cntLeft++;
            }
            else if (GetDirection(thrusters[i], cockpit) == 0)
            {
                ThrustRight += (int)(thrusters[i].MaxEffectiveThrust) / 1000;
                cntRight++;
            }
        }

    var sb = new StringBuilder();
    var sb2 = new StringBuilder();

// Lignes a Ecrire sur Ecrans LCD
    sb.Append(
          "*****************************************************************\n"
        +" G = " + Accel.ToString("F1") + " m/s² (" + (Accel / 9.81).ToString("F1") +  " G)\n"
        +" Total des Propulseurs: " + thrusters.Count + "\n"
        +" Poussée Mini requise Vers le Haut: " + (PhysMass * Accel).ToString("F1") + "kN\n"
        +" Poussée verticale: " + ((ThrustUp / PhysMass) - Accel).ToString("F2") + "m/s\n"
        +"       MASSE ACTUELLE\n" 
        +" Masse a Vide : " + DryMass.ToString("F1") + " т\n"
        +" Masse Embarqué : " + (TotalMass - DryMass).ToString("F1") + " т\n"
        +" Masse Total actuelle : " + TotalMass.ToString("F1") + " т\n"
        +"       MASSE CALCULEE\n"
        +" Peut embarquer encore : " + ((((PhysMass * Accel) - ThrustUp) / Accel) * -1).ToString("F1") + " т\n"
        +"*****************************************************************\n"
        +" Force requise contre les G actuel dans chaque Sens\n"
        +"-----------------------------------------------------------------------\n"
        +" Vers le Haut (" + cntUp + ") : " + ThrustUp + "kN" +
                            "  [" + (ThrustUp - PhysMass * Accel).ToString("F1") + "kN]\n"
        +" Vers le Bas (" + cntDown + ") : " + ThrustDown + "kN" + 
                            "  [" + (ThrustDown - PhysMass * Accel).ToString("F1") + "kN]\n"
        +" En Avant (" + cntFWD + ") : " + ThrustFWD + "kN" +
                            "  [" + (ThrustFWD - PhysMass * Accel).ToString("F1") + "kN]\n"
        +" En Arriere (" + cntBWD + ") : " + ThrustBWD + "kN" +
                            "  [" + (ThrustBWD - PhysMass * Accel).ToString("F1") + "kN]\n"
        +" Vers la Gauche (" + cntLeft + ") : " + ThrustLeft + "kN" +
                            "  [" + (ThrustLeft - PhysMass * Accel).ToString("F1") + "kN]\n"
        +" Vers la Droite (" + cntRight + ") : " + ThrustRight +
                            "kN" + "  [" + (ThrustRight - PhysMass * Accel).ToString("F1") + "kN]\n"
        +"******************************************************************\n"
        );

    sb2.Append(
          "*****************************************************************\n"
        +" g = " + Accel.ToString("F1") + " m/s² (" + (Accel / 9.81).ToString("F1") +  " G)\n\n"
        +" -POUSSEE verticale-\n"
        +"  " + ((ThrustUp / PhysMass) - Accel).ToString("F2") + "m/s\n"
        +" -MASSE Embarqué-\n"
        +"  " + (TotalMass - DryMass).ToString("F1") + " т\n"
        +" -PLACE Restante-\n"
        +"  " + ((((PhysMass * Accel) - ThrustUp) / Accel) * -1).ToString("F1") + " т\n"
        +"*****************************************************************\n"
        );

// Appel des Ecrans LCD
      var lcdBlocks = new List<IMyTextPanel>();
      GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcdBlocks, filterThis);

// Appel des Ecrans LCD dans Cockpit
      IMyTextSurfaceProvider provider;
      provider = cockpit as IMyTextSurfaceProvider;

      if (Show_in_Cockpit == true)
      {
        content_display = provider.GetSurface(0);  // Changer le chiffre pour changer d'afficheur ex: GetSurface(1)
        content_display.ContentType = ContentType.TEXT_AND_IMAGE;
        content_display.TextPadding = 1;
        content_display.WriteText(sb2);
      }

// Parametre des Ecrans LCD
    foreach(var lcd in lcdBlocks)
     {
      if(lcd.CustomName == lcdTag)
          {
          lcd.Font = ("DarkBlue");
          lcd.ContentType = ContentType.TEXT_AND_IMAGE;
          lcd.SetValueFloat("FontSize",fontsize);
          lcd.TextPadding = 0;
          if (lcd != null)
          lcd.WriteText(sb);
          }
     }
}
    bool filterThis (IMyTerminalBlock block)
    {
    return block.CubeGrid == Me.CubeGrid;
    }