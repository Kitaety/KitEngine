using KitEngine;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;

var nativeWindowSettings = new NativeWindowSettings()
{
    Size = new Vector2i(1500, 700),
    WindowState = WindowState.Normal,
    Title = "KitEngine",

    Flags = ContextFlags.ForwardCompatible,
    Profile = ContextProfile.Core,
    APIVersion = new Version(4, 6),
    API = ContextAPI.OpenGL,
    
    NumberOfSamples = 0,
};

var gameWindowSettings = GameWindowSettings.Default;


using Game game = new Game(gameWindowSettings, nativeWindowSettings, VSyncMode.On);
game.Run();