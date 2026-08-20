// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Configuration;
namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<bool> StationCreditsEnabled =
        CVarDef.Create("chronology.enabled", true, CVar.SERVER | CVar.ARCHIVE);

    /// <summary>
    /// Which year system should use. Set 0 to disable and use default value = 2710
    /// </summary>
    public static readonly CVarDef<int> CurrentYear =
        CVarDef.Create("chronology.current_year", 2710, CVar.SERVER);
}
