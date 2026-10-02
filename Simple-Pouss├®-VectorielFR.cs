// TOUJOURS VERIFIER LES MISES A JOUR
// pour mettre a jour, rechargez-le simplement depuis l'onglet workshop (pas besoin d'aller sur la page workshop)

/*
═══════════════════════════════════════════════════════════════════
VECTOR THRUST 3.7 FR - VERSION 2.4 STABLE
═══════════════════════════════════════════════════════════════════

NOUVELLES AMÉLIORATIONS V2.4 :
	• Suppression du mode croisière (simplification)
	• Anti-tremblements renforcé (hystérésis + stabilisation passive)
	• Conduite plus fluide à basse vitesse (< 0.20 m/s)
	• Organisation interne améliorée (Main découpé en helpers)
	• Optimisation légère des comparaisons de chaînes

═══════════════════════════════════════════════════════════════════

COMMANDES DISPONIBLES (à utiliser dans la barre d'outils du cockpit) :

  %veille           - Mettre en pause / reprendre le script
  %entrerVeille     - Forcer le mode veille
  %sortirVeille     - Forcer la sortie de veille
  %propulseurs      - Activer/désactiver les propulseurs
  %amortisseurs     - Activer/désactiver les amortisseurs
  %augmenterAccel   - Augmenter l'accélération cible
  %diminuerAccel    - Diminuer l'accélération cible
  %reinitAccel      - Réinitialiser l'accélération par défaut
  %reinitialiser    - Rescanner tous les blocs
	%axesAuto         - Détection automatique (1 axe et 2 axes)
	%axes1            - Forcer le mode nacelles 1 axe
	%axes2            - Forcer le mode nacelles 2 axes
  %appliquerTags    - Marquer automatiquement les blocs avec |VT|
  %retirerTags      - Retirer tous les tags |VT|

CONFIGURATION LCD :
  - Nom du LCD : %VecteurLCD
  - Custom Data du cockpit : %Vecteur:0 (0 = 1er écran, 1 = 2ème, etc.)

MODES DE FONCTIONNEMENT :
  - Mode "Greedy" (par défaut) : Utilise TOUS les blocs automatiquement
  - Mode "Tags" : Utilise uniquement les blocs marqués |VT|
    → Lancez %appliquerTags pour activer ce mode

COMPATIBILITÉ :
  ✓ Vol atmosphérique (planètes)
  ✓ Vol spatial (espace)
  ✓ Transitions atmosphère/espace
  ✓ Control Module (gamepad)

═══════════════════════════════════════════════════════════════════
*/

// Amortisseurs actives au demarrage du script
public bool dampeners = true;

// Propulseurs actives au demarrage du script
public bool jetpack = false;

// Ceci identifie les blocs appartenant a ce bloc programmable.
// Utilisez l'argument '%appliquerTags' pour que le programme marque tous les blocs qu'il controle.
// Le programme n'interagira qu'avec les blocs ayant ce tag, sauf si vous utilisez '%retirerTags'.
// Si vous ajoutez ou retirez un tag manuellement, utilisez '%reinitialiser' pour forcer une reverification
// Si vous rendez ce tag unique pour ce vaisseau, il n'interferera pas avec vos autres vaisseaux a poussee vectorielle
public const string myName = "VT";
// actif: |VT|
public const string activeSurround = "|";
// veille: .VT.
public const string standbySurround = ".";

// Mettez ceci dans les donnees personnalisees d'un cockpit pour utiliser un ecran de ce cockpit
// Doit etre sur sa propre ligne, avec un nombre entier apres le ':'
// Le nombre doit etre 0 <= nombre <= nombre total d'ecrans dans le cockpit
// eg:
//		%Vecteur:0
// Ceci utiliserait le 1er ecran. Le 1er ecran est #0, le 2eme #1 etc..
// En cas de probleme, regardez en bas a droite du terminal du BP, il affichera les erreurs
public const string textSurfaceKeyword = "%Vecteur:";

// La veille arrete tous les calculs et eteint toutes les nacelles en toute securite
// Utile si vous voulez arreter de voler sans eteindre le vaisseau
public const bool startInStandby = true;
// Changez ceci si vous ne voulez pas que le script demarre en veille... utilisez ceci uniquement avec permission du proprietaire du serveur

// Vitesse maximale des rotors en RPM (-1 pour la vitesse max du jeu, change avec les mods)
public float maxRotorRPM = 60f;

// Anti-butée: ralentit les articulations près des limites pour éviter les tremblements
public const float limitSoftZoneDeg = 8f;
public const float limitHardZoneDeg = 1.5f;

public const float defaultAccel = 1f;// Acceleration cible par defaut affichee
// Si vous voulez changer la valeur par defaut, modifiez ceci
// Note : valeurs > 1 signifient que vos nacelles pointeront vers le sol quand vous voulez descendre
// plutot que simplement reduire la poussee
// '1g' = acceleration causee par la gravite actuelle (pas necessairement 9.81m/s)
// si la gravite actuelle est < 0.1m/s, ce reglage sera ignore et sera 9.81m/s de toute facon

public const float accelBase = 1.5f;// accel = defaultAccel * g * base^exposant
// Les touches +, - et 0 incrementent, decrementent et reinitialisent l'exposant
// Augmenter la base augmente la quantite de changement d'acceleration avec + et -

// Multiplicateur pour les amortisseurs, plus eleve = amortisseurs plus forts
public const float dampenersModifier = 0.1f;

// =====================================================================
// REGLAGES RECOMMANDES (version stable)
// Ajustez progressivement, puis testez en vol entre chaque changement.
// =====================================================================
public const float dampenersCompOnSpeed = 0.30f;   // active la compensation inertielle
public const float dampenersCompOffSpeed = 0.10f;  // coupe la compensation inertielle
public const float lowSpeedNoInputThreshold = 0.20f;

public const double rotorDeadzoneNoInputLow = 0.14; // ~8°
public const double rotorDeadzoneLow = 0.09;        // ~5.2°
public const double rotorDeadzoneSlow = 0.05;       // ~3°
public const double rotorDeadzoneNormal = 0.01;     // ~0.6°

public const double rotorDampingDefault = 0.7;
public const double rotorDampingNoInputLow = 0.82;

public const double rotorMinRpmNormal = 0.1;
public const double rotorMinRpmLow = 0.25;
public const double rotorMinRpmNoInputLow = 0.45;


// Acceleration par defaut en gravite zero (ou faible)
public const float zeroGAcceleration = 9.81f;
// Si la gravite devient inferieure a cela, zeroGAcceleration s'active
public const float gravCutoff = 0.1f * zeroGAcceleration;


// Determine quand les propulseurs s'eteindront dans l'espace
public const float lowThrustCutOff = 0.1f;
// Determine quand les propulseurs se rallumeront. Doit toujours etre > cutoff
public const float lowThrustCutOn = 1.0f;

// true: seul le cockpit principal peut etre utilise meme si personne n'est dedans
// false: tous les cockpits peuvent etre utilises, mais si quelqu'un est dans le cockpit principal, seul celui-ci sera obei
// pas de cockpit principal: tous les cockpits peuvent etre utilises
public const bool onlyMainCockpit = true;

// Choisissez si vous voulez que le script se mette a jour a chaque frame, toutes les 10 frames, ou toutes les 100 frames
// Doit etre l'un de:
// UpdateFrequency.Update1
// UpdateFrequency.Update10
// UpdateFrequency.Update100
public const UpdateFrequency update_frequency = UpdateFrequency.Update1;

public const string LCDName = "%VecteurLCD";

// Arguments de commande - vous pouvez les modifier pour changer le texte avec lequel vous executez le bloc programmable
public const string standbytogArg = "%veille";
public const string standbyonArg = "%entrerVeille";
public const string standbyoffArg = "%sortirVeille";
public const string dampenersArg = "%amortisseurs";
public const string jetpackArg = "%propulseurs";
public const string raiseAccelArg = "%augmenterAccel";
public const string lowerAccelArg = "%diminuerAccel";
public const string resetAccelArg = "%reinitAccel";
public const string resetArg = "%reinitialiser";// Reexecute la configuration initiale... vous voulez probablement utiliser %reinitAccel
public const string applyTagsArg = "%appliquerTags";
public const string removeTagsArg = "%retirerTags";
public const string axisAutoArg = "%axesAuto";
public const string axis1Arg = "%axes1";
public const string axis2Arg = "%axes2";

public enum AxisMode {
	Auto = 0,
	SingleAxisOnly = 1,
	TwoAxisOnly = 2
}

// Controles gamepad du module de controle
// Tapez "/cm showinputs" dans le chat
// Appuyez sur le bouton desire
// Mettez ce texte EXACTEMENT tel quel entre guillemets pour le controle voulu
public const string jetpackButton = "c.thrusts";
public const string dampenersButton = "c.damping";
public const string lowerAccel = "c.switchleft";
public const string raiseAccel = "c.switchright";
public const string resetAccel = "pipe";

// Reglages de boost (fonctionne uniquement avec le module de controle)
// Vous pouvez utiliser ceci pour definir des valeurs d'acceleration cible auxquelles vous pouvez rapidement acceder en maintenant le bouton specifie
// Valeurs par defaut:
// 	c.sprint (shift)	3g
// 	ctrl 			0.3g
public const bool useBoosts = true;
public BA[] boosts = {
	new BA("c.sprint", 3f),
	new BA("ctrl", 0.3f)
};



public struct BA {
	public string button;
	public float accel;

	public BA(string button, float accel) {
		this.button = button;
		this.accel = accel;
	}
}



//                              V 180 degrees
//              V 0 degrees                      V 360 degrees
// 				|-----\                    /------
// desired power|----------------------------------------- value of 0.1
// 				|       \                /
// 				|        \              /
// 				|         \            /
// no power 	|-----------------------------------------
//
//
// 				|-----\                    /------stuff above desired power gets set to desired power
// 				|      \                  /
// 				|       \                /
// desired power|----------------------------------------- value of 0.8
// 				|         \            /
// no power 	|-----------------------------------------

// Les schémas ci-dessus concernent 'thrustModifierAbove'. Le même principe s'applique à
// 'thrustModifierBelow', sauf qu'il agit sous la ligne 0 au lieu d'au-dessus de la ligne de puissance max.
// La valeur de clipping 'thrustModifier' définit l'écart angulaire toléré tout en gardant la poussée cible.
// Ces valeurs doivent rester entre 0 et 1.

// Autre façon de le voir :
// avec above = 1, on garde 100% de la poussée cible même loin de la direction voulue.
// avec below = 1, on tombe à 0% de poussée cible même loin de la direction opposée.

// avec above = 0, on a 100% de poussée cible uniquement si l'alignement est parfait.
// avec below = 0, on a 0% de poussée cible uniquement si l'alignement opposé est parfait.

public const double thrustModifierAboveSpace = 0.01;
public const double thrustModifierBelowSpace = 0.9;

public const double thrustModifierAboveGrav = 0.1;
public const double thrustModifierBelowGrav = 0.1;

























































// Utiliser le module de controle... peut toujours etre true
public bool controlModule = true;

public Program() {
	Echo("Vient d'etre Compile");
	programCounter = 0;
	gotNacellesCount = 0;
	updateNacellesCount = 0;
	Runtime.UpdateFrequency = UpdateFrequency.Once;
	this.greedy = !hasTag(Me);
	if(Me.CustomData.Equals("")) {
		Me.CustomData = textSurfaceKeyword + 0;
	}
}
public void Save() {}


