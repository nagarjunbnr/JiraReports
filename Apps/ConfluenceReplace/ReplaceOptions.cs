using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optify;

namespace ConfluenceReplace
{
    [OptionGroup("action", 1, 1)]
    public class ReplaceOptions
    {
        [OptionFlag('t'), OptionGroup("action")]
        public bool TargetOneSpace { get; set; }

        [OptionFlag('a'), OptionGroup("action")]
        public bool TargetAllSpaces { get; set; }

        [RequiresOneOption('t'), OptionFlagArgument('t')]
        public string SpaceToTarget { get; set; }

        [OptionFlag('r'), OptionRequired]
        public string ReplaceWhat { get; set; }

        [OptionFlag('w'), OptionRequired]
        public string WithWhat { get; set; }

        [OptionFlag('u'), OptionRequired]
        public string Username { get; set; }

        [OptionFlag('p'), OptionRequired]
        public string Password { get; set; }

        [OptionFlag('s')]
        public string ConfluenceBaseURL { get; set; } = "https://dev-confluence.affinitiv.com";
    }
}
