using System;

namespace InterfurCreations.AdventureGames.Logging
{
    public static class StringHelper
    {
        public static string CleanHtmlTags(this string text)
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
            text = text.Replace("<span style=\"white-space: normal\">", "");
            text = text.Replace("style=\"white-space: normal\">", "");
            text = text.Replace("style=\"font-size: 10px\">", "");
            text = text.Replace("style=\"font-size: 11px\">", "");
            text = text.Replace("style=\"font-size: 12px\">", "");
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
