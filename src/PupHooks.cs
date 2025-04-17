namespace SlugpupPurrs;

public class PupHooks
{
    public static ConditionalWeakTable<Player, ChunkDynamicSoundLoop> PlayerSoundLoops = new();

    public static bool IsMeowing;

    public static void Init()
    {
        On.Player.SlugOnBack.Update += SlugOnBack_Update;
        On.Player.SlugOnBack.SlugToBack += SlugOnBack_SlugToBack;
        On.Player.ctor += Player_ctor;
        On.Player.ThrowObject += Player_ThrowObject;
        On.Player.Update += Player_Update;
    }

    private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
    {
        orig(player, eu);
        Room room = player.room;
        try
        {
            if (PlayerSoundLoops.TryGetValue(player, out var soundLoop) && room != null && !player.dead && !PurrrrMix.DisablePurrrOnBack.Value)
            {
                if (player.slugOnBack != null &&
                    player.slugOnBack.slugcat != null &&
                    player.slugOnBack.slugcat.isNPC &&
                    !player.isNPC)
                {
                    IsMeowing = true;
                }

                soundLoop.Volume = IsMeowing && !room.game.GamePaused ? 0.4f : 0f;
                soundLoop.Update();
            }
        }
        catch (Exception e)
        {
            Plugin.Error($"Error during Player Update in {Plugin.MOD_NAME}, {Plugin.VERSION}");
            Debug.LogException(e);
        }
    }

    private static void Player_ThrowObject(On.Player.orig_ThrowObject orig, Player player, int grasp, bool eu)
    {
        if (!PurrrrMix.DisableMeow.Value)
        {
            PhysicalObject grabbedObject = player.grasps[grasp].grabbed;
            if (grabbedObject is Player grabbedPlayer && (grabbedPlayer.isNPC || grabbedPlayer.isSlugpup) && !player.dead)
            {
                player.room.PlaySound(MeowEnums.Meow, player.mainBodyChunk);
            }
        }
        orig(player, grasp, eu);
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player player, AbstractCreature abstractCreature, World world)
    {
        orig(player, abstractCreature, world);
        if (!player.isSlugpup)
        {
            var soundLoop = new ChunkDynamicSoundLoop(player.bodyChunks[0])
            {
                sound = MeowEnums.Purr,
                Pitch = 1f
            };
            PlayerSoundLoops.Add(player, soundLoop);
        }
    }

    private static void SlugOnBack_SlugToBack(On.Player.SlugOnBack.orig_SlugToBack orig, SlugOnBack slugOnBack, Player playerToBack)
    {
        orig(slugOnBack, playerToBack);
        if (playerToBack.isNPC && !PurrrrMix.DisableMeow    .Value)
        {
            slugOnBack.owner.room.PlaySound(MeowEnums.Meow, slugOnBack.owner.bodyChunks[0]);
        }
    }

    private static void SlugOnBack_Update(On.Player.SlugOnBack.orig_Update orig, SlugOnBack slugOnBack, bool eu)
    {
        orig(slugOnBack, eu);

        bool isGraspingPlayer0 = slugOnBack.owner.grasps[0] != null && slugOnBack.owner.grasps[0].grabbed is Player;
        bool isGraspingPlayer1 = slugOnBack.owner.grasps[1] != null && slugOnBack.owner.grasps[1].grabbed is Player;

        IsMeowing = isGraspingPlayer0 || isGraspingPlayer1;
    }
}
