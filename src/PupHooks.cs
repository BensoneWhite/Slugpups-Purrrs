namespace SlugpupPurrs;

public class PupHooks
{
    public static ConditionalWeakTable<Player, ChunkDynamicSoundLoop> PurrrMeows = new();

    public static bool MeowOrMeownt;

    public static void Init()
    {
        On.Player.SlugOnBack.Update += SlugOnBack_Update;
        On.Player.SlugOnBack.SlugToBack += SlugOnBack_SlugToBack;
        On.Player.ctor += Player_ctor;
        On.Player.ThrowObject += Player_ThrowObject;
        On.Player.Update += Player_Update;
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        Room ruuuuuum = self.room;
        try
        {
            if (PurrrMeows.TryGetValue(self, out var meow) && ruuuuuum != null && !self.dead && !PurrrrMix.DisablePurrrOnBack.Value)
            {
                if (self.slugOnBack != null && 
                    self.slugOnBack.slugcat != null && 
                    self.slugOnBack.slugcat.isNPC && 
                    !self.isNPC)
                {
                    MeowOrMeownt = true;
                }

                if (MeowOrMeownt && !ruuuuuum.game.GamePaused) meow.Volume = 0.4f;
                else meow.Volume = 0f;
                meow.Update();
            }
        }
        catch (Exception e)
        {
            Plugin.Error($"Something went wrong on Player Update {Plugin.MOD_NAME}, {Plugin.VERSION}");
            Debug.LogException(e);
        }
    }

    private static void Player_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
    {
        if (!PurrrrMix.DisableMeowThrow.Value)
        {
            PhysicalObject grabbed = self.grasps[grasp].grabbed;
            Player purrrpup = (Player)((grabbed is Player) ? grabbed : null);
            if (purrrpup != null && (purrrpup.isNPC || purrrpup.isSlugpup) && !self.dead)
            {
                self.room.PlaySound(MeowEnums.Meow, self.mainBodyChunk);
            }
        }
        orig(self, grasp, eu);
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        if (!self.isSlugpup)
        {
            ChunkDynamicSoundLoop meower;
            meower = new ChunkDynamicSoundLoop(self.bodyChunks[0])
            {
                sound = MeowEnums.Purr,
                Pitch = 1f
            };
            PurrrMeows.Add(self, meower);
        }
    }

    private static void SlugOnBack_SlugToBack(On.Player.SlugOnBack.orig_SlugToBack orig, SlugOnBack self, Player playerToBack)
    {
        orig(self, playerToBack);
        if (playerToBack.isNPC && !PurrrrMix.DisableMeowThrow.Value)
        {
            self.owner.room.PlaySound(MeowEnums.Meow, self.owner.bodyChunks[0]);
        }
    }

    private static void SlugOnBack_Update(On.Player.SlugOnBack.orig_Update orig, SlugOnBack self, bool eu)
    {
        orig(self, eu);

        bool meow0 = self.owner.grasps[0] != null && self.owner.grasps[0].grabbed is Player;
        bool meow1 = self.owner.grasps[1] != null && self.owner.grasps[1].grabbed is Player;

        if (meow0) MeowOrMeownt = true;
        else if (meow1) MeowOrMeownt = true;
        else MeowOrMeownt = false;
    }
}