//a 60 fps cela durera 9000+ hrs avant de devenir negatif
public long programCounter;
public long gotNacellesCount;
public long updateNacellesCount;



// ====== Gestion des trains d'atterrissage (Landing Gear) ======
// Liste globale des trains d'atterrissage de la grille
List<IMyLandingGear> landingGears = new List<IMyLandingGear>();
// Limite la fréquence des rescans GTS pour alléger la boucle principale.
int landingGearScanCounter = 0;
const int landingGearScanIntervalTicks = 6;
// Flag pour mémoriser l'état précédent (au moins un train verrouillé ou non)
bool wasAnyLocked = false;
// Etat global: au moins un train est verrouille
bool anyLandingGearLocked = false;
// Mémorise si le script a force les amortisseurs OFF a cause du lock
bool dampenersForcedOffByLandingGear = false;
// Mémorise l'etat précédent pour restauration au déverrouillage
bool dampenersBeforeLandingGearLock = true;

// Fonction dédiée à la gestion des landing gears et de l'inertie
void CheckLandingGearAndInertia() {
	landingGearScanCounter++;
	if(landingGearScanCounter >= landingGearScanIntervalTicks || landingGears.Count == 0) {
		landingGearScanCounter = 0;
		landingGears.Clear();
		GridTerminalSystem.GetBlocksOfType<IMyLandingGear>(landingGears, lg => lg.CubeGrid == Me.CubeGrid);
	} else {
		landingGears.RemoveAll(lg => lg == null || !lg.IsAlive() || lg.CubeGrid != Me.CubeGrid);
	}

	bool anyLocked = false;
	foreach (var lg in landingGears) {
		if (lg.LockMode == LandingGearMode.Locked) {
			anyLocked = true;
			break;
		}
	}
	anyLandingGearLocked = anyLocked;
	// Désactive les amortisseurs uniquement lors du passage à l'état verrouillé
	if (anyLocked && !wasAnyLocked) {
		dampenersBeforeLandingGearLock = dampeners;
		if (dampeners) {
			dampeners = false;
			dampenersForcedOffByLandingGear = true;
			write("Amortisseurs désactivés : train d'atterrissage verrouillé");
		}
	}

	// Restaure les amortisseurs si c'est ce script qui les avait forcés OFF
	if (!anyLocked && wasAnyLocked && dampenersForcedOffByLandingGear) {
		dampeners = dampenersBeforeLandingGearLock;
		dampenersForcedOffByLandingGear = false;
		write("Train déverrouillé : amortisseurs restaurés");
	}
	wasAnyLocked = anyLocked;
}

public void Main(string argument, UpdateType runType) {
	// Gestion landing gear & inertie (séparée pour clarté)
	CheckLandingGearAndInertia();






	// ========== STARTUP ==========
	WriteRuntimeHeader();
	argument = FilterAndNormalizeArgument(argument, runType);
	Echo("Gourmand: " + this.greedy);
	bool togglePower = argument.Contains(standbytogArg);
	bool axisModeChanged = ApplyAxisModeCommand(argument);

	// Si %propulseurs est passé, getMovementInput() va basculer 'jetpack'.
	// On capture l'état avant pour pouvoir appliquer l'ON/OFF aux thrusters normaux après.
	bool jetpackBefore = jetpack;

	bool anyArg = HasAnyCommandArg(argument);
	if(HandleStandbyStateMachine(argument, runType, anyArg, togglePower)) {
		return;
	}

	if(justCompiled || controllers.Count == 0 || argument.Contains(resetArg) || axisModeChanged) {
		if(!init()) {
			return;
		}
	}



	if(!RefreshBlocksAndTags(argument)) {
		return;
	}




	if(HandlePostCompileState()) {
		return;
	}

	if(standby) {
		Echo("En Veille");
		write("En Veille");
		return;
	}

	// ========== END OF STARTUP ==========










	// ========== PHYSICS ==========

	// Récupère la gravité en espace monde
	Vector3D worldGrav = usableControllers[0].theBlock.GetNaturalGravity();

	// Récupère la vitesse du vaisseau
	MyShipVelocities shipVelocities = usableControllers[0].theBlock.GetShipVelocities();
	shipVelocity = shipVelocities.LinearVelocity;
	// Vector3D shipAngularVelocity = shipVelocities.AngularVelocity;

	// Prépare la masse physique
	MyShipMass myShipMass = usableControllers[0].theBlock.CalculateShipMass();
	float shipMass = myShipMass.PhysicalMass;

	if(myShipMass.BaseMass < 0.001f) {
		Echo("Impossible de piloter une Station");
		shipMass = 0.001f;
	}

	// Prépare les coefficients selon la gravité
	float gravLength = (float)worldGrav.Length();
	if(gravLength < gravCutoff) {
		gravLength = zeroGAcceleration;
		thrustModifierAbove = thrustModifierAboveSpace;
		thrustModifierBelow = thrustModifierBelowSpace;
	}
	else {
		thrustModifierAbove = thrustModifierAboveGrav;
		thrustModifierBelow = thrustModifierBelowGrav;
	}

	Vector3D desiredVec = getMovementInput(argument);
	pilotInputActive = desiredVec.LengthSquared() > 0.0004;

	// Si le flag 'jetpack' a changé, synchroniser l'état des propulseurs normaux
	if(jetpack != jetpackBefore) {
		setNormalThrustersEnabled(jetpack);
	}

	// f = m * a
	Vector3D shipWeight = shipMass * worldGrav;





	// Anti-jitter: hystérésis pour éviter les bascules rapides autour de l'arrêt.
	float dampenersOnSpeed = dampenersCompOnSpeed;
	float dampenersOffSpeed = dampenersCompOffSpeed;
	float shipSpeed = (float)shipVelocity.Length();
	UpdateDampenersCompState(shipSpeed, dampenersOnSpeed, dampenersOffSpeed);

	if(dampenersCompActive) {
		Vector3D dampVec = Vector3D.Zero;

		// Détection du type de propulseur dominant
		float hydroThrust = 0f, ionThrust = 0f, atmoThrust = 0f;
		foreach(var t in normalThrusters) {
			if(t == null || !t.IsAlive()) continue;
			string subtype = t.BlockDefinition.SubtypeName;
			if(subtype.IndexOf("hydrogen", StringComparison.OrdinalIgnoreCase) >= 0) {
				hydroThrust += t.CurrentThrust;
			} else if(subtype.IndexOf("atmo", StringComparison.OrdinalIgnoreCase) >= 0) {
				atmoThrust += t.CurrentThrust;
			} else if(subtype.IndexOf("ion", StringComparison.OrdinalIgnoreCase) >= 0) {
				ionThrust += t.CurrentThrust;
			}
		}
		// Même logique pour les nacelles
		foreach(Nacelle n in nacelles) {
			foreach(Thruster t in n.thrusters) {
				if(t.theBlock == null || !t.theBlock.IsAlive()) continue;
				string subtype = t.theBlock.BlockDefinition.SubtypeName;
				if(subtype.IndexOf("hydrogen", StringComparison.OrdinalIgnoreCase) >= 0) {
					hydroThrust += t.theBlock.CurrentThrust;
				} else if(subtype.IndexOf("atmo", StringComparison.OrdinalIgnoreCase) >= 0) {
					atmoThrust += t.theBlock.CurrentThrust;
				} else if(subtype.IndexOf("ion", StringComparison.OrdinalIgnoreCase) >= 0) {
					ionThrust += t.theBlock.CurrentThrust;
				}
			}
		}
		// Détermination du type dominant
		float maxThrust = Math.Max(hydroThrust, Math.Max(ionThrust, atmoThrust));
		float localDampenersModifier = dampenersModifier;
		if(maxThrust == hydroThrust && hydroThrust > 0) {
			localDampenersModifier = dampenersModifier * 2.5f; // Amortissement fort pour hydrogène
		} else if(maxThrust == atmoThrust && atmoThrust > 0) {
			localDampenersModifier = dampenersModifier * 1.5f; // Amortissement intermédiaire pour atmosphérique
		} // Ion = valeur par défaut

		if(desiredVec != Vector3D.Zero) {
			// Annule le mouvement opposé à la direction demandée
			if(desiredVec.dot(shipVelocity) < 0) {
				dampVec += shipVelocity.project(desiredVec.normalized());
			}
			// Annule le mouvement latéral
			dampVec += shipVelocity.reject(desiredVec.normalized());
		} else {
			dampVec += shipVelocity;
		}

		desiredVec -= dampVec * localDampenersModifier;
	}



	// f = m * a
	desiredVec *= shipMass * (float)getAcceleration(gravLength);

	// Oriente la poussée dans le sens opposé et ajoute le poids (force, pas accélération)
	Vector3D requiredVec = -desiredVec + shipWeight;
	bool hasNaturalGravity = worldGrav.LengthSquared() > (gravCutoff * gravCutoff);
	bool suppressNormalThrustersForHover = hasNaturalGravity && !pilotInputActive;

	// Retire la poussée déjà produite par les propulseurs fixes
	if(!suppressNormalThrustersForHover) {
		for(int i = 0; i < normalThrusters.Count; i++) {
			requiredVec -= -1 * normalThrusters[i].WorldMatrix.Backward * normalThrusters[i].CurrentThrust;
		}
	}

	Echo("Force Requise: " + $"{Math.Round(requiredVec.Length(),0)}" + "N");

	// Repos simplifié et extinction active : uniformité TOTALE entre nacelles et normaux
	bool atRest = (requiredVec.LengthSquared() < (30f * 30f)); // seuil: 30N
	bool noInput = (desiredVec.LengthSquared() < 0.01f); // pas d'input pilote
	bool realRest = atRest && noInput; // repos vraiment détecté
	bool normalShouldBeOff = !jetpack || !thrustOn || realRest || suppressNormalThrustersForHover; // coupe aussi en gravité sans input
	
	// UNIFORMITÉ: Même logique d'extinction pour propulseurs normaux ET nacelles
	for(int i = 0; i < normalThrusters.Count; i++) {
		var t = normalThrusters[i];
		if(t != null && t.IsAlive()) {
			if(normalShouldBeOff) {
				t.ThrustOverride = 0f;
				t.Enabled = false; // EXTINCTION ACTIVE comme les nacelles
			} else if(jetpack) {
				t.Enabled = true; // Rallumage quand conditions OK
				// Override géré par IA/dampeners
			}
		}
	}
	
	// Les nacelles sont déjà gérées dans leur propre logique (nacelle.go())
	// mais on s'assure du reset override au repos pour cohérence
	if(realRest) {
		foreach(Nacelle n in nacelles) {
			foreach(Thruster t in n.thrusters) {
				if(t.theBlock != null && t.theBlock.IsAlive()) {
					t.theBlock.ThrustOverride = 0f;
				}
			}
		}
	}

	// ========== END OF PHYSICS ==========









	// ========== DISTRIBUTE THE FORCE EVENLY BETWEEN NACELLES ==========

	// Hystérésis
	if(requiredVec.Length() > lowThrustCutOn * gravCutoff * shipMass) {//TODO: this causes problems if there are many small nacelles
		thrustOn = true;
	}
	if(requiredVec.Length() < lowThrustCutOff * gravCutoff * shipMass) {
		thrustOn = false;
	}

	// Amélioration possible: lisser cette transition
	if(!thrustOn) {// Zero G
		Vector3D zero_G_accel = Vector3D.Zero;
		if(mainController != null) {
			zero_G_accel = (mainController.theBlock.WorldMatrix.Down + mainController.theBlock.WorldMatrix.Backward) * zeroGAcceleration / 1.414f;
		} else {
			zero_G_accel = (usableControllers[0].theBlock.WorldMatrix.Down + usableControllers[0].theBlock.WorldMatrix.Backward) * zeroGAcceleration / 1.414f;
		}
		if(dampeners) {
			requiredVec = zero_G_accel * shipMass + requiredVec;
		} else {
			requiredVec = (requiredVec - shipVelocity) + zero_G_accel;
		}
	}

	// Met à jour l'état des propulseurs et revérifie la direction des nacelles
	bool gravChanged = Math.Abs(lastGrav - gravLength) > 0.05f;
	lastGrav = gravLength;
	foreach(Nacelle n in nacelles) {
		// Met à jour si les propulseurs ne sont plus valides ou si l'environnement a changé
		if(!n.validateThrusters(jetpack) || gravChanged) {
			n.detectThrustDirection();
		}
	}

	// Regroupe les nacelles par axe de rotor pour répartir la force proprement.
	// Regroupe les nacelles ayant un axe de rotor similaire
	List<List<Nacelle>> nacelleGroups = new List<List<Nacelle>>();
	for(int i = 0; i < nacelles.Count; i++) {
		bool foundGroup = false;
		foreach(List<Nacelle> g in nacelleGroups) {// vérifie l'alignement avec chaque groupe
			if(Math.Abs(Vector3D.Dot(nacelles[i].rotor.theBlock.WorldMatrix.Up, g[0].rotor.theBlock.WorldMatrix.Up)) > 0.9f) {
				g.Add(nacelles[i]);
				foundGroup = true;
				break;
			}
		}
		if(!foundGroup) {// si aucun groupe ne correspond, en crée un
			nacelleGroups.Add(new List<Nacelle>());
			nacelleGroups[nacelleGroups.Count-1].Add(nacelles[i]);
		}
	}

	// Corrige le désalignement global des nacelles
	Vector3D correctionErreur = Vector3D.Zero;
	// 1) Projection initiale par groupe
	foreach(List<Nacelle> g in nacelleGroups) {
		// En 2 axes, on garde le vecteur complet : projeter sur un seul axe casse les déplacements latéraux.
		if(g[0].hasSecondaryRotor()) {
			g[0].requiredVec = requiredVec;
		} else {
			g[0].requiredVec = requiredVec.reject(g[0].rotor.theBlock.WorldMatrix.Up);
		}
		correctionErreur += g[0].requiredVec;
	}
	// 2) Erreur globale
	correctionErreur -= requiredVec;
	// 3) Retrait de l'erreur par groupe
	foreach(List<Nacelle> g in nacelleGroups) {
		g[0].requiredVec -= correctionErreur;
	}
	// 4) Moyenne
	correctionErreur /= nacelleGroups.Count;
	// 5) Réinjection de la moyenne
	foreach(List<Nacelle> g in nacelleGroups) {
		g[0].requiredVec += correctionErreur;
	}
	// Applique le réglage de la première nacelle au reste du groupe
	double total = 0;
	foreach(List<Nacelle> g in nacelleGroups) {
		Vector3D req = g[0].requiredVec / g.Count;
		for(int i = 0; i < g.Count; i++) {
			g[i].requiredVec = req;
			g[i].thrustModifierAbove = thrustModifierAbove;
			g[i].thrustModifierBelow = thrustModifierBelow;
			// Echo(g[i].errStr);
			g[i].go(jetpack, dampeners, shipMass);
			total += req.Length();
		}
	}
	Echo("Force Totale: " + $"{Math.Round(total,0)}" + "N");


    // ========== AFFICHAGE DES AVARIES ==========

    int avaries = 0;
    // Vérifier les rotors et propulseurs des nacelles
    foreach(Nacelle n in nacelles) {
		if (n.rotor.theBlock == null || !n.rotor.theBlock.IsFunctional) {
            avaries++;
        }
        foreach(Thruster t in n.thrusters) {
			if (t.theBlock == null || !t.theBlock.IsFunctional) {
                avaries++;
            }
        }
    }
    // Vérifier les propulseurs normaux
    foreach(IMyThrust t in normalThrusters) {
        if (!t.IsFunctional) {
            avaries++;
        }
    }

	// Flag/message d'alerte (coloration gérée plus bas au moment de l'écriture)
	bool alertActive = avaries > 0;

	// Affichage LCD en tableau 2 colonnes (format normal)
	string lcdText = "Tableau de bord vectoriel\n\n";
	string axisModeLabel = "Auto";
	if(axisMode == AxisMode.SingleAxisOnly) {
		axisModeLabel = "1 axe";
	} else if(axisMode == AxisMode.TwoAxisOnly) {
		axisModeLabel = "2 axes";
	}

	double vitesse = shipVelocity.Length();
	double graviteG = worldGrav.Length() / 9.81;
	double accelCible = getAcceleration(gravLength) / gravLength;
	double masseT = shipMass / 1000.0;

	// Valeurs de référence pour les barres (ajustables si besoin)
	double vitesseRatio = Math.Min(vitesse / 100.0, 1.0);
	double graviteRatio = Math.Min(graviteG / 2.0, 1.0);
	double accelRatio = Math.Min(accelCible / 3.0, 1.0);
	double masseRatio = Math.Min(masseT / 300.0, 1.0);

	int propulseursFonctionnels = 0;
	int propulseursTotaux = normalThrusters.Count;
	foreach(IMyThrust t in normalThrusters) {
		if (t != null && t.IsFunctional) {
			propulseursFonctionnels++;
		}
	}

	int nacellesFonctionnelles = 0;
	int nacellesTotales = nacelles.Count;
	foreach(Nacelle n in nacelles) {
		propulseursTotaux += n.thrusters.Count;
		foreach(Thruster t in n.thrusters) {
			if (t.theBlock != null && t.theBlock.IsFunctional) {
				propulseursFonctionnels++;
			}
		}
		if (n.rotor.theBlock != null && n.rotor.theBlock.IsFunctional) {
			nacellesFonctionnelles++;
		}
	}

	double propulseursRatio = propulseursTotaux > 0 ? (double)propulseursFonctionnels / propulseursTotaux : 0;
	double nacellesRatio = nacellesTotales > 0 ? (double)nacellesFonctionnelles / nacellesTotales : 0;

	int largeurCol = 24;
	lcdText += "Alerte".PadRight(largeurCol) + "| " + avaries + "\n";
	lcdText += "Vitesse".PadRight(largeurCol) + "| " + vitesseRatio.progressBar() + " " + Math.Round(vitesse, 1) + " m/s\n";
	lcdText += "Gravité".PadRight(largeurCol) + "| " + graviteRatio.progressBar() + " " + Math.Round(graviteG, 2) + " G\n";
	lcdText += "Accélération".PadRight(largeurCol) + "| " + accelRatio.progressBar() + " " + Math.Round(accelCible, 2) + " g\n";
	lcdText += "Masse".PadRight(largeurCol) + "| " + masseRatio.progressBar() + " " + Math.Round(masseT, 0) + " t\n";
	lcdText += "Propulseurs".PadRight(largeurCol) + "| " + (jetpack ? "ACTIFS" : "INACTIFS") + "\n";
	lcdText += "Amortisseurs".PadRight(largeurCol) + "| " + (dampeners ? "ACTIFS" : "INACTIFS") + "\n";
	lcdText += "Mode axes".PadRight(largeurCol) + "| " + axisModeLabel + "\n";
	lcdText += "Propulseurs fonctionnels".PadRight(largeurCol) + "| " + propulseursRatio.progressBar() + " " + propulseursFonctionnels + "/" + propulseursTotaux + "\n";
	lcdText += "Nacelles fonctionnelles".PadRight(largeurCol) + "| " + nacellesRatio.progressBar() + " " + nacellesFonctionnelles + "/" + nacellesTotales + "\n";
	// Couleur globale: rouge si alerte active, sinon blanc
	foreach(IMyTextSurface surface in this.surfaces) {
		surface.FontColor = alertActive ? Color.Red : Color.White;
		surface.ContentType = ContentType.TEXT_AND_IMAGE;
	}
	write(lcdText);
	// ========== END OF MAIN ==========

	// Affiche les erreurs liées aux surfaces
	Echo(surfaceProviderErrorStr);
}

