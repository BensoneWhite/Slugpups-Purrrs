namespace SlugpupPurrs;

public class MainMenu
{
    public static void Init()
    {
        if (!ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "dressmyslugcat"))
        {
            IL.Menu.MainMenu.AddMainMenuButton += MainMenu_AddMainMenuButton;
        }
        On.Menu.MainMenu.ctor += MainMenu_ctor;
    }

    private static void MainMenu_ctor(On.Menu.MainMenu.orig_ctor orig, Menu.MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
    {
        orig.Invoke(self, manager, showRegionSpecificBkg);
        if (self != null && !PurrrrMix.DisableMeowButton.Value)
        {
            float buttonWidth = Menu.MainMenu.GetButtonWidth(self.CurrLang);
            Vector2 pos = new(683f - buttonWidth / 2f, 0f);
            Vector2 size = new(buttonWidth, 30f);
            self.AddMainMenuButton(new SimpleButton(self, self.pages[0], "MEOW~", "MEOW~", pos, size), delegate
            {
                int num;
                num = Random.Range(0, MeowEnums.Meows.Length);
                self.PlaySound(MeowEnums.Meows[num]);
            }, 0);
        }
    }

    private static void MainMenu_AddMainMenuButton(ILContext il)
    {
        ILCursor cursor = new(il);
        try
        {
            if (cursor.TryGotoNext(MoveType.After, [i => i.MatchLdcI4(8)] ))
            {
                cursor.MoveAfterLabels();
                cursor.EmitDelegate((int _) => 12);
            }
            else
            {
                Plugin.Error($"Failed to load Meow Button from {Plugin.MOD_NAME}, {Plugin.VERSION}");
            }
        }
        catch (Exception ex)
        {
            Plugin.Error("Exception when matching IL for MainMenu_ctor1!" + ex);
            throw;
        }
    }
}
