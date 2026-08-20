// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Power;
using Content.Server.Station.Systems;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Consoles;
using Content.Shared.Power;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Server.Silicons.Borgs;

public sealed partial class BorgSystem
{
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    // erida edit
    private void InitializeMonitoring()
    {
        SubscribeLocalEvent<BorgChassisComponent, BorgOpenAtmosConsoleMessage>(OnOpenAtmosConsole);
        SubscribeLocalEvent<BorgChassisComponent, BorgOpenPowerConsoleMessage>(OnOpenPowerConsole);
    }

    private void OnOpenAtmosConsole(Entity<BorgChassisComponent> chassis, ref BorgOpenAtmosConsoleMessage args)
    {
        if (!IsEngineeringBorg(chassis))
            return;

        if (TryFindStationConsole<AtmosAlertsComputerComponent>(chassis, out var console))
            OpenRemoteConsole(console, AtmosAlertsComputerUiKey.Key, args.Actor);
    }

    private void OnOpenPowerConsole(Entity<BorgChassisComponent> chassis, ref BorgOpenPowerConsoleMessage args)
    {
        if (!IsEngineeringBorg(chassis))
            return;

        if (TryFindStationConsole<PowerMonitoringConsoleComponent>(chassis, out var console))
            OpenRemoteConsole(console, PowerMonitoringConsoleUiKey.Key, args.Actor);
    }

    private bool IsEngineeringBorg(Entity<BorgChassisComponent> chassis)
    {
        foreach (var module in chassis.Comp.ModuleContainer.ContainedEntities)
        {
            if (_tag.HasTag(module, "BorgModuleEngineering"))
                return true;
        }

        return false;
    }

    private bool TryFindStationConsole<T>(Entity<BorgChassisComponent> chassis, out EntityUid console)
        where T : IComponent
    {
        console = EntityUid.Invalid;

        var station = _station.GetOwningStation(chassis);
        var query = EntityQueryEnumerator<T>();
        while (query.MoveNext(out EntityUid uid, out T? _))
        {
            if (station == null || _station.GetOwningStation(uid) != station)
                continue;

            console = uid;
            return true;
        }

        return false;
    }

    private void OpenRemoteConsole(EntityUid console, Enum key, EntityUid actor)
    {
        if (!_player.TryGetSessionByEntity(actor, out var session))
            return;

        _uiSystem.OpenUi(console, key, session);
    }
    // erida edit end
}