void WriteRuntimeHeader() {
	globalAppend = false;
	programCounter++;
	Echo($"Derniere Execution {Runtime.LastRunTimeMs.Round(2)}ms");
	String spinner = "";
	switch(programCounter/10%4) {
		case 0:
			spinner = "|";
		break;
		case 1:
			spinner = "\\";
		break;
		case 2:
			spinner = "-";
		break;
		case 3:
			spinner = "/";
		break;
	}
	write($"{spinner} {Runtime.LastRunTimeMs.Round(0)}ms");
}

string FilterAndNormalizeArgument(string argument, UpdateType runType) {
	UpdateType validArgumentUpdates = UpdateType.None;
	validArgumentUpdates |= UpdateType.Terminal;
	validArgumentUpdates |= UpdateType.Trigger;
	validArgumentUpdates |= UpdateType.Script;
	if((runType & validArgumentUpdates) == UpdateType.None) {
		return "";
	}
	return argument.ToLower();
}

bool ApplyAxisModeCommand(string argument) {
	bool axisModeChanged = false;
	if(argument.Contains(axisAutoArg) && axisMode != AxisMode.Auto) {
		axisMode = AxisMode.Auto;
		axisModeChanged = true;
	}
	if(argument.Contains(axis1Arg) && axisMode != AxisMode.SingleAxisOnly) {
		axisMode = AxisMode.SingleAxisOnly;
		axisModeChanged = true;
	}
	if(argument.Contains(axis2Arg) && axisMode != AxisMode.TwoAxisOnly) {
		axisMode = AxisMode.TwoAxisOnly;
		axisModeChanged = true;
	}
	return axisModeChanged;
}

bool HasAnyCommandArg(string argument) {
	return
	argument.Contains(dampenersArg) ||
	argument.Contains(jetpackArg) ||
	argument.Contains(standbytogArg) ||
	argument.Contains(standbyonArg) ||
	argument.Contains(standbyoffArg) ||
	argument.Contains(raiseAccelArg) ||
	argument.Contains(lowerAccelArg) ||
	argument.Contains(resetAccelArg) ||
	argument.Contains(resetArg) ||
	argument.Contains(axisAutoArg) ||
	argument.Contains(axis1Arg) ||
	argument.Contains(axis2Arg) ||
	argument.Contains(applyTagsArg) ||
	argument.Contains(removeTagsArg);
}

bool HandleStandbyStateMachine(string argument, UpdateType runType, bool anyArg, bool togglePower) {
	if(argument.Contains(standbyonArg) || goToStandby) {
		enterStandby();
		return true;
	} else if(argument.Contains(standbyoffArg) || comeFromStandby) {
		exitStandby();
		return true;
	} else if((togglePower && !standby) || goToStandby) {
		enterStandby();
		return true;
	} else if((anyArg || runType == UpdateType.Terminal) && standby || comeFromStandby) {
		exitStandby();
	} else {
		Echo("Fonctionnement Normal");
	}
	return false;
}

