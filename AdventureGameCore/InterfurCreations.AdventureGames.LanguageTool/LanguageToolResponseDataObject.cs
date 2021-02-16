using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.LanguageTool
{
    public class LanguageToolResponseDataObject
    {
        public LTSoftware software { get; set; }
        public LTLanguage language { get; set; }
        public List<LTMatch> matches { get; set; }
    }

    public class LTSoftware
    {
        public string name { get; set; }
        public string version { get; set; }
        public string buildDate { get; set; }
        public int apiVersion { get; set; }
        public string status { get; set; }
        public bool premium { get; set; }
    }

    public class LTDetectedLanguage
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class LTLanguage
    {
        public string name { get; set; }
        public string code { get; set; }
        public LTDetectedLanguage detectedLanguage { get; set; }
    }

    public class LTReplacement
    {
        public string value { get; set; }
    }

    public class LTContext
    {
        public string text { get; set; }
        public int offset { get; set; }
        public int length { get; set; }
    }

    public class LTUrl
    {
        public string value { get; set; }
    }

    public class LTCategory
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class LTRule
    {
        public string id { get; set; }
        public string subId { get; set; }
        public string description { get; set; }
        public List<LTUrl> urls { get; set; }
        public string issueType { get; set; }
        public LTCategory category { get; set; }
    }

    public class LTMatch
    {
        public string message { get; set; }
        public string shortMessage { get; set; }
        public int offset { get; set; }
        public int length { get; set; }
        public List<LTReplacement> replacements { get; set; }
        public LTContext context { get; set; }
        public string sentence { get; set; }
        public LTRule rule { get; set; }
    }
}
