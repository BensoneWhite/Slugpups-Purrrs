global using System;
global using System.IO;
global using System.Security.Permissions;
global using System.Collections.Generic;
global using System.Linq;
global using RWCustom;
global using UnityEngine;
global using Mono.Cecil.Cil;
global using MonoMod.Cil;
global using Random = UnityEngine.Random;
global using BepInEx;
global using System.Runtime.CompilerServices;
global using static Player;
global using Menu.Remix.MixedUI;
global using BepInEx.Logging;
global using Menu;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete