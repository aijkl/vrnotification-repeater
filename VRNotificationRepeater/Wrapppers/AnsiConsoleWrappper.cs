using Spectre.Console;
using System.Collections.Generic;

namespace Aijkl.VR.NotificationRepeater.Wrappers
{
    internal static class AnsiConsoleHelper
    {
        internal enum State
        {
            Success,
            Failure,
            Info,
            Warning
        }
        private static readonly Dictionary<State, Color> ColorMap = new()
        {
            { State.Success, Color.Green1 },
            { State.Info, Color.Grey46 },
            { State.Failure, Color.Red1 },
            { State.Warning, Color.Yellow },
        };
        internal static void Markup(string text, State state = State.Info)
        {
            if (!ColorMap.TryGetValue(state, out Color color)) return;
            AnsiConsole.Markup($" [[[rgb({color.R},{color.G},{color.B})]]]{state}[/] {text}");
        }
        internal static void MarkupLine(string text, State state = State.Info)
        {
            if (!ColorMap.TryGetValue(state, out Color color)) return;
            AnsiConsole.MarkupLine($" [[[rgb({color.R},{color.G},{color.B})]{state}[/]]] {text}");
        }
    }
}
