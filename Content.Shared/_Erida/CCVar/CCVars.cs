// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Configuration;

namespace Content.Shared._Erida.CCVar;

public sealed partial class ECCVars
{
    public static readonly CVarDef<bool> ShouldPromoteAllFlags =
        CVarDef.Create("debug.need_promote", false, CVar.SERVER | CVar.SERVERONLY);
}
