// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Maths.FixedPoint;

namespace Content.Shared._Shitmed.Medical.Surgery.Traumas.Systems;

public partial class TraumaSystem
{
    #region Data

    private readonly Dictionary<BoneSeverity, FixedPoint2> _boneThresholds = new()
    {
        { BoneSeverity.Normal, 40 },
        { BoneSeverity.Damaged, 25 },
        { BoneSeverity.Cracked, 10 },
        { BoneSeverity.Broken, 0 },
    };

    private readonly Dictionary<BoneSeverity, FixedPoint2> _bonePainModifiers = new()
    {
        { BoneSeverity.Normal, 0.4 },
        { BoneSeverity.Damaged, 0.6 },
        { BoneSeverity.Cracked, 0.8 },
        { BoneSeverity.Broken, 1 },
    };

    #endregion
}
