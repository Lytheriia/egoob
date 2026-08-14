// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Erida.Silicons.SwitchableState;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._Erida.Silicons.SwitchableState;

public sealed class SwitchableStateVisualizerSystem : VisualizerSystem<BorgSwitchableStateComponent>
{
    [Dependency] private readonly SpriteSystem _spriteSystem = default!;

    protected override void OnAppearanceChange(EntityUid uid, BorgSwitchableStateComponent comp, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (!AppearanceSystem.TryGetData<BorgStateType>(
                uid, SwitchStateVisuals.Key, out var type, args.Component))
            return;

        var (path, state) = comp.BaseSprite switch
        {
            SpriteSpecifier.Rsi rsi => (rsi.RsiPath, rsi.RsiState),
            SpriteSpecifier.Texture tex => (tex.TexturePath, null),
            _ => throw new NotSupportedException()
        };

        var rsiState = type == BorgStateType.Base ? state
            : $"{state}_{type.ToString().ToLowerInvariant()}";

        _spriteSystem.LayerSetRsiState((uid, null), 0, rsiState);


        switch (type)
        {
            case BorgStateType.Base:
                {
                    EnableRest(uid, args.Sprite, false);
                    break;
                }
            default:
                {
                    EnableRest(uid, args.Sprite, true);
                    break;
                }
        }
    }

    private void EnableRest(EntityUid uid, SpriteComponent sprite, bool isEnabled)
    {
        _spriteSystem.LayerSetVisible((uid, sprite), BorgVisualLayers.LightStatus, !isEnabled);
        _spriteSystem.LayerSetVisible((uid, sprite), BorgVisualLayers.Light, !isEnabled);
    }
}
