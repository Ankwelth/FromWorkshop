//-------------------------------------
// NAV - Gives navigation information on LCD.
// SpaceEngineers version 1.202.1 (2023/05/11).
//
// Requires:
//  - One remote control (first found will be used).
//  - At least one text panel (LCD, Cockpit...) with '[NAV]' tag in name.
//  - If the "NAV" tag interfers with other scripts, you can change it in 'tag' user variable.
//
// Usage:
//  - Choosing text panel and diplay format:
//    For blocks with several text panels, you can select it/them using their indexes in the [NAV] tag.
//    Two display modes can be selected depending of the size of the display panel:
//     - Use ':' as separator for regular display (square text panels, such as LCDs or main cockpit panels).
//     - Use '.' as separator for condensed display (usually secondary cockpit panels).
//    Eg: "Cockpit [NAV:1]" will use center panel with regular display mode.
//        "Cockpit [NAV.0.3]" will use both left and right panels with condensed diplay mode.
//        "Cockpit [NAV]" will use '0' as default panel, ie: left one for cockpit.
//  - Use command "Calibrate" to trigger a new calibration (ie: planet center, latitute and longitude).
//  - During calibration, fly your ship far enough, so the algorithm can use the position and gravity
//    to compute the planet center location. (Around 1.5km on earth with default calibrationAngle.)
//  - If you want more accurate calibration, you can increase the 'calibrationAngle' variable.
//
//-------------------------------------
//
// Thanks:
//  - Compass and coordinates algorithm by Pennywise.
//      (https://steamcommunity.com/sharedfiles/filedetails/?id=558981481)
//
//--- User constant -------------------

const String tag = "NAV";
const double calibrationAngle = 0.025;

//-------------------------------------

const int navVersion = 1;
const string navFullVersion = "1.2";

const string calibrateCmd = "calibrate";

int rescanCount = 0;
const int rescanTrigger = 10;
enum DisplayMode { regular, condensed };
struct TextSurface { public IMyTextSurface myTextSurface; public DisplayMode displayMode; };
List<TextSurface> textSurfaces = new List<TextSurface>();
List<IMyTerminalBlock> terminalBlocks = new List<IMyTerminalBlock>();
const string tagPattern = ".*\\[" + tag + "([:\\.][0-9])*\\].*";
System.Text.RegularExpressions.Regex RegexBlockName = new System.Text.RegularExpressions.Regex ( tagPattern );

IMyRemoteControl remoteCtrl = null;
List<IMyRemoteControl> remoteCtrls = new List<IMyRemoteControl>();

enum NavState { notCalibrated, calibrating, calibrated };
NavState navState = NavState.notCalibrated;
bool displayCalibration = true;
int displayCalibrationCount = 0;
const int displayCalibrationTrigger = 5;
const string navCalibrating = "      --- Calibrating ---      ";
int calibrationPercent;
string calibrationText;

const string compassTop    = "---------------▼---------------\n";
const string compassBottom = "---------------▲---------------\n";
const string textSpacing   = "                               ";
const string compassCard = "               N                     NE                     E                     SE                     S                     SW                     W                     NW                     N               ";
const string compassGrid = "30--340--350--000--010--020--030--040--050--060--070--080--090--100--110--120--130--140--150--160--170--180--190--200--210--220--230--240--250--260--270--280--290--300--310--320--330--340--350--000--010--020--03";

double bestAlpha;
Vector3D calibrationPosition;
Vector3D calibrationGravity;

Vector3D planetCenter;
double bearing;
double altitude;
double latitude;
double longitude;

public Program ()
{
    Echo ( "NAV v" + navFullVersion );
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    LoadStorage ();
}

void Save ()
{
    Storage = GetSaveString ();
}

void Main ( string argument )
{
    UpdateTextSurfaces ();
    if ( textSurfaces.Count == 0 )
    {
        Echo ( tag + ": No text panel found." );
        return;
    }

    UpdateRemoteControl ();
    if ( remoteCtrl == null )
    {
        string errorText = tag + ": No remote control found.";
        DisplayText ( errorText, errorText );
        return;
    }

    if ( ( navState == NavState.notCalibrated )
        || ( argument == calibrateCmd && navState != NavState.calibrating ) )
    {
        ResetCalibration ();
    }

    Vector3D position = remoteCtrl.GetPosition ();
    Vector3D gravity = remoteCtrl.GetNaturalGravity ();
    Vector3D normalizedGravity = Vector3D.Normalize ( gravity );

    if ( navState == NavState.calibrating )
    {
        Calibrate ( position, gravity, normalizedGravity );
    }

    ComputeBearing ( normalizedGravity );
    ComputeAltLatLong ( position );

    string regularText = GetDisplayText ( position, DisplayMode.regular );
    string condensedText = GetDisplayText ( position, DisplayMode.condensed );
    DisplayText ( regularText, condensedText );
}

