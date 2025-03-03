namespace SlugpupPurrs;

public class Whiskers
{
    public class PurrrWhiskers
    {
        public bool Purring { get; set; } = false;
        public int InitialFaceWhiskerLoc { get; set; }
        public string Sprite { get; set; } = "LizardScaleA0";
        public string FaceSprite { get; set; } = "LizardScaleA0";
        public WeakReference<Player> PlayerRef { get; }
        public Vector2[] HeadPositions { get; } = new Vector2[6];
        public Scale[] HeadScales { get; } = new Scale[6];

        public PurrrWhiskers(Player player)
        {
            PlayerRef = new(player);
        }

        public int GetFaceWhiskerSpriteIndex(int side, int pair) =>
            InitialFaceWhiskerLoc + side + pair + pair;

        public class Scale : BodyPart
        {
            public float Length = 5f;
            public float Width = 1f;

            public Scale(GraphicsModule cosmetics) : base(cosmetics) { }

            public override void Update()
            {
                base.Update();
                vel *= owner.owner.room.PointSubmerged(pos) ? 0.5f : 0.9f;
                lastPos = pos;
                pos += vel;
            }
        }
    }

    public static ConditionalWeakTable<Player, PurrrWhiskers> WhiskerStorage = new();

    public static void Init()
    {
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
        On.PlayerGraphics.ctor += PlayerGraphics_ctor;
        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        On.PlayerGraphics.Update += PlayerGraphics_Update;
    }

