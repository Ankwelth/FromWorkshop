// Asteroids LCD Game for Space Engineers
// Simple 2D vector-style Asteroids playable from a cockpit
// Controls (while seated in a cockpit):
//   Left/Right (arrows / A-D / mouse yaw) = rotate
//   Up / Forward (W / up arrow)           = thrust
//   Space / Jump                          = fire
// To restart: Backwards (S) on the title or game-over screen

// change the sound block terminal names to these
// [AsteroidShoot] 
// [AsteroidExp]
// [AsteroidGameOver]

// change the target LCD terminal name to this
// [AsteroidArcade]

// simple sounds in vanilla sound block
const string SND_SHOOT = "ArcWepShipAutocannonShot"; // shoot sounds
const string SND_EXPLODE = "ArcPlayDropItem"; // explosion  other sounds ArcPlayTakeItem SoundBlockAlert1 
const string SND_HIT = "SoundBlockEnemyDetected"; // warning
const string SND_GAMEOVER = "SoundBlockAlert3"; //gameover
const string SND_LEVEL = "SoundBlockObjectiveComplete"; // Level up

const string LCD_NAME = "[AsteroidArcade]";   // optional tag in LCD / surface name
const int MAX_ASTEROIDS = 12;
const int MAX_BULLETS = 8;
const float SHIP_SIZE = 12f;
const float BULLET_SPEED = 4.5f;
const float THRUST = 0.12f;
const float ROT_SPEED = 0.09f;
const float FRICTION = 0.995f;
const float ASTEROID_SPEED = 0.65f;      // slower asteroids
const float SOUND_VOLUME = 2.0f; 


// DO NOT EDIT ANYTHING BEYOND THIS COMMENT
// only the above values and names can be changed,
// but best to leave block names the same.


IMyTextSurface surface;
RectangleF viewport;
IMyShipController cockpit;
IMySoundBlock shootBlock;
IMySoundBlock expBlock;
IMySoundBlock gameoverBlock;
IMySoundBlock levelupBlock;

// Game state
enum GameState { Title, Playing, GameOver }
GameState state = GameState.Title;

Vector2 shipPos;
float shipAngle;
Vector2 shipVel;
int lives = 4;                          // start with 4 lives
int score = 0;
int level = 1;
bool canFire = true;
bool canRestart = true;
int fireCooldown = 0;

struct Bullet {
    public Vector2 Pos;
    public Vector2 Vel;
    public bool Active;
}

struct Asteroid {
    public Vector2 Pos;
    public Vector2 Vel;
    public float Size;      // radius
    public float Angle;
    public float Spin;
    public bool Active;
    public int Points;      // 3-7 for irregular shape
}

Bullet[] bullets = new Bullet[MAX_BULLETS];
Asteroid[] asteroids = new Asteroid[MAX_ASTEROIDS];
Random rnd = new Random();

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    FindSurface();
    FindCockpit();
    FindSoundBlocks();
    ResetGame(true);
}

void FindSurface() {
    // Prefer tagged LCD
    var panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, p => p.CustomName.Contains(LCD_NAME));
    if (panels.Count > 0) {
        surface = panels[0];
    } else {
        // fallback to PB surface
        surface = Me.GetSurface(0);
    }
    surface.ContentType = ContentType.SCRIPT;
    surface.Script = "";
    viewport = new RectangleF(
        (surface.TextureSize - surface.SurfaceSize) / 2f,
        surface.SurfaceSize
    );
}

void FindCockpit() {
    var controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, c => c.IsUnderControl || c.CanControlShip);
    // Prefer one that is currently under control
    cockpit = controllers.Find(c => c.IsUnderControl) ?? (controllers.Count > 0 ? controllers[0] : null);
}

public void Main(string argument, UpdateType updateSource) {
    if (surface == null) FindSurface();
    if (cockpit == null || !cockpit.IsUnderControl) FindCockpit();

    float dt = (float)Runtime.TimeSinceLastRun.TotalSeconds;
    if (dt <= 0) dt = 1f / 60f;

    HandleInput(dt);
    if (state == GameState.Playing) {
        UpdatePhysics(dt);
        CheckCollisions();
    }

    Draw();
}

void FindSoundBlocks() {
    shootBlock = GridTerminalSystem.GetBlockWithName("[AsteroidShoot]") as IMySoundBlock;
    expBlock = GridTerminalSystem.GetBlockWithName("[AsteroidExp]") as IMySoundBlock;
    gameoverBlock = GridTerminalSystem.GetBlockWithName("[AsteroidGameOver]") as IMySoundBlock;
    levelupBlock = GridTerminalSystem.GetBlockWithName("[AsteroidLevelUp]") as IMySoundBlock;

    if (shootBlock != null) shootBlock.Volume = MathHelper.Clamp(4f, 0f, 4f);
    if (expBlock != null) expBlock.Volume = MathHelper.Clamp(SOUND_VOLUME, 0f, 2f);
    if (gameoverBlock != null) gameoverBlock.Volume = MathHelper.Clamp(SOUND_VOLUME, 0f, 2f);
    if (levelupBlock != null) levelupBlock.Volume = MathHelper.Clamp(SOUND_VOLUME, 0f, 2f);
}