void LoadStorage ()
{
    if ( Storage.Length == 0 )
    {
        return;
    }

    var fields = Storage.Split ( ';' );
    if ( Convert.ToInt32 ( fields [ 0 ] ) != navVersion )
    {
        return;
    }

    navState = ( NavState ) Convert.ToInt32 ( fields [ 1 ] );
    bestAlpha = Convert.ToDouble ( fields [ 2 ] );

    calibrationPosition.X = Convert.ToDouble ( fields [ 3 ] );
    calibrationPosition.Y = Convert.ToDouble ( fields [ 4 ] );
    calibrationPosition.Z = Convert.ToDouble ( fields [ 5 ] );

    calibrationGravity.X = Convert.ToDouble ( fields [ 6 ] );
    calibrationGravity.Y = Convert.ToDouble ( fields [ 7 ] );
    calibrationGravity.Z = Convert.ToDouble ( fields [ 8 ] );

    planetCenter.X = Convert.ToDouble ( fields [ 9 ] );
    planetCenter.Y = Convert.ToDouble ( fields [ 10 ] );
    planetCenter.Z = Convert.ToDouble ( fields [ 11 ] );
}

string GetSaveString ()
{
    return navVersion.ToString () + ";"
        + ( int ) navState + ";"
        + bestAlpha + ";"
        + Vector3DToSaveString ( calibrationPosition ) + ";"
        + Vector3DToSaveString ( calibrationGravity ) + ";"
        + Vector3DToSaveString ( planetCenter );
}

string Vector3DToSaveString ( Vector3D vector )
{
    return vector.X + ";" + vector.Y + ";" + vector.Z;
}

void UpdateTextSurfaces ()
{
    if ( rescanCount < rescanTrigger && textSurfaces.Count != 0 )
    {
        rescanCount ++;
        return;
    }

    rescanCount = 0;
    textSurfaces.RemoveAll ( item => true );

    GridTerminalSystem.GetBlocksOfType ( terminalBlocks );
    foreach ( IMyTerminalBlock terminalBlock in terminalBlocks )
    {
        var textSurfaceProvider = terminalBlock as IMyTextSurfaceProvider;
        if ( textSurfaceProvider != null )
        {
            System.Text.RegularExpressions.Match match = RegexBlockName.Match ( terminalBlock.CustomName );
            if ( match.Success && match.Groups.Count == 2 )
            {
                System.Text.RegularExpressions.Group group = match.Groups [ 1 ];
                System.Text.RegularExpressions.CaptureCollection collection = group.Captures;
                if ( collection.Count == 0 )
                {
                    AddTextSurface ( textSurfaceProvider, 0, DisplayMode.regular );
                }
                else
                {
                    for ( int i = 0; i < collection.Count; i ++ ) 
                    {
                        System.Text.RegularExpressions.Capture capture = collection [ i ];
                        string captureString = capture.ToString ();

                        DisplayMode displayMode = DisplayMode.regular;
                        if ( captureString [ 0 ] == '.' )
                        {
                            displayMode = DisplayMode.condensed;
                        }

                        int index = Convert.ToInt32 ( captureString.Trim ( ':', '.' ) );
                        if ( index < textSurfaceProvider.SurfaceCount )
                        {
                            AddTextSurface ( textSurfaceProvider, index, displayMode );
                        }
                    }
                }
            }
        }
    }
}

void AddTextSurface ( IMyTextSurfaceProvider textSurfaceProvider, int index, DisplayMode displayMode )
{
    IMyTextSurface myTextSurface = textSurfaceProvider.GetSurface ( index );
    myTextSurface.ContentType = ContentType.TEXT_AND_IMAGE;
    myTextSurface.Font = "Monospace";
    myTextSurface.FontSize = 0.82F;

    TextSurface textSurface;
    textSurface.myTextSurface = myTextSurface;
    textSurface.displayMode = displayMode;
    textSurfaces.Add ( textSurface );
}

void UpdateRemoteControl ()
{
    if ( remoteCtrl == null )
    {
        GridTerminalSystem.GetBlocksOfType ( remoteCtrls );
        if ( remoteCtrls.Count != 0 )
        {
            remoteCtrl = remoteCtrls [ 0 ];
        }
    }
}

void ResetCalibration ()
{
    navState = NavState.calibrating;

    calibrationPercent = 0;
    calibrationText = navCalibrating;

    bestAlpha = 0;
    calibrationPosition = remoteCtrl.GetPosition ();
    calibrationGravity = remoteCtrl.GetNaturalGravity ();

    planetCenter.X = 0;
    planetCenter.Y = 0;
    planetCenter.Z = 0;
    altitude = 0;
    latitude = 0;
    longitude = 0;
}