    private static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics graphics)
    {
        orig(graphics);
        if (!graphics.player.isNPC ||
            !WhiskerStorage.TryGetValue(graphics.player, out var whiskerData) ||
            PurrrrMix.DisableWhiskers.Value)
        {
            return;
        }

        int index = 0;
        for (int side = 0; side < 2; side++)
        {
            for (int pair = 0; pair < 3; pair++)
            {
                UpdateHeadScale(graphics, whiskerData, ref index, side, pair);
            }
        }
    }

    private static void UpdateHeadScale(PlayerGraphics graphics, PurrrWhiskers whiskerData, ref int index, int side, int pair)
    {
        Vector2 headPos = graphics.owner.bodyChunks[0].pos;
        Vector2 secondaryPos = graphics.owner.bodyChunks[1].pos;
        const float angleOffset = 0f;
        const float totalAngle = 90f;
        int halfLength = whiskerData.HeadScales.Length / 2;
        int scaleIndex = index % halfLength;
        float angleStep = totalAngle / halfLength;

        // Adjust head position based on side
        headPos.x += side == 1 ? 5f : -5f;

        Vector2 baseDirection = Custom.rotateVectorDeg(Custom.DegToVec(0f),
            scaleIndex * angleStep - totalAngle / 2f + angleOffset + 90f);
        float lookAngle = Custom.VecToDeg(graphics.lookDirection);
        Vector2 baseVector = Custom.rotateVectorDeg(Custom.DegToVec(0f),
            scaleIndex * angleStep - totalAngle / 2f + angleOffset);
        Vector2 lerpedDir = Vector2.Lerp(baseVector, Custom.DirVec(secondaryPos, headPos), Mathf.Abs(lookAngle));

        // Adjust based on head scale's vertical position
        if (whiskerData.HeadPositions[index].y < 0.2f)
        {
            float t = Mathf.InverseLerp(0.2f, 0f, whiskerData.HeadPositions[index].y);
            lerpedDir -= baseDirection * Mathf.Pow(t, 2f) * 2f;
        }

        lerpedDir = Vector2.Lerp(lerpedDir, baseVector, Mathf.Pow(0.0875f, 1f)).normalized;
        Vector2 targetPos = headPos + lerpedDir * whiskerData.HeadScales.Length;

        // Adjust the head scale if too far from the target
        if (!Custom.DistLess(whiskerData.HeadScales[index].pos, targetPos, whiskerData.HeadScales[index].Length / 2f))
        {
            Vector2 direction = Custom.DirVec(whiskerData.HeadScales[index].pos, targetPos);
            float distance = Vector2.Distance(whiskerData.HeadScales[index].pos, targetPos);
            float halfLengthScale = whiskerData.HeadScales[index].Length / 2f;
            whiskerData.HeadScales[index].pos += direction * (distance - halfLengthScale);
            whiskerData.HeadScales[index].vel += direction * (distance - halfLengthScale);
        }

        whiskerData.HeadScales[index].vel += Vector2.ClampMagnitude(targetPos - whiskerData.HeadScales[index].pos, 10f) /
                                               Mathf.Lerp(5f, 1.5f, 0.5873646f);
        whiskerData.HeadScales[index].vel *= Mathf.Lerp(1f, 0.8f, 0.5873646f);
        whiskerData.HeadScales[index].ConnectToPoint(headPos, whiskerData.HeadScales[index].Length, push: true,
            0f, Vector2.zero, 0f, 0f);
        whiskerData.HeadScales[index].Update();
        index++;
    }

    private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics graphics, RoomCamera.SpriteLeaser spriteLeaser, RoomCamera cam, float timeStacker, Vector2 camPos)
    {
        orig(graphics, spriteLeaser, cam, timeStacker, camPos);
        if (!graphics.player.isNPC ||
            !WhiskerStorage.TryGetValue(graphics.player, out var whiskerData) ||
            PurrrrMix.DisableWhiskers.Value)
        {
            return;
        }

        int index = 0;
        for (int side = 0; side < 2; side++)
        {
            for (int pair = 0; pair < 3; pair++)
            {
                DrawFaceWhiskerSprite(spriteLeaser, cam, timeStacker, camPos, whiskerData, ref index, side, pair);
            }
        }
    }

    private static void DrawFaceWhiskerSprite(RoomCamera.SpriteLeaser spriteLeaser, RoomCamera cam, float timeStacker, Vector2 camPos, PurrrWhiskers whiskerData, ref int index, int side, int pair)
    {
        Vector2 basePos = new Vector2(spriteLeaser.sprites[9].x + camPos.x, spriteLeaser.sprites[9].y + camPos.y);
        float rotationOffset = side == 0 ? 0f : 180f;
        basePos.x += side == 0 ? -5f : 5f;

        int spriteIndex = whiskerData.GetFaceWhiskerSpriteIndex(side, pair);
        spriteLeaser.sprites[spriteIndex].x = basePos.x - camPos.x;
        spriteLeaser.sprites[spriteIndex].y = basePos.y - camPos.y;
        spriteLeaser.sprites[spriteIndex].rotation = Custom.AimFromOneVectorToAnother(
            basePos,
            Vector2.Lerp(whiskerData.HeadScales[index].lastPos, whiskerData.HeadScales[index].pos, timeStacker)) + rotationOffset;
        spriteLeaser.sprites[spriteIndex].scaleX = 0.4f * Mathf.Sign(0f);
        spriteLeaser.sprites[spriteIndex].color = spriteLeaser.sprites[side].color;
        index++;
    }

    private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics graphics, RoomCamera.SpriteLeaser spriteLeaser, RoomCamera cam, FContainer container)
    {
        orig(graphics, spriteLeaser, cam, container);
        if (!graphics.player.isNPC ||
            !WhiskerStorage.TryGetValue(graphics.player, out var whiskerData) ||
            !whiskerData.Purring ||
            PurrrrMix.DisableWhiskers.Value)
        {
            return;
        }

        FContainer bgContainer = cam.ReturnFContainer("Background");
        for (int side = 0; side < 2; side++)
        {
            for (int pair = 0; pair < 3; pair++)
            {
                int spriteIndex = whiskerData.GetFaceWhiskerSpriteIndex(side, pair);
                bgContainer.AddChild(spriteLeaser.sprites[spriteIndex]);
                bgContainer.MoveBehindOtherNode(spriteLeaser.sprites[3]);
            }
        }
        whiskerData.Purring = false;
    }

    private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics graphics, RoomCamera.SpriteLeaser spriteLeaser, RoomCamera cam)
    {
        orig(graphics, spriteLeaser, cam);
        if (!graphics.player.isNPC || PurrrrMix.DisableWhiskers.Value)
        {
            return;
        }

        if (WhiskerStorage.TryGetValue(graphics.player, out var whiskerData))
        {
            whiskerData.InitialFaceWhiskerLoc = spriteLeaser.sprites.Length;
            Array.Resize(ref spriteLeaser.sprites, spriteLeaser.sprites.Length + 6);
            for (int side = 0; side < 2; side++)
            {
                for (int pair = 0; pair < 3; pair++)
                {
                    spriteLeaser.sprites[whiskerData.GetFaceWhiskerSpriteIndex(side, pair)] =
                        new FSprite(whiskerData.FaceSprite)
                        {
                            scaleY = 16f / Futile.atlasManager.GetElementWithName(whiskerData.Sprite).sourcePixelSize.y,
                            anchorY = 0.15f
                        };
                }
            }
            whiskerData.Purring = true;
            graphics.AddToContainer(spriteLeaser, cam, null);
        }
    }

    private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics graphics, PhysicalObject owner)
    {
        orig(graphics, owner);
        // Only add whiskers to non-NPC players or if disabled whiskers is false.
        if (graphics.player.isNPC || !PurrrrMix.DisableWhiskers.Value)
        {
            WhiskerStorage.Add(graphics.player, new PurrrWhiskers(graphics.player));
            if (WhiskerStorage.TryGetValue(graphics.player, out var whiskerData))
            {
                for (int i = 0; i < whiskerData.HeadScales.Length; i++)
                {
                    whiskerData.HeadScales[i] = new PurrrWhiskers.Scale(graphics);
                    whiskerData.HeadPositions[i] = new Vector2(
                        i < whiskerData.HeadScales.Length / 2 ? 0.7f : -0.7f,
                        i == 1 ? 0.035f : 0.026f);
                }
            }
        }
    }
}