void PlaySound(IMySoundBlock block, string soundId)
{
    if (block == null) return;

    block.SelectedSound = soundId;
    block.Play();
}

void HandleInput(float dt) {
    float rotate = 0, thrust = 0;
    bool fire = false;
    bool restart = false;

    if (cockpit != null && cockpit.IsUnderControl) {
        // Rotation
        rotate = MathHelper.Clamp(cockpit.RotationIndicator.Y * 0.02f, -1f, 1f);
        // Also allow left/right strafe as rotate for arrow-key players
        if (Math.Abs(cockpit.MoveIndicator.X) > 0.1f)
            rotate = MathHelper.Clamp(cockpit.MoveIndicator.X, -1f, 1f);

        // Thrust = forward
        thrust = Math.Max(0, -cockpit.MoveIndicator.Z);

        // Fire = space / jump (MoveIndicator.Y > 0)
        fire = cockpit.MoveIndicator.Y > 0.4f;
        // Restart = backward movement (usually S)
        restart = cockpit.MoveIndicator.Z > 0.4f;
    }
    if (state == GameState.Title || state == GameState.GameOver) {
        if (restart && canRestart) {
            ResetGame(true);
            state = GameState.Playing;
            canRestart = false;
        }
    } else if (state == GameState.Playing) {
        shipAngle += rotate * ROT_SPEED;
        if (thrust > 0.1f) {
            shipVel.X += (float)Math.Cos(shipAngle) * THRUST * thrust;
            shipVel.Y += (float)Math.Sin(shipAngle) * THRUST * thrust;
        }
        if (fire && canFire && fireCooldown <= 0) {
            FireBullet();
            fireCooldown = 8; // frames
            canFire = false;
        }
    }

    if (!fire) canFire = true;
    if (!restart) canRestart = true;
    if (fireCooldown > 0) fireCooldown--;
}

void FireBullet() {
    for (int i = 0; i < MAX_BULLETS; i++) {
        if (!bullets[i].Active) {
            bullets[i].Active = true;
            bullets[i].Pos = shipPos;
            bullets[i].Vel = new Vector2(
                (float)Math.Cos(shipAngle) * BULLET_SPEED,
                (float)Math.Sin(shipAngle) * BULLET_SPEED
            ) + shipVel * 0.3f;
            PlaySound(shootBlock, SND_SHOOT);
            break;
        }
    }
}

void ResetGame(bool full) {
    shipPos = viewport.Center;
    shipAngle = -MathHelper.PiOver2; // point up
    shipVel = Vector2.Zero;
    if (full) {
        score = 0;
        lives = 4;                      // start with 4 lives
        level = 1;
    }
    for (int i = 0; i < MAX_BULLETS; i++) bullets[i].Active = false;
    SpawnAsteroids(3 + level);
}

void SpawnAsteroids(int count) {
    for (int i = 0; i < MAX_ASTEROIDS; i++) asteroids[i].Active = false;
    for (int i = 0; i < count && i < MAX_ASTEROIDS; i++) {
        float ang = (float)(rnd.NextDouble() * MathHelper.TwoPi);
        float dist = Math.Max(viewport.Width, viewport.Height) * 0.4f;
        asteroids[i].Active = true;
        asteroids[i].Pos = viewport.Center + new Vector2(
            (float)Math.Cos(ang) * dist,
            (float)Math.Sin(ang) * dist
        );
        float speed = ASTEROID_SPEED * (0.6f + (float)rnd.NextDouble() * 0.8f);
        float dir = (float)(rnd.NextDouble() * MathHelper.TwoPi);
        asteroids[i].Vel = new Vector2((float)Math.Cos(dir) * speed, (float)Math.Sin(dir) * speed);
        asteroids[i].Size = 28f + (float)rnd.NextDouble() * 18f;
        asteroids[i].Angle = (float)(rnd.NextDouble() * MathHelper.TwoPi);
        asteroids[i].Spin = ((float)rnd.NextDouble() - 0.5f) * 0.05f;
        asteroids[i].Points = 5 + rnd.Next(3);
    }
}

