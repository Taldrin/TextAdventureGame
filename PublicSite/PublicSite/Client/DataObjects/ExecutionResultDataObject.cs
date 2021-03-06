﻿using System.Collections.Generic;

namespace PublicSite.Client.DataObjects
{
    public class ExecutionResultDataObject
    {
        public ExecutionResultDataObject()
        {
            OptionsToShow = new List<string>();
            MessagesToShow = new List<MessageResult>();
        }

        public List<string> OptionsToShow { get; set; }
        public List<MessageResult> MessagesToShow { get; set; }
        public bool IsInvalidInput { get; set; }
        public List<string> StatesVisited { get; set; }
    }
}
