namespace SlugpupPurrs;

public class MeowEnums
{
    public static SoundID Purr;

    public static SoundID Meow;

    public static SoundID[] Meows;

    public static void Init()
    {
        Purr = new SoundID("purrr1", register: true);
        Meow = new SoundID("meow1", register: true);

        Meows = [
            new("meow2", register: true),
            new("meow3", register: true),
            new("catf1", register: true),
            new("catf2", register: true),
            new("purrr2", register: true)
        ];
    }
}