void UpdatePhysics(float dt) {
    // Ship
    shipVel *= FRICTION;
    shipPos += shipVel;
    Wrap(ref shipPos);

    // Bullets
    for (int i = 0; i < MAX_BULLETS; i++) {
        if (!bullets[i].Active) continue;
        bullets[i].Pos += bullets[i].Vel;
        if (OutOfBounds(bullets[i].Pos, 20f)) bullets[i].Active = false;
    }

    // Asteroids
    for (int i = 0; i < MAX_ASTEROIDS; i++) {
        if (!asteroids[i].Active) continue;
        asteroids[i].Pos += asteroids[i].Vel;
        asteroids[i].Angle += asteroids[i].Spin;
        Wrap(ref asteroids[i].Pos);
    }
}

void Wrap(ref Vector2 p) {
    if (p.X < viewport.X) p.X += viewport.Width;
    if (p.X > viewport.X + viewport.Width) p.X -= viewport.Width;
    if (p.Y < viewport.Y) p.Y += viewport.Height;
    if (p.Y > viewport.Y + viewport.Height) p.Y -= viewport.Height;
}

bool OutOfBounds(Vector2 p, float margin) {
    return p.X < viewport.X - margin || p.X > viewport.X + viewport.Width + margin ||
           p.Y < viewport.Y - margin || p.Y > viewport.Y + viewport.Height + margin;
}

void CheckCollisions() {
    // Bullets vs Asteroids
    for (int b = 0; b < MAX_BULLETS; b++) {
        if (!bullets[b].Active) continue;
        for (int a = 0; a < MAX_ASTEROIDS; a++) {
            if (!asteroids[a].Active) continue;
            if (Vector2.DistanceSquared(bullets[b].Pos, asteroids[a].Pos) < asteroids[a].Size * asteroids[a].Size) {
                bullets[b].Active = false;
                SplitAsteroid(a);
                score += (int)(40 / (asteroids[a].Size / 20f));
                break;
            }
        }
    }

    // Ship vs Asteroids
    for (int a = 0; a < MAX_ASTEROIDS; a++) {
        if (!asteroids[a].Active) continue;
        if (Vector2.DistanceSquared(shipPos, asteroids[a].Pos) < (asteroids[a].Size + SHIP_SIZE * 0.6f) * (asteroids[a].Size + SHIP_SIZE * 0.6f)) {
            lives--;
            PlaySound(expBlock, SND_HIT);
            if (lives <= 0) {               // game over when lives reach 0
                state = GameState.GameOver;
                PlaySound(gameoverBlock, SND_GAMEOVER);
            } else {
                // respawn ship
                shipPos = viewport.Center;
                shipVel = Vector2.Zero;
                shipAngle = -MathHelper.PiOver2;
            }
            // remove the asteroid that hit
            SplitAsteroid(a);
            break;
        }
    }

    // Next level?
    bool any = false;
    for (int i = 0; i < MAX_ASTEROIDS; i++) if (asteroids[i].Active) { any = true; break; }
    if (!any) {
        level++;
        PlaySound(levelupBlock, SND_LEVEL);
        SpawnAsteroids(3 + level);
    }
}

void SplitAsteroid(int idx) {
    float oldSize = asteroids[idx].Size;
    Vector2 oldPos = asteroids[idx].Pos;
    Vector2 oldVel = asteroids[idx].Vel;
    asteroids[idx].Active = false;
    PlaySound(expBlock, SND_EXPLODE);

    if (oldSize < 18f) return; // too small, just destroy

    // spawn two smaller ones
    for (int n = 0; n < 2; n++) {
        for (int i = 0; i < MAX_ASTEROIDS; i++) {
            if (asteroids[i].Active) continue;
            asteroids[i].Active = true;
            asteroids[i].Pos = oldPos;
            float ang = (float)(rnd.NextDouble() * MathHelper.TwoPi);
            float spd = ASTEROID_SPEED * (0.8f + (float)rnd.NextDouble());
            asteroids[i].Vel = oldVel * 0.5f + new Vector2((float)Math.Cos(ang) * spd, (float)Math.Sin(ang) * spd);
            asteroids[i].Size = oldSize * 0.55f;
            asteroids[i].Angle = (float)(rnd.NextDouble() * MathHelper.TwoPi);
            asteroids[i].Spin = ((float)rnd.NextDouble() - 0.5f) * 0.08f;
            asteroids[i].Points = 4 + rnd.Next(3);
            break;
        }
    }
}

