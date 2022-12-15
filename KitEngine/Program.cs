using KitEngine;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;

var nativeWindowSettings = new NativeWindowSettings()
{
    Size = new Vector2i(800, 600),
    WindowState = WindowState.Normal,
    Title = "KitEngine",

    Flags = ContextFlags.Default,
    Profile = ContextProfile.Compatability,
    APIVersion = new Version(4, 6),
    API = ContextAPI.OpenGL,

    NumberOfSamples = 0,
};

var gameWindowSettings = GameWindowSettings.Default;


using (Game game = new Game(gameWindowSettings, nativeWindowSettings))
{
    game.Run();
}
