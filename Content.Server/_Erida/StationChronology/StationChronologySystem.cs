// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Globalization;
using Content.Shared._Erida.StationChronology;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Robust.Shared.Configuration;

namespace Content.Server._Erida.StationChronology;

public sealed class StationChronologySystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private static readonly CultureInfo Culture = CultureInfo.CurrentCulture;
    private bool _isActive;
    private int _currentYear = 2710;

    public override void Initialize()
    {
        base.Initialize();

        _cfg.OnValueChanged(CCVars.StationCreditsEnabled, value => _isActive = value, true);
        _cfg.OnValueChanged(CCVars.CurrentYear, value => _currentYear = value, true);

        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawnComplete);
    }

    private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent args)
    {
        if (!_isActive)
            return;

        if (args.Silent)
            return;

        var stationName = MetaData(args.Station).EntityName;

        var dateTime = DateTime.UtcNow.AddHours(3).AddYears(_currentYear - DateTime.UtcNow.Year);

        var description = dateTime.ToString("d MMMM yyyy 'года', HH:mm", Culture);

        var ev = new ShowStationCreditsEvent(stationName, description);
        RaiseNetworkEvent(ev, args.Player);
    }
}