bool RefreshBlocksAndTags(string argument) {
	this.applyTags = argument.Contains(Program.applyTagsArg);
	this.removeTags = !this.applyTags && argument.Contains(Program.removeTagsArg);
	this.greedy = (!this.applyTags && this.greedy) || this.removeTags;
	if(this.applyTags) {
		addTag(Me);
	} else if(this.removeTags) {
		removeTag(Me);
	}
	if(!checkNacelles()) {
		Echo("Configuration echouee, arret.");
		return false;
	}
	this.applyTags = false;
	this.removeTags = false;
	return true;
}

bool HandlePostCompileState() {
	if(!justCompiled) {
		return false;
	}
	justCompiled = false;
	Runtime.UpdateFrequency = UpdateFrequency.Once;
	if(Storage == "" || !startInStandby) {
		Storage = "Don't Start Automatically";
		comeFromStandby = true;
		return true;
	}
	goToStandby = true;
	return true;
}

void UpdateDampenersCompState(float shipSpeed, float dampenersOnSpeed, float dampenersOffSpeed) {
	if(!dampeners) {
		dampenersCompActive = false;
	} else if(dampenersCompActive) {
		if(shipSpeed < dampenersOffSpeed) {
			dampenersCompActive = false;
		}
	} else if(shipSpeed > dampenersOnSpeed) {
		dampenersCompActive = true;
	}
}

bool ContainsIgnoreCase(string text, string value) {
	if(text == null || value == null) return false;
	return text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
}


public string surfaceProviderErrorStr = "";

public int accelExponent = 0;

public bool jetpackIsPressed = false;
public bool dampenersIsPressed = false;
public bool plusIsPressed = false;
public bool minusIsPressed = false;

public bool globalAppend = false;

public ShipController mainController = null;
public List<ShipController> controllers = new List<ShipController>();
public List<ShipController> usableControllers = new List<ShipController>();
public List<Nacelle> nacelles = new List<Nacelle>();
public List<IMyThrust> normalThrusters = new List<IMyThrust>();
public List<IMyTextPanel> screens = new List<IMyTextPanel>();
public List<IMyTextPanel> usableScreens = new List<IMyTextPanel>();
public HashSet<IMyTextSurface> surfaces = new HashSet<IMyTextSurface>();
public List<IMyProgrammableBlock> programBlocks = new List<IMyProgrammableBlock>();

public float oldMass = 0;

public int rotorCount = 0;
public int rotorTopCount = 0;
public int thrusterCount = 0;
public int screenCount = 0;
public int programBlockCount = 0;


public bool standby = startInStandby;
public Vector3D shipVelocity = Vector3D.Zero;
public bool dampenersCompActive = false;
public bool pilotInputActive = false;
public double thrustModifierAbove = 0.1;// how close the rotor has to be to target position before the thruster gets to full power
public double thrustModifierBelow = 0.1;// how close the rotor has to be to opposite of target position before the thruster gets to 0 power

public bool justCompiled = true;
public bool goToStandby = false;
public bool comeFromStandby = false;

public const string tag = activeSurround + myName + activeSurround;
public const string offtag = standbySurround + myName + standbySurround;

public bool applyTags = false;
public bool removeTags = false;
public bool greedy = true;
public float lastGrav = 0;
public bool thrustOn = false;
public AxisMode axisMode = AxisMode.Auto;
public int twoAxisNacelleCount = 0;

public Dictionary<string, object> CMinputs = null;

public void setNormalThrustersEnabled(bool enabled) {
	for(int i = 0; i < normalThrusters.Count; i++) {
		var t = normalThrusters[i];
		if(t == null || !t.IsAlive()) continue;
		// ON/OFF et reset des overrides pour éviter les micro-poussées non contrôlées
		t.Enabled = enabled;
		if(!enabled) {
			// Quand on coupe, on remet l'override à 0 pour éviter les poussées résiduelles
			t.ThrustOverride = 0f;
		}
		// Note: quand enabled=true, on laisse les overrides intacts pour permettre
		// le contrôle par d'autres systèmes (IA, dampeners, manual, etc.)
	}
	
	// Appliquer la même logique aux propulseurs des nacelles pour uniformité
	foreach(Nacelle n in nacelles) {
		foreach(Thruster t in n.thrusters) {
			if(t.theBlock != null && t.theBlock.IsAlive()) {
				t.theBlock.Enabled = enabled;
				if(!enabled) {
					t.theBlock.ThrustOverride = 0f;
				}
			}
		}
	}
}






public void enterStandby() {
	standby = true;
	goToStandby = false;

	//set status of blocks
	foreach(Nacelle n in nacelles) {
		n.rotor.theBlock.Enabled = false;
		standbyTag(n.rotor.theBlock);
		if(n.hasSecondaryRotor()) {
			n.secondaryRotor.theBlock.Enabled = false;
			standbyTag(n.secondaryRotor.theBlock);
		}
		foreach(Thruster t in n.thrusters) {
			t.theBlock.Enabled = false;
			standbyTag(t.theBlock);
		}
	}
	foreach(IMyTextPanel screen in usableScreens) {
		standbyTag(screen);
	}
	foreach(ShipController cont in usableControllers) {
		standbyTag(cont.theBlock);
	}
	// Couper aussi les propulseurs normaux pendant la veille
	setNormalThrustersEnabled(false);
	standbyTag(Me);

	Runtime.UpdateFrequency = UpdateFrequency.None;

	Echo("En Veille");
	write("En Veille");
}

public void exitStandby() {
	standby = false;
	comeFromStandby = false;

	//set status of blocks
	foreach(Nacelle n in nacelles) {
		n.rotor.theBlock.Enabled = true;
		activeTag(n.rotor.theBlock);
		if(n.hasSecondaryRotor()) {
			n.secondaryRotor.theBlock.Enabled = true;
			activeTag(n.secondaryRotor.theBlock);
		}
		foreach(Thruster t in n.thrusters) {
			if(t.IsOn) {
				t.theBlock.Enabled = true;
			}
			activeTag(t.theBlock);
		}
	}
	foreach(IMyTextPanel screen in usableScreens) {
		activeTag(screen);
	}
	foreach(ShipController cont in usableControllers) {
		activeTag(cont.theBlock);
	}
	activeTag(Me);

	// Rétablir l'état des propulseurs normaux selon 'jetpack'
	setNormalThrustersEnabled(jetpack);

	Runtime.UpdateFrequency = update_frequency;
}

public bool hasTag(IMyTerminalBlock block) {
	return block.CustomName.Contains(tag) || block.CustomName.Contains(offtag);
}

public void addTag(IMyTerminalBlock block) {
	string name = block.CustomName;

	if(name.Contains(tag)) {
		// there is already a tag, just set it to current status
		if(standby) {
			block.CustomName = name.Replace(tag, offtag);
		}

	} else if(name.Contains(offtag)) {
		// there is already a tag, just set it to current status
		if(!standby) {
			block.CustomName = name.Replace(offtag, tag);
		}

	} else {
		// no tag found, add tag to start of string

		if(standby) {
			block.CustomName = offtag + " " + name;
		} else {
			block.CustomName = tag + " " + name;
		}
	}

}

public void removeTag(IMyTerminalBlock block) {
	block.CustomName = block.CustomName.Replace(tag, "").Trim();
	block.CustomName = block.CustomName.Replace(offtag, "").Trim();
}

public void standbyTag(IMyTerminalBlock block) {
	block.CustomName = block.CustomName.Replace(tag, offtag);
}

public void activeTag(IMyTerminalBlock block) {
	block.CustomName = block.CustomName.Replace(offtag, tag);
}


// true: seul le cockpit principal peut etre utilise meme si personne n'est dedans
// false: tous les cockpits peuvent etre utilises, mais si quelqu'un est dans le cockpit principal, seul celui-ci sera obei
// pas de cockpit principal: tous les cockpits peuvent etre utilises
public bool onlyMain() {
	return mainController != null && (mainController.theBlock.IsUnderControl || onlyMainCockpit);
}

public void getScreens() {
	getScreens(this.screens);
}

public void getScreens(List<IMyTextPanel> screens) {
	bool greedy = this.greedy || this.applyTags || this.removeTags;
	this.screens = screens;
	usableScreens.Clear();
	foreach(IMyTextPanel screen in screens) {

		if(this.removeTags) {
			removeTag(screen);
		}

		// Un écran est utilisable s'il est fonctionnel ET (soit le mode est "greedy", soit il a un tag, soit il contient le nom spécial)
		bool isUsable = screen.IsWorking && (greedy || hasTag(screen) || ContainsIgnoreCase(screen.CustomName, LCDName));

		if(!isUsable) {
			surfaces.Remove(screen);
			continue;
		}

		if(this.applyTags) {
			addTag(screen);
		}
		usableScreens.Add(screen);
		surfaces.Add(screen);
	}
	screenCount = screens.Count;
}

public void write(string str) {
	if(this.surfaces.Count > 0) {
		str += "\n";
		foreach(IMyTextSurface surface in this.surfaces) {
			surface.WriteText(str, globalAppend);
			surface.ContentType = ContentType.TEXT_AND_IMAGE;
		}
	} else if(!globalAppend) {
		Echo("Aucune surface de texte disponible");
	}
	globalAppend = true;
}
double getAcceleration(double gravity) {
	// Parcourt les boosts et applique la première accélération correspondante
	if(Program.useBoosts && this.controlModule) {
		for(int i = 0; i < this.boosts.Length; i++) {
			if(this.CMinputs.ContainsKey(this.boosts[i].button)) {
				return this.boosts[i].accel * gravity * defaultAccel;
			}
		}
	}

	// Aucun boost trouvé (ou désactivé): accélération normale
	return Math.Pow(accelBase, accelExponent) * gravity * defaultAccel;
}

