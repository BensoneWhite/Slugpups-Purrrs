namespace SlugpupPurrs;

public class PurrrrMix : OptionInterface
{
    public static Configurable<bool> DisableMeowButton;
    public static Configurable<bool> DisableMeow;
    public static Configurable<bool> DisablePurrrOnBack;
    public static Configurable<bool> EnableCleaningpups;
    public static Configurable<bool> DisableWhiskers;

    public PurrrrMix()
    {
        DisableMeowButton = config.Bind("DisableMeowButton", false);
        DisableMeow = config.Bind("DisableMeowThrow", false);
        DisablePurrrOnBack = config.Bind("DisablePurrrOnBack", false);
        EnableCleaningpups = config.Bind("EnableCleaningpups", false);
        DisableWhiskers = config.Bind("DisableWhiskers", false);
    }

    public override void Initialize()
    {
        OpTab optab1 = new(this, "Options");
        Tabs = [optab1];
        OpContainer tab1Container = new(new Vector2(0f, 0f));
        optab1.AddItems(tab1Container);

        AddOptionItems(optab1);
    }

    private void AddOptionItems(OpTab optab)
    {
        optab.AddItems(
            new OpCheckBox(DisableMeowButton, 10f, 540f),
            new OpLabel(45f, 540f, "Disable Meow Button"),
            new OpCheckBox(DisableMeow, 10f, 500f),
            new OpLabel(45f, 500f, "Disable Meow on throw"),
            new OpCheckBox(DisablePurrrOnBack, 10f, 460f),
            new OpLabel(45f, 460f, "Disable purrrr on back"),
            new OpCheckBox(EnableCleaningpups, 10f, 420f),
            new OpLabel(45f, 420f, "Cleaning Pups"),
            new OpCheckBox(DisableWhiskers, 10f, 380f),
            new OpLabel(45f, 380f, "Disable Whiskers")
        );
    }
}