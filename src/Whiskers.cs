namespace SlugpupPurrs;

public class Whiskers
{
    public class PurrrWhiskers(Player player)
    {
        public class Scale(GraphicsModule cosmetics) : BodyPart(cosmetics)
        {
            public float Length = 7f;
            public float Width = 2f;

            public override void Update()
            {
                base.Update();
                vel *= owner.owner.room.PointSubmerged(pos) ? 0.5f : 0.9f;
                lastPos = pos;
                pos += vel;
            }
        }

        public bool Purring = false;
        public int InitialFaceWhiskerLoc { get; set; }
        public string Sprite = "LizardScaleA0";
        public string FaceSprite = "LizardScaleA0";
        public WeakReference<Player> PlayerRef = new(player);
        public Vector2[] HeadPositions = new Vector2[6];
        public Scale[] HeadScales = new Scale[6];
        public Color HeadColor = new(1f, 1f, 0f);

        public int FaceWhiskerSprite(int side, int pair) => InitialFaceWhiskerLoc + side + pair + pair;
    }

    public static ConditionalWeakTable<Player, PurrrWhiskers> MeowStorage = new();

    public static void Init()
    {
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
        On.PlayerGraphics.ctor += PlayerGraphics_ctor;
        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        On.PlayerGraphics.Update += PlayerGraphics_Update;
    }

    private static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);
        if (!self.player.isNPC || !MeowStorage.TryGetValue(self.player, out var meowwwww))
        {
            return;
        }

        int index = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                UpdateHeadScale(self, meowwwww, ref index, i, j);
            }
        }
    }

    private static void UpdateHeadScale(PlayerGraphics self, PurrrWhiskers meowwwww, ref int index, int i, int j)
    {
        Vector2 pos = self.owner.bodyChunks[0].pos;
        Vector2 pos2 = self.owner.bodyChunks[1].pos;
        float num = 0f;
        float num2 = 90f;
        int num3 = index % (meowwwww.HeadScales.Length / 2);
        float num4 = num2 / (meowwwww.HeadScales.Length / 2);

        pos.x += (i == 1) ? 5f : -5f;

        Vector2 a = Custom.rotateVectorDeg(Custom.DegToVec(0f), num3 * num4 - num2 / 2f + num + 90f);
        float f = Custom.VecToDeg(self.lookDirection);
        Vector2 vector = Custom.rotateVectorDeg(Custom.DegToVec(0f), num3 * num4 - num2 / 2f + num);
        Vector2 a2 = Vector2.Lerp(vector, Custom.DirVec(pos2, pos), Mathf.Abs(f));

        if (meowwwww.HeadPositions[index].y < 0.2f)
        {
            a2 -= a * Mathf.Pow(Mathf.InverseLerp(0.2f, 0f, meowwwww.HeadPositions[index].y), 2f) * 2f;
        }

        a2 = Vector2.Lerp(a2, vector, Mathf.Pow(0.0875f, 1f)).normalized;
        Vector2 vector2 = pos + a2 * meowwwww.HeadScales.Length;

        if (!Custom.DistLess(meowwwww.HeadScales[index].pos, vector2, meowwwww.HeadScales[index].Length / 2f))
        {
            Vector2 a3 = Custom.DirVec(meowwwww.HeadScales[index].pos, vector2);
            float num5 = Vector2.Distance(meowwwww.HeadScales[index].pos, vector2);
            float num6 = meowwwww.HeadScales[index].Length / 2f;
            meowwwww.HeadScales[index].pos += a3 * (num5 - num6);
            meowwwww.HeadScales[index].vel += a3 * (num5 - num6);
        }

        meowwwww.HeadScales[index].vel += Vector2.ClampMagnitude(vector2 - meowwwww.HeadScales[index].pos, 10f) / Mathf.Lerp(5f, 1.5f, 0.5873646f);
        meowwwww.HeadScales[index].vel *= Mathf.Lerp(1f, 0.8f, 0.5873646f);
        meowwwww.HeadScales[index].ConnectToPoint(pos, meowwwww.HeadScales[index].Length, push: true, 0f, new Vector2(0f, 0f), 0f, 0f);
        meowwwww.HeadScales[index].Update();
        index++;
    }

    private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        if (!self.player.isNPC || !MeowStorage.TryGetValue(self.player, out var meowwwww))
        {
            return;
        }

        int index = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                DrawFaceWhiskerSprite(sLeaser, rCam, timeStacker, camPos, meowwwww, ref index, i, j);
            }
        }
    }

    private static void DrawFaceWhiskerSprite(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, PurrrWhiskers meowwwww, ref int index, int i, int j)
    {
        Vector2 vector = new(sLeaser.sprites[9].x + camPos.x, sLeaser.sprites[9].y + camPos.y);
        float num = (i == 0) ? 0f : 180f;
        vector.x += (i == 0) ? -5f : 5f;

        sLeaser.sprites[meowwwww.FaceWhiskerSprite(i, j)].x = vector.x - camPos.x;
        sLeaser.sprites[meowwwww.FaceWhiskerSprite(i, j)].y = vector.y - camPos.y;
        sLeaser.sprites[meowwwww.FaceWhiskerSprite(i, j)].rotation = Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(meowwwww.HeadScales[index].lastPos, meowwwww.HeadScales[index].pos, timeStacker)) + num;
        sLeaser.sprites[meowwwww.FaceWhiskerSprite(i, j)].scaleX = 0.4f * Mathf.Sign(0f);
        sLeaser.sprites[meowwwww.FaceWhiskerSprite(i, j)].color = sLeaser.sprites[1].color;
        index++;
    }

    private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
    {
        orig(self, sLeaser, rCam, newContainer);
        if (!self.player.isNPC || !MeowStorage.TryGetValue(self.player, out var meowwwww) || !meowwwww.Purring)
        {
            return;
        }

        FContainer container = rCam.ReturnFContainer("Midground");
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                container.AddChild(sLeaser.sprites[meowwwww.FaceWhiskerSprite(i, j)]);
            }
        }
        meowwwww.Purring = false;
    }

    private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        if (!self.player.isNPC)
        {
            return;
        }

        MeowStorage.TryGetValue(self.player, out var player);
        player.InitialFaceWhiskerLoc = sLeaser.sprites.Length;
        Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 6);
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                sLeaser.sprites[player.FaceWhiskerSprite(i, j)] = new FSprite(player.FaceSprite)
                {
                    scaleY = 16f / Futile.atlasManager.GetElementWithName(player.Sprite).sourcePixelSize.y,
                    anchorY = 0.15f
                };
            }
        }
        player.Purring = true;
        self.AddToContainer(sLeaser, rCam, null);
    }

    private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
    {
        orig(self, ow);
        if (self.player.isNPC)
        {
            MeowStorage.Add(self.player, new PurrrWhiskers(self.player));
            MeowStorage.TryGetValue(self.player, out var player);
            for (int i = 0; i < player.HeadScales.Length; i++)
            {
                player.HeadScales[i] = new PurrrWhiskers.Scale(self);
                player.HeadPositions[i] = new Vector2((i < player.HeadScales.Length / 2) ? 0.7f : -0.7f, (i == 1) ? 0.035f : 0.026f);
            }
        }
    }
}