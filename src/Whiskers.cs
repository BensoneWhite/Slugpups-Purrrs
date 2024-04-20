namespace SlugpupPurrs;

public class Whiskers
{
    public class PurrrWhiskers
    {
        public class Scale : BodyPart
        {
            public float length = 7f;

            public float width = 2f;

            public Scale(GraphicsModule cosmetics) : base(cosmetics)
            {
            }

            public override void Update()
            {
                base.Update();
                if (owner.owner.room.PointSubmerged(pos))
                {
                    vel *= 0.5f;
                }
                else
                {
                    vel *= 0.9f;
                }
                lastPos = pos;
                pos += vel;
            }
        }

        public bool purring = false;

        public int initialfacewhiskerloc;

        public string sprite = "LizardScaleA0";

        public string facesprite = "LizardScaleA0";

        public WeakReference<Player> playerref;

        public Vector2[] headpositions = (Vector2[])(object)new Vector2[6];

        public Scale[] headScales = new Scale[6];

        public Color headcolor = new(1f, 1f, 0f);

        public PurrrWhiskers(Player player) => playerref = new WeakReference<Player>(player);

        public int Facewhiskersprite(int side, int pair) => initialfacewhiskerloc + side + pair + pair;
    }

    public static ConditionalWeakTable<Player, PurrrWhiskers> meowstorage = new();

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
        if (!self.player.isNPC || !meowstorage.TryGetValue(self.player, out var meowwwww))
        {
            return;
        }
        int index = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2 pos = self.owner.bodyChunks[0].pos;
                Vector2 pos2 = self.owner.bodyChunks[1].pos;
                float num = 0f;
                float num2 = 90f;
                int num3 = index % (meowwwww.headScales.Length / 2);
                float num4 = num2 / (float)(meowwwww.headScales.Length / 2);
                if (i == 1)
                {
                    num = 0f;
                    pos.x += 5f;
                }
                else pos.x -= 5f;
                
                Vector2 a = Custom.rotateVectorDeg(Custom.DegToVec(0f), num3 * num4 - num2 / 2f + num + 90f);
                float f = Custom.VecToDeg(self.lookDirection);
                Vector2 vector = Custom.rotateVectorDeg(Custom.DegToVec(0f), num3 * num4 - num2 / 2f + num);
                Vector2 a2 = Vector2.Lerp(vector, Custom.DirVec(pos2, pos), Mathf.Abs(f));
                
                if (meowwwww.headpositions[index].y < 0.2f) a2 -= a * Mathf.Pow(Mathf.InverseLerp(0.2f, 0f, meowwwww.headpositions[index].y), 2f) * 2f;
                
                a2 = Vector2.Lerp(a2, vector, Mathf.Pow(0.0875f, 1f)).normalized;
                Vector2 vector2 = pos + a2 * meowwwww.headScales.Length;

                if (!Custom.DistLess(meowwwww.headScales[index].pos, vector2, meowwwww.headScales[index].length / 2f))
                {
                    Vector2 a3 = Custom.DirVec(meowwwww.headScales[index].pos, vector2);
                    float num5 = Vector2.Distance(meowwwww.headScales[index].pos, vector2);
                    float num6 = meowwwww.headScales[index].length / 2f;
                    PurrrWhiskers.Scale obj = meowwwww.headScales[index];
                    obj.pos += a3 * (num5 - num6);
                    PurrrWhiskers.Scale obj2 = meowwwww.headScales[index];
                    obj2.vel += a3 * (num5 - num6);
                }
                PurrrWhiskers.Scale obj3  = meowwwww.headScales[index];
                obj3.vel += Vector2.ClampMagnitude(vector2 - meowwwww.headScales[index].pos, 10f) / Mathf.Lerp(5f, 1.5f, 0.5873646f);
                PurrrWhiskers.Scale obj4 = meowwwww.headScales[index];
                obj4.vel *= Mathf.Lerp(1f, 0.8f, 0.5873646f);
                meowwwww.headScales[index].ConnectToPoint(pos, meowwwww.headScales[index].length, push: true, 0f, new Vector2(0f, 0f), 0f, 0f);
                meowwwww.headScales[index].Update();
                index++;
            }
        }
    }

    private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        if (!self.player.isNPC || !meowstorage.TryGetValue(self.player, out var meowwwww))
        {
            return;
        }
        int index = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2 vector = new(sLeaser.sprites[9].x + camPos.x, sLeaser.sprites[9].y + camPos.y);
                float f= 0f;
                float num = 0f;

                if (i == 0) vector.x -= 5f;
                else
                {
                    num = 180f;
                    vector.x += 5f;
                }
                sLeaser.sprites[meowwwww.Facewhiskersprite(i, j)].x = vector.x - camPos.x;
                sLeaser.sprites[meowwwww.Facewhiskersprite(i, j)].y = vector.y - camPos.y;
                sLeaser.sprites[meowwwww.Facewhiskersprite(i, j)].rotation = Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(meowwwww.headScales[index].lastPos, meowwwww.headScales[index].pos, timeStacker)) + num;
                sLeaser.sprites[meowwwww.Facewhiskersprite(i, j)].scaleX = 0.4f * Mathf.Sign(f);
                sLeaser.sprites[meowwwww.Facewhiskersprite(i, j)].color = sLeaser.sprites[1].color;
                index++;
            }
        }
    }

    private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        if (!self.player.isNPC || !meowstorage.TryGetValue(self.player, out var meowwwww) || !meowwwww.purring)
        {
            return;
        }
        FContainer container = rCam.ReturnFContainer("Midground");
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                container.AddChild(sLeaser.sprites[meowwwww.Facewhiskersprite(i, j)]);
            }
        }
        meowwwww.purring = false;
    }

    private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        if (!self.player.isNPC)
        {
            return;
        }
        meowstorage.TryGetValue(self.player, out var player);
        player.initialfacewhiskerloc = sLeaser.sprites.Length;
        Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 6);
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                sLeaser.sprites[player.Facewhiskersprite(i, j)] = new FSprite(player.facesprite)
                {
                    scaleY = 16f / Futile.atlasManager.GetElementWithName(player.sprite).sourcePixelSize.y,
                    anchorY = 0.15f
                };
            }
        }
        player.purring = true;
        self.AddToContainer(sLeaser, rCam, null);
    }

    private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
    {
        orig(self, ow);
        if (self.player.isNPC)
        {
            meowstorage.Add(self.player, new PurrrWhiskers(self.player));
            meowstorage.TryGetValue(self.player, out var player);
            for (int i = 0; i < player.headScales.Length; i++)
            {
                player.headScales[i] = new PurrrWhiskers.Scale(self);
                player.headpositions[i] = new Vector2((i < player.headScales.Length / 2) ? 0.7f : (-0.7f), (i == 1) ? 0.035f : 0.026f);
            }
        }
    }
}