void Calibrate ( Vector3D position, Vector3D gravity, Vector3D normalizedGravity )
{
    double alpha = Math.Acos ( Vector3D.Dot ( normalizedGravity, Vector3D.Normalize ( calibrationGravity ) ) );

    if ( alpha > bestAlpha )
    {
        bestAlpha = alpha;
        ComputePlanetCenter ( calibrationPosition, calibrationPosition + calibrationGravity, position, position + gravity );
    }

    if ( alpha > calibrationAngle )
    {
        navState = NavState.calibrated;
        return;
    }

    UpdateCalibrationText ( ( int ) ( alpha * 100 / calibrationAngle ) );
}

void ComputePlanetCenter ( Vector3D position1, Vector3D gravity1, Vector3D position2, Vector3D gravity2 )
{
    double aX = position1.X;
    double aY = position1.Y;
    double aZ = position1.Z;
    double bX = gravity1.X;
    double bY = gravity1.Y;
    double bZ = gravity1.Z;

    double cX = position2.X;
    double cY = position2.Y;
    double cZ = position2.Z;
    double sX = gravity2.X;
    double sY = gravity2.Y;
    double sZ = gravity2.Z;

    double AB2   = ( bX - aX ) * ( bX - aX ) + ( bY - aY ) * ( bY - aY ) + ( bZ - aZ ) * ( bZ - aZ );
    double SCxAB = ( cX - sX ) * ( bX - aX ) + ( cY - sY ) * ( bY - aY ) + ( cZ - sZ ) * ( bZ - aZ );
    double ASxAB = ( sX - aX ) * ( bX - aX ) + ( sY - aY ) * ( bY - aY ) + ( sZ - aZ ) * ( bZ - aZ );
    double ABxSC = ( bX - aX ) * ( cX - sX ) + ( bY - aY ) * ( cY - sY ) + ( bZ - aZ ) * ( cZ - sZ );
    double SC2   = ( cX - sX ) * ( cX - sX ) + ( cY - sY ) * ( cY - sY ) + ( cZ - sZ ) * ( cZ - sZ );
    double ASxSC = ( sX - aX ) * ( cX - sX ) + ( sY - aY ) * ( cY - sY ) + ( sZ - aZ ) * ( cZ - sZ );

    double Mk = ( ASxAB * SC2 - ASxSC * SCxAB ) / ( AB2 * SC2 - ABxSC * SCxAB );
    double Nk = ( AB2 * ASxSC - ABxSC * ASxAB ) / ( AB2 * SC2 - ABxSC * SCxAB );

    double mX = aX + ( bX - aX ) * Mk;
    double mY = aY + ( bY - aY ) * Mk;
    double mZ = aZ + ( bZ - aZ ) * Mk;

    double nX = sX + ( sX - cX ) * Nk;
    double nY = sY + ( sY - cY ) * Nk;
    double nZ = sZ + ( sZ - cZ ) * Nk;

    planetCenter.X = ( mX + nX ) / 2;
    planetCenter.Y = ( mY + nY ) / 2;
    planetCenter.Z = ( mZ + nZ ) / 2;
}

void UpdateCalibrationText ( int percent )
{
    bool formatText = false;

    displayCalibrationCount ++;
    if ( displayCalibrationCount >= displayCalibrationTrigger )
    {
        displayCalibrationCount = 0;
        displayCalibration = !displayCalibration;
        formatText = true;
    }

    if ( percent > calibrationPercent )
    {
        calibrationPercent = percent;
        formatText = true;
    }

    if ( formatText )
    {
        string percentText = calibrationPercent.ToString () + "%";
        if ( displayCalibration )
        {
            calibrationText = navCalibrating.Substring ( 0, 31 - percentText.Length );
        }
        else
        {
            calibrationText = textSpacing.Substring ( 0, 31 - percentText.Length );
        }
        calibrationText += percentText;
    }
}

void ComputeBearing ( Vector3D normalizedGravity )
{
    Vector3D forwardReject = Vector3D.Normalize ( Vector3D.Reject ( remoteCtrl.WorldMatrix.Forward, normalizedGravity ) );
    Vector3D nordVector = Vector3D.Normalize ( Vector3D.Reject ( new Vector3D ( 0, -1, 0 ), normalizedGravity ) );

    Vector3D leftReject;
    if ( Math.Acos ( Vector3D.Dot ( remoteCtrl.WorldMatrix.Down, normalizedGravity ) ) < Math.PI / 2 )
    {
        leftReject = Vector3D.Reject ( remoteCtrl.WorldMatrix.Right, normalizedGravity );
    }
    else
    {
        leftReject = Vector3D.Reject ( remoteCtrl.WorldMatrix.Left, normalizedGravity );
    }

    if ( leftReject.GetDim ( 1 ) > 0 )
    {
        bearing = Math.Acos ( Vector3D.Dot ( forwardReject, nordVector ) ) * 180 / Math.PI;
    }
    else
    {
        bearing = 360 - Math.Acos ( Vector3D.Dot ( forwardReject, nordVector ) ) * 180 / Math.PI;
    }
}