void Draw() {
    var frame = surface.DrawFrame();

    // Background
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", viewport.Center, viewport.Size, new Color(5, 5, 15)));

    if (state == GameState.Title) {
        DrawText(ref frame, "ASTEROIDS", viewport.Center + new Vector2(0, -40), 1.8f, Color.White);
        DrawText(ref frame, "Sit in cockpit", viewport.Center + new Vector2(0, 10), 0.8f, Color.Gray);
        DrawText(ref frame, "Arrows / WASD = move   Space = fire", viewport.Center + new Vector2(0, 40), 0.7f, Color.Gray);
        DrawText(ref frame, "Press Backwards (S) to start", viewport.Center + new Vector2(0, 80), 0.9f, Color.Cyan);
    } else if (state == GameState.GameOver) {
        DrawText(ref frame, "GAME OVER", viewport.Center + new Vector2(0, -30), 1.6f, Color.Red);
        DrawText(ref frame, "Score: " + score, viewport.Center + new Vector2(0, 20), 1.0f, Color.White);
        DrawText(ref frame, "Press Backwards (S) to restart", viewport.Center + new Vector2(0, 60), 0.8f, Color.Cyan);
    } else {
        // HUD
        DrawText(ref frame, "Score: " + score, new Vector2(viewport.X + 10, viewport.Y + 8), 0.7f, Color.White, TextAlignment.LEFT);
        DrawText(ref frame, "Lives: " + lives, new Vector2(viewport.X + viewport.Width - 10, viewport.Y + 8), 0.7f, Color.White, TextAlignment.RIGHT);
        DrawText(ref frame, "Lvl " + level, new Vector2(viewport.Center.X, viewport.Y + 8), 0.7f, Color.Yellow);

        // Ship (triangle)
        DrawShip(ref frame);

        // Bullets
        for (int i = 0; i < MAX_BULLETS; i++) {
            if (!bullets[i].Active) continue;
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", bullets[i].Pos, new Vector2(4, 4), Color.Yellow));
        }

        // Asteroids
        for (int i = 0; i < MAX_ASTEROIDS; i++) {
            if (!asteroids[i].Active) continue;
            DrawAsteroid(ref frame, asteroids[i]);
        }
    }

    frame.Dispose();
}

void DrawShip(ref MySpriteDrawFrame frame) {
    // Simple triangle pointing in shipAngle direction
    float a = shipAngle;
    Vector2 tip = shipPos + new Vector2((float)Math.Cos(a), (float)Math.Sin(a)) * SHIP_SIZE;
    Vector2 left = shipPos + new Vector2((float)Math.Cos(a + 2.5f), (float)Math.Sin(a + 2.5f)) * (SHIP_SIZE * 0.7f);
    Vector2 right = shipPos + new Vector2((float)Math.Cos(a - 2.5f), (float)Math.Sin(a - 2.5f)) * (SHIP_SIZE * 0.7f);

    DrawLine(ref frame, tip, left, Color.Cyan, 2f);
    DrawLine(ref frame, tip, right, Color.Cyan, 2f);
    DrawLine(ref frame, left, right, Color.Cyan, 2f);

    // thrust flame
    if (shipVel.LengthSquared() > 0.5f || (cockpit != null && cockpit.MoveIndicator.Z > 0.2f)) {
        Vector2 flame = shipPos - new Vector2((float)Math.Cos(a), (float)Math.Sin(a)) * (SHIP_SIZE * 0.9f);
        frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", flame, new Vector2(6, 6), new Color(255, 120, 0)));
    }
}

void DrawAsteroid(ref MySpriteDrawFrame frame, Asteroid ast) {
    int pts = Math.Max(3, ast.Points);
    Vector2 prev = Vector2.Zero;
    for (int i = 0; i <= pts; i++) {
        float ang = ast.Angle + (i * MathHelper.TwoPi / pts);
        // irregular radius
        float r = ast.Size * (0.75f + 0.25f * (float)Math.Sin(i * 1.7f + ast.Angle * 3));
        Vector2 p = ast.Pos + new Vector2((float)Math.Cos(ang) * r, (float)Math.Sin(ang) * r);
        if (i > 0) DrawLine(ref frame, prev, p, new Color(180, 180, 200), 1.5f);
        prev = p;
    }
}

void DrawLine(ref MySpriteDrawFrame frame, Vector2 a, Vector2 b, Color color, float thickness) {
    Vector2 diff = b - a;
    float len = diff.Length();
    if (len < 0.5f) return;
    float ang = (float)Math.Atan2(diff.Y, diff.X);
    var sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple",
        (a + b) * 0.5f,
        new Vector2(len, thickness),
        color) {
        RotationOrScale = ang,
        Alignment = TextAlignment.CENTER
    };
    frame.Add(sprite);
}

void DrawText(ref MySpriteDrawFrame frame, string text, Vector2 pos, float scale, Color color, TextAlignment align = TextAlignment.CENTER) {
    var sprite = MySprite.CreateText(text, "Debug", color, scale, align);
    sprite.Position = pos;
    frame.Add(sprite);
}

public void Save() { }