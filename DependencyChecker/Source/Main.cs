using HugsLib;
using HugsLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace $rootnamespace$
{
    public class Main : ModBase {
    public Main() {
        Instance = this;
    }

    public override string ModIdentifier { get { return "$rootnamespace$"; } }

    internal static Main Instance { get; private set; }
    internal static ModLogger Log { get { return Instance.Logger; } }
}
}