void ComputeAltLatLong ( Vector3D position )
{
    Vector3D altitudeVector = position - planetCenter;
    Vector3D altitudeReject = Vector3D.Normalize ( new Vector3D ( altitudeVector.X, 0, altitudeVector.Z ) );

    altitude = altitudeVector.Length ();

    latitude = Math.Acos ( Vector3D.Dot ( Vector3D.Normalize ( altitudeVector ), altitudeReject ) ) * 180 / Math.PI;
    if ( altitudeVector.Y > 0 )
    {
        latitude = -latitude;
    }

    longitude = Math.Acos ( Vector3D.Dot ( altitudeReject, new Vector3D ( 0, 0, 1 ) ) ) * 180 / Math.PI;
    if ( altitudeReject.X < 0 )
    {
        longitude = -longitude;
    }
}

string GetDisplayText ( Vector3D position, DisplayMode displayMode )
{
    string text = GetCompassText ();
    text += "\n";

    text += "Altitude:    " + GetCalibratedText ( altitude, 0, 8 ) + " (*)\n";
    text += "Latitude:    " + GetCalibratedText ( latitude, 2, 8 ) + "\n";
    text += "Longitude:   " + GetCalibratedText ( longitude, 2, 8 ) + "\n";
    text += "\n";

    if ( displayMode == DisplayMode.condensed )
    {
        text += "Position:       Planet center:\n";
        text += "  X =" + AlignRight ( position.X, 0, 8 ) + "     X =" + GetCalibratedText ( planetCenter.X, 0, 8 ) + "\n";
        text += "  Y =" + AlignRight ( position.Y, 0, 8 ) + "     Y =" + GetCalibratedText ( planetCenter.Y, 0, 8 ) + "\n";
        text += "  Z =" + AlignRight ( position.Z, 0, 8 ) + "     Z =" + GetCalibratedText ( planetCenter.Z, 0, 8 ) + "\n";

        if ( navState != NavState.calibrating )
        {
            text += "\n";
            text += "(* from planet center)";
        }
    }
    else // DisplayMode.regular
    {
        text += "Position: X =" + AlignRight ( position.X, 0, 8 ) + "\n";
        text += "          Y =" + AlignRight ( position.Y, 0, 8 ) + "\n";
        text += "          Z =" + AlignRight ( position.Z, 0, 8 ) + "\n";
        text += "\n";

        text += "Planet center:\n";
        text += "          X =" + GetCalibratedText ( planetCenter.X, 0, 8 ) + "\n";
        text += "          Y =" + GetCalibratedText ( planetCenter.Y, 0, 8 ) + "\n";
        text += "          Z =" + GetCalibratedText ( planetCenter.Z, 0, 8 ) + "\n";
        text += "\n";

        text += "(* from planet center)";
    }

    if ( navState == NavState.calibrating )
    {
        text += "\n";
        text += calibrationText;
    }

    return text;
}

string GetCompassText ()
{
    int textPos = ( int ) Math.Round ( bearing / 2, 0 );
    string bearingText = Math.Round ( bearing, 2 ).ToString ();

    return compassCard.Substring ( textPos, 31 ) + "\n"
        + compassTop
        + compassGrid.Substring ( textPos, 31 ) + "\n"
        + compassBottom
        + textSpacing.Substring ( 0, ( 31 - bearingText.Length ) / 2 ) + bearingText + "\n";
}

string GetCalibratedText ( double value, int precision, int width )
{
    if ( navState == NavState.calibrating && !displayCalibration )
    {
        return textSpacing.Substring ( 0, width - 3 ) + "...";
    }

    return AlignRight ( value, precision, width );
}

string AlignRight ( double value, int precision, int width )
{
    string text = Math.Round ( value, precision ).ToString ();

    width -= text.Length;
    if ( width < 0 )
    {
        width = 0;
    }
    return textSpacing.Substring ( 0, width ) + text;
}

void DisplayText ( string regularText, string condensedText )
{
    foreach ( TextSurface textSurface in textSurfaces )
    {
        if ( textSurface.myTextSurface != null )
        {
            switch ( textSurface.displayMode )
            {
            case DisplayMode.condensed:
                textSurface.myTextSurface.WriteText ( condensedText );
                break;
            default:
                textSurface.myTextSurface.WriteText ( regularText );
                break;
            }
        }
    }
}
