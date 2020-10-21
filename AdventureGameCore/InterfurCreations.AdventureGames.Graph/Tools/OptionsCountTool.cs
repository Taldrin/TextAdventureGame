using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;

namespace InterfurCreations.AdventureGames.BotMain.Tools
{
    public class OptionsCountTool
    {

        private static char[] delimiters = new char[] { ' ', '\r', '\n' };

        public static GameStats Run(DrawGame game)
        {
            var decisions = new HashSet<StateOption>();
            var states = new HashSet<DrawState>();
            GameStats stats = new GameStats();
            CountStates(game.StartState, decisions, states, stats);
            game.GameFunctions.ForEach(a => CountStates(a.StartState, decisions, states, stats));

            return new GameStats
            {
                optionsCount = decisions.Count,
                states = states,
                wordCount = stats.wordCount,
                options = decisions
            };
        }

        public static void CountStates(DrawState startState, HashSet<StateOption> foundDecisions, HashSet<DrawState> foundStates, GameStats stats)
        {
            if (!foundStates.Contains(startState))
            {
                if (!string.IsNullOrWhiteSpace(startState.StateText))
                    stats.wordCount = stats.wordCount + CleanText(startState.StateText).Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
                foundStates.Add(startState);
            }
            startState.StateOptions.ForEach(a =>
            {
                if (!foundDecisions.Contains(a))
                {
                    if (!string.IsNullOrWhiteSpace(a.StateText))
                    {
                        stats.wordCount = stats.wordCount + CleanText(a.StateText).Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
                    }

                    foundDecisions.Add(a);
                    CountStates(a.ResultState, foundDecisions, foundStates, stats);
                }
            });
        }

        public static string CleanText(string text)
        {
            text = text.Replace("<br>", " ");
            text = text.Replace("<p>", " ");
            text = text.Replace("<p align=\"left\">", "");
            text = text.Replace("</p>", " ");
            text = text.Replace("<br", " ");
            text = text.Replace("</br>", " ");
            text = text.Replace("<span>", "");
            text = text.Replace("</span>", "");
            text = text.Replace("</div>", " ");
            text = text.Replace("<div>", " ");
            text = text.Replace("<font style=\"font-size: 12px\">", "");
            text = text.Replace("<font style=\"font-size: 11px\">", "");
            text = text.Replace("<font style=\"font-size: 10px\">", "");
            text = text.Replace("style=\"font-size: 10px\">", "");
            text = text.Replace("</font>", "");
            text = text.Replace("&nbsp;", " ");
            text = text.Replace("\\n", Environment.NewLine);
            text = text.Replace("&gt;", ">");
            text = text.Replace("&lt;", "<");
            text = text.Replace("    ", " ");
            text = text.Replace("   ", " ");
            text = text.Replace("  ", " ");
            return text;
        }
    }
}