public Vector3D getMovementInput(string arg) {
	Vector3D moveVec = Vector3D.Zero;

	if(controlModule) {
		// Initialise le module de contrôle
		Dictionary<string, object> inputs = new Dictionary<string, object>();
		try {
			this.CMinputs = Me.GetValue<Dictionary<string, object>>("ControlModule.Inputs");
			Me.SetValue<string>("ControlModule.AddInput", "all");
			Me.SetValue<bool>("ControlModule.RunOnInput", true);
			Me.SetValue<int>("ControlModule.InputState", 1);
			Me.SetValue<float>("ControlModule.RepeatDelay", 0.016f);
		} catch(Exception e) {
			controlModule = false;
		}
	}

	if(controlModule) {
		if(this.CMinputs == null) {
			this.CMinputs = new Dictionary<string, object>();
		}

		// Commandes hors déplacement
		if(this.CMinputs.ContainsKey(dampenersButton) && !dampenersIsPressed) {//inertia dampener key
			dampeners = !dampeners;//toggle
			dampenersIsPressed = true;
		}
		if(!this.CMinputs.ContainsKey(dampenersButton)) {
			dampenersIsPressed = false;
		}


		if(this.CMinputs.ContainsKey(jetpackButton) && !jetpackIsPressed) {//jetpack key
			jetpack = !jetpack;//toggle
			jetpackIsPressed = true;
		}
		if(!this.CMinputs.ContainsKey(jetpackButton)) {
			jetpackIsPressed = false;
		}

		if(this.CMinputs.ContainsKey(raiseAccel) && !plusIsPressed) {//throttle up
			accelExponent++;
			plusIsPressed = true;
		}
		if(!this.CMinputs.ContainsKey(raiseAccel)) { //increase target acceleration
			plusIsPressed = false;
		}

		if(this.CMinputs.ContainsKey(lowerAccel) && !minusIsPressed) {//throttle down
			accelExponent--;
			minusIsPressed = true;
		}
		if(!this.CMinputs.ContainsKey(lowerAccel)) { //lower target acceleration
			minusIsPressed = false;
		}

		if(this.CMinputs.ContainsKey(resetAccel)) { //default target acceleration
			accelExponent = 0;
		}

	}

	bool changeDampeners = false;
	if(arg.Contains(dampenersArg)) {
		dampeners = !dampeners;
		changeDampeners	= true;
	}
	if(arg.Contains(jetpackArg)) {
		jetpack = !jetpack;
	}
	if(arg.Contains(raiseAccelArg)) {
		accelExponent++;
	}
	if(arg.Contains(lowerAccelArg)) {
		accelExponent--;
	}
	if(arg.Contains(resetAccelArg)) {
		accelExponent = 0;
	}

	// Amortisseurs (si des propulseurs fixes existent, la commande amortisseurs est active)
	if(normalThrusters.Count != 0) {

		if(onlyMain()) {

			if(changeDampeners) {
				mainController.theBlock.DampenersOverride = dampeners;
			} else {
				dampeners = mainController.theBlock.DampenersOverride;
			}
		} else {

			if(changeDampeners) {
				// Uniformise l'état sur tous les contrôleurs
				foreach(ShipController cont in usableControllers) {
					cont.setDampener(dampeners);
				}
			} else {

				// Vérifie si un contrôleur diffère de l'état courant
				bool any_different = false;
				foreach(ShipController cont in usableControllers) {
					if(cont.theBlock.DampenersOverride != dampeners) {
						any_different = true;
						dampeners = cont.theBlock.DampenersOverride;
						break;
					}
				}

				if(any_different) {
					// Répercute la nouvelle valeur sur les autres contrôleurs
					foreach(ShipController cont in usableControllers) {
						cont.setDampener(dampeners);
					}
				}
			}
		}
	}


	// Commandes de déplacement
	if(onlyMain()) {
		moveVec = mainController.theBlock.getWorldMoveIndicator();
	} else {
		foreach(ShipController cont in usableControllers) {
			if(cont.theBlock.IsUnderControl) {
				moveVec += cont.theBlock.getWorldMoveIndicator();
			}
		}
	}

	return moveVec;
}

void removeSurface(IMyTextSurface surface) {
	if(this.surfaces.Contains(surface)) {
		//need to check this, because otherwise it will reset panels
		//we aren't controlling
		this.surfaces.Remove(surface);
		surface.ContentType = ContentType.NONE;
		surface.WriteText("", false);
	}
}

bool removeSurfaceProvider(IMyTerminalBlock block) {
	if(!(block is IMyTextSurfaceProvider)) return false;
	IMyTextSurfaceProvider provider = (IMyTextSurfaceProvider)block;

	for(int i = 0; i < provider.SurfaceCount; i++) {
		if(surfaces.Contains(provider.GetSurface(i))) {
			removeSurface(provider.GetSurface(i));
		}
	}
	return true;
}

bool addSurfaceProvider(IMyTerminalBlock block) {
	if(!(block is IMyTextSurfaceProvider)) return false;
	IMyTextSurfaceProvider provider = (IMyTextSurfaceProvider)block;
	bool retval = true;

	if(block.CustomData.Length == 0) {
		return false;
	}

	bool [] to_add = new bool[provider.SurfaceCount];
	for(int i = 0; i < to_add.Length; i++) {
		to_add[i] = false;
	}

	int begin_search = 0;
	while(begin_search >= 0) {
		string data = block.CustomData;
		int start = data.IndexOf(textSurfaceKeyword, begin_search);

		if(start < 0) {
			// Vrai si au moins une entrée valide a été trouvée
			retval =  begin_search != 0;
			break;
		}
		int end = data.IndexOf("\n", start);
		begin_search = end;

		string display = "";
		if(end < 0) {
			display = data.Substring(start + textSurfaceKeyword.Length);
		} else {
			display = data.Substring(start + textSurfaceKeyword.Length, end - (start + textSurfaceKeyword.Length) );
		}

		int display_num = 0;
		if(Int32.TryParse(display, out display_num)) {
			if(display_num >= 0 && display_num < provider.SurfaceCount) {
				// Succès: ajoute la surface
				to_add[display_num] = true;

			} else {
				// Échec: index hors plage
				string err_str = "";
				if(end < 0) {
					err_str = data.Substring(start);
				} else {
					err_str = data.Substring(start, end - (start) );
				}
				surfaceProviderErrorStr += $"\nDisplay number out of range: {display_num}\nshould be: 0 <= num < {provider.SurfaceCount}\non line: ({err_str})\nin block: {block.CustomName}\n";
			}

		} else {
			//didn't parse
			string err_str = "";
			if(end < 0) {
				err_str = data.Substring(start);
			} else {
				err_str = data.Substring(start, end - (start) );
			}
			surfaceProviderErrorStr += $"\nDisplay number invalid: {display}\non line: ({err_str})\nin block: {block.CustomName}\n";
		}
	}

	for(int i = 0; i < to_add.Length; i++) {
		if(to_add[i]) {
			this.surfaces.Add(provider.GetSurface(i));
		} else {
			removeSurface(provider.GetSurface(i));
		}
	}


	return retval;
}

bool getControllers() {
	return getControllers(this.controllers);
}

bool getControllers(List<IMyShipController> blocks) {
	List<ShipController> conts = new List<ShipController>();
	foreach(IMyShipController imy in blocks) {
		conts.Add(new ShipController(imy));
	}
	return getControllers(conts);
}

bool getControllers(List<ShipController> blocks) {
	bool greedy = this.greedy || this.applyTags || this.removeTags;
	mainController = null;
	this.controllers = blocks;

	usableControllers.Clear();

	string reason = "";
	for(int i = 0; i < blocks.Count; i++) {
		bool canAdd = true;
		string currreason = blocks[i].theBlock.CustomName + "\n";
		// Note: ShowInTerminal toujours accepté (ignoreHiddenBlocks supprimé)
		if(!blocks[i].theBlock.CanControlShip) {
			currreason += "  CanControlShip not set\n";
			canAdd = false;
		}
		if(!blocks[i].theBlock.ControlThrusters) {
			currreason += "  can't ControlThrusters\n";
			canAdd = false;
		}
		if(blocks[i].theBlock.IsMainCockpit) {
			mainController = blocks[i];
		}
		if(!(greedy || hasTag(blocks[i].theBlock))) {
			currreason += "  Doesn't match my tag\n";
			canAdd = false;
		}
		if(this.removeTags) {
			removeTag(blocks[i].theBlock);
		}

		if(canAdd) {
			addSurfaceProvider(blocks[i].theBlock);
			usableControllers.Add(blocks[i]);
			if(this.applyTags) {
				addTag(blocks[i].theBlock);
			}
		} else {
			removeSurfaceProvider(blocks[i].theBlock);
			reason += currreason;
		}
	}
	if(blocks.Count == 0) {
		reason += "no controllers\n";
	}

	if(usableControllers.Count == 0) {
		Echo("ERREUR: aucun controleur de vaisseau utilisable trouve. Raison: \n");
		Echo(reason);
		return false;
	}

	controllers = blocks;
	return true;
}

public ShipController findACockpit() {
	foreach(ShipController cont in usableControllers) {
		if(cont.theBlock.IsWorking) {
			return cont;
		}
	}

	return null;
}

// Vérifie si la configuration des nacelles a changé
public bool checkNacelles() {
	Echo("Verification des Nacelles..");

	bool greedy = this.applyTags || this.removeTags;

	ShipController cont = findACockpit();
	if(cont == null) {
		Echo("Aucun cockpit enregistre, verification de tout.");
	} else if(!greedy) {
		MyShipMass shipmass = cont.theBlock.CalculateShipMass();
		if(this.oldMass == shipmass.BaseMass) {
			Echo("Mass is the same, everything is good.");

			// Le nom d'écran a peut-être été modifié pour correspondre au LCD VT
			getControllers();
			getScreens();
			return true;
		}
		Echo("Mass is different, checking everything.");
		this.oldMass = shipmass.BaseMass;
		// Une variation de masse peut invalider des surfaces; on nettoie les références fantômes
		this.surfaces.Clear();
	}

	List<ShipController> conts = new List<ShipController>();
	List<IMyMotorStator> rots = new List<IMyMotorStator>();
	List<IMyThrust> thrs = new List<IMyThrust>();
	List<IMyTextPanel> txts = new List<IMyTextPanel>();
	List<IMyProgrammableBlock> programBlocks = new List<IMyProgrammableBlock>();

	if(true) {//artificial scope :)
		List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
		GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks);
		for(int i = 0; i < blocks.Count; i++) {
			if(blocks[i] is IMyShipController) {
				conts.Add(new ShipController((IMyShipController)blocks[i]));
			}
			if(blocks[i] is IMyMotorStator) {
				rots.Add((IMyMotorStator)blocks[i]);
			}
			if(blocks[i] is IMyThrust) {
				thrs.Add((IMyThrust)blocks[i]);
			}
			if(blocks[i] is IMyTextPanel) {
				txts.Add((IMyTextPanel)blocks[i]);
			}
			if(blocks[i] is IMyProgrammableBlock) {
				programBlocks.Add((IMyProgrammableBlock)blocks[i]);
			}
		}
	}

	if(Me.SurfaceCount > 0) {
		surfaceProviderErrorStr = "";
		Me.CustomData = textSurfaceKeyword + 0;
		addSurfaceProvider(Me);
		Me.GetSurface(0).FontSize = 1.032f;// Ce n'est pas l'endroit idéal, mais c'est la solution légère ici
	}

	bool updateNacelles = false;

		// Attention: cette condition évite un verrouillage tardif des cockpits non-principaux
	if(/*(mainController != null ? !mainController.theBlock.IsMainCockpit : false) || */controllers.Count != conts.Count || cont == null || greedy) {
				Echo("Masse identique, configuration inchangée.");
				Echo("Masse modifiée, vérification complète.");
				Echo($"Compteur d'écrans ({screenCount}) incohérent (actuel: {txts.Count})");
		Echo($"Compteur de controleurs ({controllers.Count}) est incorrect (actuel: {conts.Count})");
		if(!getControllers(conts)) {
			return false;
		}
	}

	if(screenCount != txts.Count || greedy) {
		Echo($"Screen count ({screenCount}) is out of whack (current: {txts.Count})");
		getScreens(txts);
	} else {
		//probably may-aswell just getScreens either way. seems like there wouldn't be much performance hit
		// On pourrait appeler getScreens systématiquement: l'impact perf serait faible
		foreach(IMyTextPanel screen in txts) {
			if(!screen.IsWorking) continue;
			if(!ContainsIgnoreCase(screen.CustomName, LCDName)) continue;
			getScreens(txts);
		}
		Echo("Configuration des nacelles OK.");
	}
	Echo("Initialisation..");

	if(rotorCount != rots.Count) {
		Echo($"Compteur de rotors ({rotorCount}) est incorrect (actuel: {rots.Count})");
		updateNacelles = true;
	}

	var rotorHeads = new List<IMyAttachableTopBlock>();
	foreach(IMyMotorStator rotor in rots) {
		if(rotor.Top != null) {
			rotorHeads.Add(rotor.Top);
		}
	}
	if(rotorTopCount != rotorHeads.Count) {
		Echo($"Compteur de tetes de rotors ({rotorTopCount}) est incorrect (actuel: {rotorHeads.Count})");
		Echo($"Rotors: {rots.Count}");
		updateNacelles = true;
	}

	if(thrusterCount != thrs.Count) {
		Echo($"Compteur de propulseurs ({thrusterCount}) est incorrect (actuel: {thrs.Count})");
		updateNacelles = true;
	}


	if(updateNacelles || greedy) {
		Echo("Mise a jour des Nacelles");
		normalThrusters = thrs;
		getNacelles(rots, normalThrusters);
	} else {
		Echo("They seem fine.");
	}

	return true;
}

