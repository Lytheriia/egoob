// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared._Erida.StationChronology;

[Serializable, NetSerializable]
public sealed partial class ShowStationCreditsEvent(string text, string description) : EntityEventArgs
{
    public string Text = text;

    public string Description = description;
};
