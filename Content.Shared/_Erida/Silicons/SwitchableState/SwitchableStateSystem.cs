// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Content.Shared.Mobs;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;

namespace Content.Shared._Erida.Silicons.SwitchableState;

public sealed partial class ChangeSpriteSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifierSystem = default!;
    [Dependency] private readonly SharedHandheldLightSystem _lightSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BorgSwitchableStateComponent, ComponentInit>(OnCompInit);

        SubscribeLocalEvent<BorgSwitchableStateComponent, SwitchStateActionEvent>(OnSwitchStateActionEvent);
        SubscribeLocalEvent<BorgSwitchableStateComponent, SwitchStateMessage>(OnSwitchStateMessage);

        SubscribeLocalEvent<BorgSwitchableStateComponent, SwitchStateDoAfterEvent>(OnDoAfter);

        SubscribeLocalEvent<BorgSwitchableStateComponent, LightToggleEvent>(OnLightToggle);
        SubscribeLocalEvent<BorgSwitchableStateComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshSpeed);

        SubscribeLocalEvent<BorgSwitchableStateComponent, MobStateChangedEvent>(OnMobstateChanged);
    }

    private void OnCompInit(Entity<BorgSwitchableStateComponent> ent, ref ComponentInit args)
    {
        var action = _action.AddAction(ent, ent.Comp.ActionId);

        if (action == null)
            return;

        _action.SetEntityIcon((action.Value, null), ent.Owner);

        var userInterfaceComp = EnsureComp<UserInterfaceComponent>(ent);
        _ui.SetUi((ent, userInterfaceComp), SwitchStateUiKey.Key, new InterfaceData("SwitchableStateBoundUserInterface"));

        if (!ent.Comp.StatesWhiteList.ContainsKey(BorgStateType.Base))
            ent.Comp.StatesWhiteList.Add(BorgStateType.Base, new Vector2(1, 1));
    }

    private void OnSwitchStateActionEvent(Entity<BorgSwitchableStateComponent> ent, ref SwitchStateActionEvent args)
    {
        if (args.Handled || !TryComp<UserInterfaceComponent>(ent, out var userInterfaceComp))
            return;

        args.Handled = true;

        if (!_ui.IsUiOpen((ent, userInterfaceComp), SwitchStateUiKey.Key, args.Performer))
            _ui.OpenUi((ent, userInterfaceComp), SwitchStateUiKey.Key, args.Performer);
        else
            _ui.CloseUi((ent, userInterfaceComp), SwitchStateUiKey.Key, args.Performer);
    }

    private void OnSwitchStateMessage(Entity<BorgSwitchableStateComponent> ent, ref SwitchStateMessage args)
    {
        if (args.Type != BorgStateType.Base && !ent.Comp.StatesWhiteList.ContainsKey(args.Type))
            return;

        if (ent.Comp.CurrentType == args.Type)
        {
            if (TryComp<UserInterfaceComponent>(ent, out var uiComp) &&
                _ui.IsUiOpen((ent, uiComp), SwitchStateUiKey.Key, args.Actor))
                _ui.CloseUi((ent, uiComp), SwitchStateUiKey.Key, args.Actor);

            return;
        }

        var doAfterArgs = new DoAfterArgs(EntityManager,
            ent.Owner,
            ent.Comp.DoAfterDuration,
            new SwitchStateDoAfterEvent(args.Type),
            ent.Owner)
        {
            BreakOnDamage = false,
            BlockDuplicate = true,
            BreakOnHandChange = false,
        };

        _doAfter.TryStartDoAfter(doAfterArgs);

        if (TryComp<UserInterfaceComponent>(ent, out var uiComp2) &&
            _ui.IsUiOpen((ent, uiComp2), SwitchStateUiKey.Key, args.Actor))
            _ui.CloseUi((ent, uiComp2), SwitchStateUiKey.Key, args.Actor);
    }

    private void OnDoAfter(Entity<BorgSwitchableStateComponent> ent, ref SwitchStateDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (args.Type == ent.Comp.CurrentType)
            return;

        if (!TryComp<AppearanceComponent>(ent, out var appearanceComp))
            return;

        args.Handled = true;

        ChangeType(ent, args.Type, appearanceComp);
    }

    private void ChangeType(Entity<BorgSwitchableStateComponent> ent, BorgStateType newType, AppearanceComponent appearanceComponent)
    {
        switch (newType)
        {
            case BorgStateType.Base:
                {
                    SetRest(ent, false);
                    break;
                }
            default:
                {
                    SetRest(ent, true);
                    break;
                }
        }

        ent.Comp.CurrentType = newType;

        _appearance.SetData(ent, SwitchStateVisuals.Key, newType, appearanceComponent);

        _movementSpeedModifierSystem.RefreshMovementSpeedModifiers(ent.Owner);
        Dirty(ent);
    }

    private void SetRest(Entity<BorgSwitchableStateComponent> ent, bool isActive)
    {
        if (TryComp<SpriteMovementComponent>(ent.Owner, out var spriteMovement))
        {
            spriteMovement.IsEnabled = !isActive;
            Dirty(ent.Owner, spriteMovement);
        }

        if (isActive)
        {
            if (TryComp<HandheldLightComponent>(ent.Owner, out var comp))
                _lightSystem.TurnOff((ent.Owner, comp));

            ent.Comp.IsRestActive = true;
        }
        else
        {
            ent.Comp.IsRestActive = false;
        }
    }

    private void OnLightToggle(Entity<BorgSwitchableStateComponent> ent, ref LightToggleEvent args)
    {
        if (ent.Comp.CurrentType != BorgStateType.Base && TryComp<HandheldLightComponent>(ent.Owner, out var comp))
            _lightSystem.TurnOff((ent.Owner, comp), false);
    }

    private void OnRefreshSpeed(Entity<BorgSwitchableStateComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        var vector = ent.Comp.StatesWhiteList[ent.Comp.CurrentType];
        args.ModifySpeed(vector.X, vector.Y);
    }

    private void OnMobstateChanged(Entity<BorgSwitchableStateComponent> ent, ref MobStateChangedEvent args)
    {
        if (!TryComp<AppearanceComponent>(ent, out var appearanceComp))
            return;

        switch (args.NewMobState)
        {
            case MobState.Critical:
                {
                    ChangeType(ent, BorgStateType.Wreck, appearanceComp);
                    break;
                }
            case MobState.Dead:
                {
                    ChangeType(ent, BorgStateType.Wreck, appearanceComp);
                    break;
                }
        }

        if (args.OldMobState == MobState.Critical && args.NewMobState == MobState.Alive)
        {
            ChangeType(ent, BorgStateType.Base, appearanceComp);
        }
    }
}