public bool init() {
	Echo("Initialising..");
	getNacelles();
	List<IMyShipController> conts = new List<IMyShipController>();
	GridTerminalSystem.GetBlocksOfType<IMyShipController>(conts);
	if(!getControllers(conts)) {
		Echo("Init failed.");
		Echo("Initialisation échouée.");
		return false;
	}
	Echo("Init success.");
	Echo("Initialisation réussie.");
	return true;
}

//addTag(IMyTerminalBlock block)
//removeTag(IMyTerminalBlock block)
//standbyTag(IMyTerminalBlock block)
//activeTag(IMyTerminalBlock block)

// Récupère tous les rotors et propulseurs
void getNacelles() {
	var blocks = new List<IMyTerminalBlock>();

	// Un seul appel au GridTerminalSystem
	GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => (block is IMyThrust) || (block is IMyMotorStator));

	Echo("Recuperation des blocs pour propulseurs et rotors");
	// Ne garde que les blocs utiles
	var rotors = new List<IMyMotorStator>();
		// var thrusters = new List<IMyThrust>(); // non utilisé ici
	normalThrusters.Clear();
	// var thrusters = new List<IMyThrust>();
	for(int i = blocks.Count-1; i >= 0; i--) {
		if(blocks[i] is IMyThrust) {
			normalThrusters.Add((IMyThrust)blocks[i]);
		} else /*if(blocks[i] is IMyMotorStator) */{
			rotors.Add((IMyMotorStator)blocks[i]);
		}
		blocks.RemoveAt(i);
	}
	rotorCount = rotors.Count;
	thrusterCount = normalThrusters.Count;

	getNacelles(rotors, normalThrusters);
}
void getNacelles(List<IMyMotorStator> rotors, List<IMyThrust> thrusters) {
	bool greedy = this.applyTags || this.removeTags || this.greedy;
	gotNacellesCount++;
	this.nacelles.Clear();
	twoAxisNacelleCount = 0;

	Echo("Recuperation des Rotors");
	rotorTopCount = 0;
	List<IMyMotorStator> usableRotors = new List<IMyMotorStator>();
	foreach(IMyMotorStator current in rotors) {
		if(this.removeTags) {
			removeTag(current);
		}
		if(!(greedy || hasTag(current))) {
			continue;
		}
		if(current.Top == null) {
			continue;
		}
		rotorTopCount++;

		if(current.TopGrid == Me.CubeGrid) {
			continue;
		}
		usableRotors.Add(current);
	}

	Echo("Recuperation des Propulseurs");
	Dictionary<IMyCubeGrid, List<IMyThrust>> thrustersByGrid = new Dictionary<IMyCubeGrid, List<IMyThrust>>();
	for(int i = 0; i < thrusters.Count; i++) {
		IMyThrust t = thrusters[i];
		if(!(greedy || hasTag(t))) {
			continue;
		}
		if(this.removeTags) {
			removeTag(t);
		}
		if(!thrustersByGrid.ContainsKey(t.CubeGrid)) {
			thrustersByGrid[t.CubeGrid] = new List<IMyThrust>();
		}
		thrustersByGrid[t.CubeGrid].Add(t);
	}

	bool allowSingleAxis = axisMode != AxisMode.TwoAxisOnly;
	bool allowTwoAxis = axisMode != AxisMode.SingleAxisOnly;
	HashSet<IMyMotorStator> usedRotors = new HashSet<IMyMotorStator>();
	HashSet<IMyThrust> usedThrusters = new HashSet<IMyThrust>();

	if(allowTwoAxis) {
		foreach(IMyMotorStator parent in usableRotors) {
			if(usedRotors.Contains(parent) || parent.Top == null) {
				continue;
			}

			IMyMotorStator bestChild = null;
			int bestThrCount = 0;
			foreach(IMyMotorStator child in usableRotors) {
				if(child == parent || usedRotors.Contains(child) || child.Top == null) {
					continue;
				}
				if(child.CubeGrid != parent.TopGrid) {
					continue;
				}

				List<IMyThrust> childThrusters;
				if(!thrustersByGrid.TryGetValue(child.TopGrid, out childThrusters)) {
					continue;
				}

				int freeThrCount = 0;
				for(int k = 0; k < childThrusters.Count; k++) {
					if(!usedThrusters.Contains(childThrusters[k])) {
						freeThrCount++;
					}
				}
				if(freeThrCount > bestThrCount) {
					bestChild = child;
					bestThrCount = freeThrCount;
				}
			}

			if(bestChild == null || bestThrCount == 0) {
				continue;
			}

			Nacelle n = new Nacelle(new Rotor(parent, this), new Rotor(bestChild, this), this);
			List<IMyThrust> list = thrustersByGrid[bestChild.TopGrid];
			for(int k = 0; k < list.Count; k++) {
				IMyThrust thr = list[k];
				if(usedThrusters.Contains(thr)) {
					continue;
				}
				if(this.applyTags) {
					addTag(thr);
				}
				n.thrusters.Add(new Thruster(thr));
				usedThrusters.Add(thr);
			}

			if(n.thrusters.Count > 0) {
				if(this.applyTags) {
					addTag(parent);
					addTag(bestChild);
				}
				n.validateThrusters(jetpack);
				n.detectThrustDirection();
				this.nacelles.Add(n);
				usedRotors.Add(parent);
				usedRotors.Add(bestChild);
				twoAxisNacelleCount++;
			}
		}
	}

	if(allowSingleAxis) {
		foreach(IMyMotorStator current in usableRotors) {
			if(usedRotors.Contains(current)) {
				continue;
			}
			List<IMyThrust> list;
			if(!thrustersByGrid.TryGetValue(current.TopGrid, out list)) {
				continue;
			}

			Nacelle n = new Nacelle(new Rotor(current, this), this);
			for(int k = 0; k < list.Count; k++) {
				IMyThrust thr = list[k];
				if(usedThrusters.Contains(thr)) {
					continue;
				}
				if(this.applyTags) {
					addTag(thr);
				}
				n.thrusters.Add(new Thruster(thr));
				usedThrusters.Add(thr);
			}

			if(n.thrusters.Count == 0) {
				continue;
			}
			if(this.applyTags) {
				addTag(current);
			}
			n.validateThrusters(jetpack);
			n.detectThrustDirection();
			this.nacelles.Add(n);
			usedRotors.Add(current);
		}
	}

	normalThrusters.Clear();
	for(int i = 0; i < thrusters.Count; i++) {
		if(!usedThrusters.Contains(thrusters[i])) {
			normalThrusters.Add(thrusters[i]);
		}
	}

	// Après l'assignation des thrusters aux nacelles, la liste 'normalThrusters' contient bien
	// ceux qui ne sont pas sur rotors. On s'assure qu'ils sont dans l'état voulu (ON/OFF jetpack).
	setNormalThrustersEnabled(jetpack);

}

public float lerp(float a, float b, float cutoff) {
	float percent = a/b;
	percent -= cutoff;
	percent *= 1/(1-cutoff);
	if(percent > 1) {
		percent = 1;
	}
	if(percent < 0) {
		percent = 0;
	}
	return percent;
}

void displayNacelles(List<Nacelle> nacelles) {
	foreach(Nacelle n in nacelles) {
		Echo($"\nNom du Rotor: {n.rotor.theBlock.CustomName}");

		Echo("Propulseurs:");
		int i = 0;
		foreach(Thruster t in n.thrusters) {
			Echo($@"{i}: {t.theBlock.CustomName}");
			i++;
		}
	}
}

public class Nacelle {
	public String errStr;
	public String DTerrStr;
	public Program program;

	// Éléments physiques
	public Rotor rotor;
	public Rotor secondaryRotor;
	public IMyCubeGrid thrustGrid;
	public Vector3D pointDirectionLocal = Vector3D.Zero;
	public HashSet<Thruster> thrusters;// Tous les propulseurs
	public HashSet<Thruster> availableThrusters;// Sous-ensemble utilisable (ShowInTerminal)
	public HashSet<Thruster> activeThrusters;// Sous-ensemble orienté vers la poussée dominante

	public double thrustModifierAbove = 0.1;// Tolérance avant pleine poussée
	public double thrustModifierBelow = 0.1;// Tolérance avant coupure de poussée

	public bool oldJetpack = true;
	public Vector3D requiredVec = Vector3D.Zero;

	public float totalEffectiveThrust = 0;
	public int detectThrustCounter = 0;
	public Vector3D currDir = Vector3D.Zero;



	public Nacelle() {}// Éviter ce constructeur si l'instance doit être conservée
	public Nacelle(Rotor rotor, Program program) {
		this.program = program;
		this.rotor = rotor;
		this.secondaryRotor = null;
		this.thrusters = new HashSet<Thruster>();
		this.availableThrusters = new HashSet<Thruster>();
		this.activeThrusters = new HashSet<Thruster>();
		errStr = "";
		DTerrStr = "";
	}

	public Nacelle(Rotor rotor, Rotor secondaryRotor, Program program) {
		this.program = program;
		this.rotor = rotor;
		this.secondaryRotor = secondaryRotor;
		this.thrusters = new HashSet<Thruster>();
		this.availableThrusters = new HashSet<Thruster>();
		this.activeThrusters = new HashSet<Thruster>();
		errStr = "";
		DTerrStr = "";
	}

	public bool hasSecondaryRotor() {
		return secondaryRotor != null && secondaryRotor.theBlock != null;
	}

	Rotor getControlRotor() {
		return hasSecondaryRotor() ? secondaryRotor : rotor;
	}

	IMyCubeGrid getThrustGrid() {
		if(hasSecondaryRotor() && secondaryRotor.theBlock.Top != null) {
			return secondaryRotor.theBlock.Top.CubeGrid;
		}
		if(rotor.theBlock.Top != null) {
			return rotor.theBlock.Top.CubeGrid;
		}
		return null;
	}

