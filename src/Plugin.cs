namespace SlugpupPurrs;

[BepInPlugin(GUID: MOD_ID, Name: MOD_NAME, Version: VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_ID = "slugpupspurr";
    public const string MOD_NAME = "SlugpupsPurr";
    public const string VERSION = "1.4.2";

    public bool IsInit;

    public static void Warning(object ex) => Logger.LogWarning(ex);

    public static void Error(object ex) => Logger.LogError(ex);

    public static new ManualLogSource Logger;

    public static PurrrrMix optionsMenuInstance;

    public void OnEnable()
    {
        Logger = base.Logger;
        Warning($"{MOD_NAME} is loading... {VERSION}");
        try
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        }
        catch (Exception e)
        {
            Error(e);
        }
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        try
        {
            if (IsInit) return;
            IsInit = true;

            Whiskers.Init();
            MainMenu.Init();
            PupHooks.Init();
            MeowEnums.Init();

            //Remix Menu
            MachineConnector.SetRegisteredOI("SlugpupsPurrs", optionsMenuInstance = new PurrrrMix());
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Error(e);
        }
    }
}
