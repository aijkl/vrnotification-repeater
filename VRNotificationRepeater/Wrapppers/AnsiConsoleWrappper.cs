using Spectre.Console;
using System.Collections.Generic;

namespace Aijkl.VR.NotificationRepeater.Wrapppers
{
    public static class AnsiConsoleWrappper
    {
        public enum State
        {
            Success,
            Failure,
            Info
        }
        public static void WrapMarkup(string text, State state)
        {
            Dictionary<State, Color> colorMap = new Dictionary<State, Color>();
            colorMap.Add(State.Success, Color.Green1);
            colorMap.Add(State.Info, Color.Purple3);
            colorMap.Add(State.Failure, Color.Red1);

            if (!colorMap.TryGetValue(state, out Color color)) return;
            AnsiConsole.Markup($" [[[rgb({color.R},{color.G},{color.B})]]]{state}[/] {text}");
        }
        public static void WrapMarkupLine(string text, State state)
        {
            Dictionary<State, Color> colorMap = new Dictionary<State, Color>();
            colorMap.Add(State.Success, Color.Green1);
            colorMap.Add(State.Info, Color.Grey46);
            colorMap.Add(State.Failure, Color.Red1);

            if (!colorMap.TryGetValue(state, out Color color)) return;
            AnsiConsole.MarkupLine($" [[[rgb({color.R},{color.G},{color.B})]{state}[/]]] {text}");
        }
    }
}