	// Calcul final et application aux composants physiques
	public void go(bool jetpack, bool dampeners, float shipMass) {
		bool primaryBroken = rotor.theBlock == null || !rotor.theBlock.IsFunctional || !rotor.theBlock.Enabled;
		bool secondaryBroken = hasSecondaryRotor() && (secondaryRotor.theBlock == null || !secondaryRotor.theBlock.IsFunctional || !secondaryRotor.theBlock.Enabled);
		if(primaryBroken || secondaryBroken) {
			foreach(Thruster thruster in thrusters) {
				if(thruster.theBlock == null || !thruster.theBlock.IsAlive()) continue;
				thruster.theBlock.Enabled = false;
				thruster.theBlock.ThrustOverride = 0f;
				thruster.IsOn = false;
			}
			activeThrusters.Clear();
			totalEffectiveThrust = 0;
			errStr = "=======Nacelle=======";
			return;
		}

		// Quand au moins un train est verrouillé, on fige la nacelle pour éviter les oscillations rotor.
		if(program.anyLandingGearLocked) {
			rotor.theBlock.TargetVelocityRPM = 0f;
			if(hasSecondaryRotor()) {
				secondaryRotor.theBlock.TargetVelocityRPM = 0f;
			}
			foreach(Thruster thruster in thrusters) {
				if(thruster.theBlock == null || !thruster.theBlock.IsAlive()) continue;
				thruster.setThrust(0);
				thruster.theBlock.Enabled = false;
				thruster.IsOffBecauseDampeners = true;
				thruster.IsOffBecauseJetpack = !jetpack;
			}
			totalEffectiveThrust = 0;
			return;
		}

		errStr = "=======Nacelle=======";
		/*errStr += $"\nactive thrusters: {activeThrusters.Count}";
		errStr += $"\nall thrusters: {thrusters.Count}";
		errStr += $"\nrequired force: {(int)requiredVec.Length()}N\n";*/
		totalEffectiveThrust = (float)calcTotalEffectiveThrust(activeThrusters);
		if(requiredVec.LengthSquared() < 0.0001f) {
			rotor.theBlock.TargetVelocityRPM = 0f;
			if(hasSecondaryRotor()) {
				secondaryRotor.theBlock.TargetVelocityRPM = 0f;
			}
		}

		double angleCos = 1;
		IMyCubeGrid endGrid = thrustGrid ?? getThrustGrid();
		if(endGrid != null && pointDirectionLocal.LengthSquared() > 0.01f) {
			Vector3D currentDir = Vector3D.TransformNormal(pointDirectionLocal, endGrid.WorldMatrix);
			if(hasSecondaryRotor()) {
				double c1 = rotor.setFromVecWithCurrent(requiredVec, currentDir, 0.75f);
				double c2 = secondaryRotor.setFromVecWithCurrent(requiredVec, currentDir, 1.25f);
				angleCos = Math.Min(c1, c2);
			} else {
				angleCos = rotor.setFromVec(requiredVec);
			}
		} else {
			angleCos = rotor.setFromVec(requiredVec);
		}
		/*errStr += $"\n=======rotor=======";
		errStr += $"\nname: '{rotor.theBlock.CustomName}'";
		errStr += $"\n{rotor.errStr}";
		errStr += $"\n-------rotor-------";*/


		// La valeur 'thrustModifier' définit l'écart angulaire toléré tout en gardant la poussée max.
		// Si 'thrustModifier' = 1, la poussée cible max reste atteinte jusqu'à 90° de la direction voulue.
		// Si 'thrustModifier' = 0, la poussée cible max n'est atteinte qu'en alignement parfait.
		// double thrustOffset = (angleCos + 1) / (1 + (1 - Program.thrustModifierAbove));//put it in some graphing calculator software where 'angleCos' is cos(x) and adjust the thrustModifier value between 0 and 1, then you can visualise it
		double abo = thrustModifierAbove;
		double bel = thrustModifierBelow;
		if(abo > 1) { abo = 1; }
		if(abo < 0) { abo = 0; }
		if(bel > 1) { bel = 1; }
		if(bel < 0) { bel = 0; }
		// Astuce: visualiser la formule sur un grapheur avec angleCos = cos(x)
		double thrustOffset = ((((angleCos + 1) * (1 + bel)) / 2) - bel) * (((angleCos + 1) * (1 + abo)) / 2);// the other one is simpler, but this one performs better
		// double thrustOffset = (angleCos * (1 + abo) * (1 + bel) + abo - bel + 1) / 2;
		if(thrustOffset > 1) {
			thrustOffset = 1;
		} else if(thrustOffset < 0) {
			thrustOffset = 0;
		}

		// Applique la poussée à chaque propulseur actif
		foreach(Thruster thruster in activeThrusters) {
			Vector3D thrust = thrustOffset * requiredVec * thruster.theBlock.MaxEffectiveThrust / totalEffectiveThrust;
			bool noThrust = thrust.LengthSquared() < 0.001f;
			if(!jetpack || !program.thrustOn || noThrust) {
				thruster.setThrust(0);
				thruster.theBlock.Enabled = false;
				thruster.IsOffBecauseDampeners = !program.thrustOn || noThrust;
				thruster.IsOffBecauseJetpack = !jetpack;
			} else {
				thruster.setThrust(thrust);
				thruster.theBlock.Enabled = true;
				thruster.IsOffBecauseDampeners = false;
				thruster.IsOffBecauseJetpack = false;
			}
		}
		oldJetpack = jetpack;
	}

	public float calcTotalEffectiveThrust(IEnumerable<Thruster> thrusters) {
		float total = 0;
		foreach(Thruster t in thrusters) {
			total += t.theBlock.MaxEffectiveThrust;
		}
		return total;
	}


	// Vrai si tous les propulseurs sont valides
	public bool validateThrusters(bool jetpack) {
		bool needsUpdate = false;
		errStr += $"validating thrusters: (jetpack {jetpack})\n";
		foreach(Thruster curr in thrusters) {

			bool shownAndFunctional = curr.theBlock.ShowInTerminal && curr.theBlock.IsFunctional;
			if(availableThrusters.Contains(curr)) {// déjà disponible
				errStr += "in available thrusters\n";

				bool wasOnAndIsNowOff = curr.IsOn && !curr.theBlock.Enabled && !curr.IsOffBecauseJetpack && !curr.IsOffBecauseDampeners;

				if((!shownAndFunctional || wasOnAndIsNowOff) && (jetpack && oldJetpack)) {
					// Si jetpack est actif, un arrêt externe doit retirer le propulseur du groupe
					// Si jetpack est inactif, il peut rester dans le groupe

					curr.IsOn = false;
					// Retire le propulseur
					availableThrusters.Remove(curr);
					needsUpdate = true;
				}

			} else {// non disponible
				errStr += "not in available thrusters\n";
				errStr += $"ShowInTerminal {curr.theBlock.ShowInTerminal}\n";
				errStr += $"IsWorking {curr.theBlock.IsWorking}\n";
				errStr += $"IsFunctional {curr.theBlock.IsFunctional}\n";

				bool wasOffAndIsNowOn = !curr.IsOn && curr.theBlock.Enabled;
				if(shownAndFunctional && wasOffAndIsNowOn) {
					availableThrusters.Add(curr);
					needsUpdate = true;
					curr.IsOn = true;
				}
			}
		}
		return !needsUpdate;
	}

	public void detectThrustDirection() {
		detectThrustCounter++;
		Vector3D engineDirection = Vector3D.Zero;
		Vector3D engineDirectionNeg = Vector3D.Zero;
		Vector3I thrustDir = Vector3I.Zero;
		Rotor controlRotor = getControlRotor();
		if(controlRotor == null || controlRotor.theBlock == null || controlRotor.theBlock.Top == null) {
			return;
		}
		thrustGrid = getThrustGrid();
		Base6Directions.Direction rotTopUp = controlRotor.theBlock.Top.Orientation.Up;

		// Additionne la poussée effective de tous les propulseurs disponibles
		foreach(Thruster t in availableThrusters) {
			Base6Directions.Direction thrustForward = t.theBlock.Orientation.Forward; // L'échappement pointe dans ce sens

			// Si ce n'est ni dans l'axe haut ni bas du rotor
			if(!(thrustForward == rotTopUp || thrustForward == Base6Directions.GetFlippedDirection(rotTopUp))) {
				// L'ajoute au cumul
				var thrustForwardVec = Base6Directions.GetVector(thrustForward);
				if(thrustForwardVec.X < 0 || thrustForwardVec.Y < 0 || thrustForwardVec.Z < 0) {
					engineDirectionNeg += Base6Directions.GetVector(thrustForward) * t.theBlock.MaxEffectiveThrust;
				} else {
					engineDirection += Base6Directions.GetVector(thrustForward) * t.theBlock.MaxEffectiveThrust;
				}
			}
		}

		// Retient la direction unique la plus puissante
		double max = Math.Max(engineDirection.Z, Math.Max(engineDirection.X, engineDirection.Y));
		double min = Math.Min(engineDirectionNeg.Z, Math.Min(engineDirectionNeg.X, engineDirectionNeg.Y));
		double maxAbs = 0;
		if(max > -1*min) {
			maxAbs = max;
		} else {
			maxAbs = min;
		}

		// TODO: swap onbool for each thruster that isn't in this
		float DELTA = 0.1f;
		if(Math.Abs(maxAbs - engineDirection.X) < DELTA) {
			thrustDir.X = 1;
		} else if(Math.Abs(maxAbs - engineDirection.Y) < DELTA) {
			thrustDir.Y = 1;
		} else if(Math.Abs(maxAbs - engineDirection.Z) < DELTA) {
			thrustDir.Z = 1;
		} else if(Math.Abs(maxAbs - engineDirectionNeg.X) < DELTA) {
			// DTerrStr += $"\nengineDirectionNeg.X";
			thrustDir.X = -1;
		} else if(Math.Abs(maxAbs - engineDirectionNeg.Y) < DELTA) {
			// DTerrStr += $"\nengineDirectionNeg.Y";
			thrustDir.Y = -1;
		} else if(Math.Abs(maxAbs - engineDirectionNeg.Z) < DELTA) {
			// DTerrStr += $"\nengineDirectionNeg.Z";
			thrustDir.Z = -1;
		} else {
			// DTerrStr += $"\nERROR (detectThrustDirection):\nmaxAbs doesn't match any engineDirection\n{maxAbs}\n{engineDirection}\n{engineDirectionNeg}";
			return;
		}

		// Utilise thrustDir pour définir l'orientation cible
		pointDirectionLocal = (Vector3D)thrustDir;
		rotor.setPointDir(pointDirectionLocal);
		if(hasSecondaryRotor()) {
			secondaryRotor.setPointDir(pointDirectionLocal);
		}
		// Base6Directions.Direction rotTopForward = rotor.theBlock.Top.Orientation.TransformDirection(Base6Directions.Direction.Forward);
		// Base6Directions.Direction rotTopLeft = rotor.theBlock.Top.Orientation.TransformDirection(Base6Directions.Direction.Left);
		// rotor.offset = (float)Math.Acos(rotor.angleBetweenCos(Base6Directions.GetVector(rotTopForward), (Vector3D)thrustDir));

		// Désambiguïsation (désactivée)
		// if(false && Math.Acos(rotor.angleBetweenCos(Base6Directions.GetVector(rotTopLeft), (Vector3D)thrustDir)) > Math.PI/2) {
			// rotor.offset += (float)Math.PI;
		// 	rotor.offset = (float)(2*Math.PI - rotor.offset);
		// }

		foreach(Thruster t in thrusters) {
			t.theBlock.Enabled = false;
			t.IsOn = false;
		}
		activeThrusters.Clear();

		// Place les propulseurs dans la liste active
		Base6Directions.Direction thrDir = Base6Directions.GetDirection(thrustDir);
		foreach(Thruster t in availableThrusters) {
			Base6Directions.Direction thrustForward = t.theBlock.Orientation.Forward; // Exhaust goes this way

			if(thrDir == thrustForward) {
				t.theBlock.Enabled = true;
				t.IsOn = true;
				activeThrusters.Add(t);
			}
		}
	}

}

public class Thruster : BlockWrapper<IMyThrust> {

	// Reste inchangé en veille; hors veille, reflète l'état ON/OFF du propulseur
	public bool IsOn;

	// Ces deux indicateurs signalent un arrêt demandé par script
	public bool IsOffBecauseDampeners = true;
	public bool IsOffBecauseJetpack = true;

