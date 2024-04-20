namespace SlugpupPurrs;

public class PurrrrMix : OptionInterface
{
    public static Configurable<bool> DisableMeowButton { get; set; }

    public static Configurable<bool> DisableMeowThrow { get; set; }

    public static Configurable<bool> DisablePurrrOnBack { get; set; }

    public static Configurable<bool> EnableCleaningpups { get; set; }

    public PurrrrMix()
    {
        DisableMeowButton = config.Bind("DisableMeowButton", false);
        DisableMeowThrow = config.Bind("DisableMeowThrow", false);
        DisablePurrrOnBack = config.Bind("DisablePurrrOnBack", false);
        EnableCleaningpups = config.Bind("EnableCleaningpups", false);
    }

    public override void Initialize()
    {
        OpTab optab1 = new(this, "Options");
        Tabs = new OpTab[1] { optab1 };
        OpContainer tab1Container = new(new Vector2(0f, 0f));
        optab1.AddItems(tab1Container);
        optab1.AddItems(
            new OpCheckBox(DisableMeowButton, 10f, 540f),
            new OpLabel(45f, 540f, "Disable Meow Button"),
            new OpCheckBox(DisableMeowThrow, 10f, 500f),
            new OpLabel(45f, 500f, "Disable Meow on throw"), 
            new OpCheckBox(DisablePurrrOnBack, 10f, 460f), 
            new OpLabel(45f, 460f, "Disable purrrr on back"), 
            new OpCheckBox(EnableCleaningpups, 10f, 420f), 
            new OpLabel(45f, 420f, "Enable the Electric death sprites")
            );
    }
}
