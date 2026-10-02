public Program()
{
    Do();
}

public void Main(string arg, UpdateType source)
{
    Do();
}

void Do()
{
    List<IMyTextSurfaceProvider> Providers = new List<IMyTextSurfaceProvider>();

    GridTerminalSystem.GetBlocksOfType(Providers, b => b.SurfaceCount > 0);

    foreach(var provider in Providers)
    {
        for(int i = 0; i < provider.SurfaceCount; i++)
        {
            var surface = provider.GetSurface(i);
            TestDraw(surface);
        }
    }
}

void TestDraw(IMyTextSurface surface)
{
    surface.ContentType = ContentType.SCRIPT;
    surface.Script = null;
    surface.ScriptBackgroundColor = Color.DarkBlue;

    var viewport = new RectangleF((surface.TextureSize - surface.SurfaceSize) * 0.5f, surface.SurfaceSize);

    Vector2 size = new Vector2(16, 16);
    Vector2 half = (surface.SurfaceSize - size) * 0.5f;

    using(var frame = surface.DrawFrame())
    {
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Alignment = TextAlignment.CENTER,
            Color = Color.White,
            Data = "SquareSimple",
            //FontId = "",
            Position = viewport.Center,
            RotationOrScale = 0f,
            Size = size,
        });

        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Alignment = TextAlignment.CENTER,
            Color = Color.Red,
            Data = "SquareSimple",
            //FontId = "",
            Position = viewport.Center + new Vector2(-half.X + 1, -half.Y + 1),
            RotationOrScale = 0f,
            Size = size,
        });

        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Alignment = TextAlignment.CENTER,
            Color = Color.Lime,
            Data = "SquareSimple",
            //FontId = "",
            Position = viewport.Center + new Vector2(-half.X + 1, half.Y - 1),
            RotationOrScale = 0f,
            Size = size,
        });

        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Alignment = TextAlignment.CENTER,
            Color = Color.Cyan,
            Data = "SquareSimple",
            //FontId = "",
            Position = viewport.Center + new Vector2(half.X - 1, -half.Y + 1),
            RotationOrScale = 0f,
            Size = size,
        });

        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Alignment = TextAlignment.CENTER,
            Color = Color.Magenta,
            Data = "SquareSimple",
            //FontId = "",
            Position = viewport.Center + new Vector2(half.X - 1, half.Y - 1),
            RotationOrScale = 0f,
            Size = size,
        });
    }
}