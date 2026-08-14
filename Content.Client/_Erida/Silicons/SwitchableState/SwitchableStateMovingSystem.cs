// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Erida.Silicons.SwitchableState;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._Erida.Silicons.SwitchableState;

public sealed class SwitchableStateMovingSystem : EntitySystem
{
    [Dependency] private readonly SpriteSystem _spriteSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BorgSwitchableStateComponent, SpriteMoveEvent>(OnMove);
    }

    private void OnMove(Entity<BorgSwitchableStateComponent> ent, ref SpriteMoveEvent args)
    {
        if (!TryComp<SpriteMovementComponent>(ent.Owner, out var comp) || !comp.IsEnabled)
            return;

        var (path, state) = ent.Comp.BaseSprite switch
        {
            SpriteSpecifier.Rsi rsi => (rsi.RsiPath, rsi.RsiState),
            SpriteSpecifier.Texture tex => (tex.TexturePath, null),
            _ => throw new NotSupportedException()
        };

        if (args.IsMoving)
        {
            _spriteSystem.LayerSetRsiState((ent.Owner, null), BorgVisualLayers.LightStatus, state + "_moving_e");
            _spriteSystem.LayerSetRsiState((ent.Owner, null), BorgVisualLayers.Light, state + "_moving_e");
        }
        else
        {
            _spriteSystem.LayerSetRsiState((ent.Owner, null), BorgVisualLayers.LightStatus, state + "_e");
            _spriteSystem.LayerSetRsiState((ent.Owner, null), BorgVisualLayers.Light, state + "_e");
        }
    }
}
