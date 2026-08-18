// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Erida.StationChronology;
using Robust.Client.Graphics;

namespace Content.Client._Erida.StationChronology;

public sealed class StationChronology : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overMan = default!;

    private StationCreditsOverlay _overlay = default!;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<ShowStationCreditsEvent>(OnShowStationCreditsEvent);

        _overlay = new StationCreditsOverlay();
        _overMan.AddOverlay(_overlay);
    }

    private void OnShowStationCreditsEvent(ShowStationCreditsEvent args)
    {
        _overlay.Reset();
        _overlay.ResetDescription();

        _overlay.Text = args.Text;
        _overlay.TextDescription = args.Description;

        if (string.IsNullOrEmpty(_overlay.Text))
            _overlay.CharInterval = TimeSpan.Zero;
        else
            _overlay.CharInterval = TimeSpan.FromSeconds(2f / _overlay.Text.Length);

        if (_overlay.TextDescription == "")
            _overlay.CharIntervalDescription = TimeSpan.Zero;
        else
            _overlay.CharIntervalDescription = TimeSpan.FromSeconds(2f / _overlay.TextDescription.Length);
    }
}