	public string errStr = "";

	public Thruster(IMyThrust thruster) : base(thruster) {
		// this.IsOn = theBlock.Enabled;
		this.IsOn = false;
		this.theBlock.Enabled = true;
	}

	// Définit la poussée en newtons (N)
	// thrustVec est en espace monde; sa norme correspond à la poussée désirée
	public void setThrust(Vector3D thrustVec) {
		setThrust(thrustVec.Length());
	}

	// Définit la poussée en newtons (N)
	public void setThrust(double thrust) {
		errStr = "";
		/*errStr += $"\ntheBlock.Enabled: {theBlock.Enabled.toString()}";
		errStr += $"\nIsOffBecauseDampeners: {IsOffBecauseDampeners.toString()}";
		errStr += $"\nIsOffBecauseJetpack: {IsOffBecauseJetpack.toString()}";*/

		if(thrust > theBlock.MaxThrust) {
			thrust = theBlock.MaxThrust;
			// errStr += $"\nExceeding max thrust";
		} else if(thrust < 0) {
			// errStr += $"\nNegative Thrust";
			thrust = 0;
		}

		theBlock.ThrustOverride = (float)(thrust * theBlock.MaxThrust / theBlock.MaxEffectiveThrust);
		/*errStr += $"\nEffective {(100*theBlock.MaxEffectiveThrust / theBlock.MaxThrust).Round(1)}%";
		errStr += $"\nOverride {theBlock.ThrustOverride}N";*/
	}
}

public class Rotor : BlockWrapper<IMyMotorStator> {
	// IMyMotorBase inclut aussi les roues, on garde IMyMotorStator

	public Program program;
	public Vector3D direction = Vector3D.Zero;//offset relative to the head

	public string errStr = "";
	float maxRPM;

	public Rotor(IMyMotorStator rotor, Program program) : base(rotor) {
		this.program = program;

		if(program.maxRotorRPM <= 0) {
			maxRPM = rotor.GetMaximum<float>("Velocity");
		} else {
			maxRPM = program.maxRotorRPM;
		}
	}

	public void setPointDir(Vector3D dir) {
		this.direction = dir;
	}

	/*===| Part of Rotation By Equinox on the KSH discord channel. |===*/
	private void PointRotorAtVector(IMyMotorStator rotor, Vector3D targetDirection, Vector3D currentDirection, float multiplier) {
		double errorScale = Math.PI * maxRPM;

		Vector3D angle = Vector3D.Cross(targetDirection, currentDirection);
		double err = Vector3D.Dot(angle, rotor.WorldMatrix.Up);

		double rpm = err * errorScale * multiplier;

		// ZONE MORTE DYNAMIQUE
		double speed = program.shipVelocity.Length();
		bool lowSpeedNoInput = !program.pilotInputActive && speed < Program.lowSpeedNoInputThreshold;
		double zoneMorte;
		if(lowSpeedNoInput) {
			zoneMorte = Program.rotorDeadzoneNoInputLow; // ~8° en stabilisation passive proche de l'arrêt
		} else if(speed < 0.30) {
			zoneMorte = Program.rotorDeadzoneLow; // ~5.2° proche de l'arrêt
		} else if(speed < 1.0) {
			zoneMorte = Program.rotorDeadzoneSlow; // ~3° si lent
		} else {
			zoneMorte = Program.rotorDeadzoneNormal; // ~0.6° en vitesse normale
		}

		if (Math.Abs(err) < zoneMorte) {
			rotor.TargetVelocityRPM = 0f;
		} else {
			double amortissement = lowSpeedNoInput ? Program.rotorDampingNoInputLow : Program.rotorDampingDefault;
			double vitesseActuelle = rotor.TargetVelocityRPM;
			double vitesseCible = rpm;
			double nouvelleVitesse = (1 - amortissement) * vitesseCible + amortissement * vitesseActuelle;

			// Anti-butée: ralentit et bloque la commande qui pousse hors limites.
			double softZoneRad = Program.limitSoftZoneDeg * Math.PI / 180.0;
			double hardZoneRad = Program.limitHardZoneDeg * Math.PI / 180.0;
			double angleNow = rotor.Angle;
			float lowerLimitDeg = rotor.LowerLimitDeg;
			float upperLimitDeg = rotor.UpperLimitDeg;
			bool hasLowerLimit = lowerLimitDeg > -360.5f;
			bool hasUpperLimit = upperLimitDeg < 360.5f;
			if(hasLowerLimit || hasUpperLimit) {
				double lowerLimitRad = hasLowerLimit ? lowerLimitDeg * Math.PI / 180.0 : -9999;
				double upperLimitRad = hasUpperLimit ? upperLimitDeg * Math.PI / 180.0 : 9999;

				if(hasLowerLimit && nouvelleVitesse < 0) {
					double distanceToLower = angleNow - lowerLimitRad;
					if(distanceToLower <= hardZoneRad) {
						nouvelleVitesse = 0;
					} else if(distanceToLower < softZoneRad) {
						double scale = (distanceToLower - hardZoneRad) / (softZoneRad - hardZoneRad);
						if(scale < 0) scale = 0;
						if(scale > 1) scale = 1;
						nouvelleVitesse *= scale;
					}
				}

				if(hasUpperLimit && nouvelleVitesse > 0) {
					double distanceToUpper = upperLimitRad - angleNow;
					if(distanceToUpper <= hardZoneRad) {
						nouvelleVitesse = 0;
					} else if(distanceToUpper < softZoneRad) {
						double scale = (distanceToUpper - hardZoneRad) / (softZoneRad - hardZoneRad);
						if(scale < 0) scale = 0;
						if(scale > 1) scale = 1;
						nouvelleVitesse *= scale;
					}
				}
			}

			double minRpmCommand = lowSpeedNoInput ? Program.rotorMinRpmNoInputLow : (speed < 0.30 ? Program.rotorMinRpmLow : Program.rotorMinRpmNormal);
			if (Math.Abs(nouvelleVitesse) < minRpmCommand) {
				rotor.TargetVelocityRPM = 0f;
			} else if (nouvelleVitesse > maxRPM) {
				rotor.TargetVelocityRPM = maxRPM;
			} else if ((nouvelleVitesse * -1) > maxRPM) {
				rotor.TargetVelocityRPM = maxRPM * -1;
			} else {
				rotor.TargetVelocityRPM = (float)nouvelleVitesse;
			}
		}
	}

	// Oriente le rotor vers la direction demandée en espace monde
	// desiredVec n'a pas besoin d'être coplanaire avec le plan de rotation
	public double setFromVec(Vector3D desiredVec, float multiplier) {
		errStr = "";
		desiredVec.Normalize();
		Vector3D currentDir = Vector3D.TransformNormal(this.direction, theBlock.Top.CubeGrid.WorldMatrix);
		PointRotorAtVector(theBlock, desiredVec, currentDir, multiplier);

		return angleBetweenCos(currentDir, desiredVec, desiredVec.Length());
	}

	public double setFromVecWithCurrent(Vector3D desiredVec, Vector3D currentDir, float multiplier) {
		errStr = "";
		if(desiredVec.LengthSquared() < 0.0001 || currentDir.LengthSquared() < 0.0001) {
			theBlock.TargetVelocityRPM = 0f;
			return 1;
		}
		desiredVec.Normalize();
		currentDir.Normalize();
		PointRotorAtVector(theBlock, desiredVec, currentDir, multiplier);
		return angleBetweenCos(currentDir, desiredVec, desiredVec.Length());
	}

	public double setFromVec(Vector3D desiredVec) {
		return setFromVec(desiredVec, 1);
	}

	// Retourne le cosinus de l'angle entre deux vecteurs
	// Utiliser Acos pour obtenir l'angle
	public double angleBetweenCos(Vector3D a, Vector3D b) {
		double dot = Vector3D.Dot(a, b);
		double Length = a.Length() * b.Length();
		return dot/Length;
	}

	// Retourne le cosinus de l'angle entre deux vecteurs
	// Utiliser Acos pour obtenir l'angle
	// N'effectue pas le calcul de longueur (optimisation)
	public double angleBetweenCos(Vector3D a, Vector3D b, double len_a_times_len_b) {
		double dot = Vector3D.Dot(a, b);
		return dot/len_a_times_len_b;
	}

}
public class ShipController : BlockWrapper<IMyShipController> {
	public bool lastDampener;


	public ShipController(IMyShipController theBlock) : base(theBlock) {
		lastDampener = theBlock.DampenersOverride;
	}

	public void setDampener(bool val) {
		lastDampener = val;
		theBlock.DampenersOverride = val;
	}

}

public interface IBlockWrapper
{
    IMyTerminalBlock theBlock { get; set; }
}

public abstract class BlockWrapper<T>: IBlockWrapper where T: class, IMyTerminalBlock
{
    public T theBlock { get; set; }

    public BlockWrapper(T block) {
    	theBlock = block;
    }

    // not allowed for some reason
    //public static implicit operator IMyTerminalBlock(BlockWrapper<T> wrap) => wrap.theBlock;

    IMyTerminalBlock IBlockWrapper.theBlock
    {
        get { return theBlock; }
        set { theBlock = (T)value; }
    }
}



}
public static class CustomProgramExtensions {

	public static bool IsAlive(this IMyTerminalBlock block) {
		return block.CubeGrid.GetCubeBlock(block.Position)?.FatBlock == block;
	}

	// Projette a sur b
	public static Vector3D project(this Vector3D a, Vector3D b) {
		double aDotB = Vector3D.Dot(a, b);
		double bDotB = Vector3D.Dot(b, b);
		return b * aDotB / bDotB;
	}

	public static Vector3D reject(this Vector3D a, Vector3D b) {
		return Vector3D.Reject(a, b);
	}

	public static Vector3D normalized(this Vector3D vec) {
		return Vector3D.Normalize(vec);
	}

	public static double dot(this Vector3D a, Vector3D b) {
		return Vector3D.Dot(a, b);
	}

	// Convertit le mouvement local en espace monde
	public static Vector3D getWorldMoveIndicator(this IMyShipController cont) {
		return Vector3D.TransformNormal(cont.MoveIndicator, cont.WorldMatrix);
	}


	public static string progressBar(this double val) {
		char[] bar = {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '};
		for(int i = 0; i < 10; i++) {
			if(i <= val * 10) {
				bar[i] = '|';
			}
		}
		var str_build = new StringBuilder("[");
		for(int i = 0; i < 10; i++) {
			str_build.Append(bar[i]);
		}
		str_build.Append("]");
		return str_build.ToString();
	}

	public static string progressBar(this float val) {
		return ((double)val).progressBar();
	}

	public static string progressBar(this Vector3D val) {
		return val.Length().progressBar();
	}


	public static Vector3D Round(this Vector3D vec, int num) {
		return Vector3D.Round(vec, num);
	}

	public static double Round(this double val, int num) {
		return Math.Round(val, num);
	}

	public static float Round(this float val, int num) {
		return (float)Math.Round(val, num);
	}

	public static String toString(this Vector3D val) {
		return $"X:{val.X} Y:{val.Y} Z:{val.Z}";
	}

	public static String toString(this Vector3D val, bool pretty) {
		if(!pretty)
			return val.toString();
		else
			return $"X:{val.X}\nY:{val.Y}\nZ:{val.Z}\n";
	}

	public static String toString(this bool val) {
		if(val) {
			return "true";
		}
		return "false";
	}
	