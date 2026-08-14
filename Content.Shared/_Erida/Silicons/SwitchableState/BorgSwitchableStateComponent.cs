// SPDX-FileCopyrightText: 2026 Lytheriia
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Erida.Silicons.SwitchableState;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BorgSwitchableStateComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public Dictionary<BorgStateType, Vector2> StatesWhiteList = [];

    [DataField(required: true), AutoNetworkedField]
    public SpriteSpecifier BaseSprite;

    [DataField, AutoNetworkedField]
    public BorgStateType CurrentType = BorgStateType.Base;

    [DataField, AutoNetworkedField] public EntProtoId ActionId = "SwitchState";

    [DataField, AutoNetworkedField] public TimeSpan DoAfterDuration = TimeSpan.FromSeconds(3);

    public bool IsRestActive = false;
}

public enum BorgStateType
{
    Base,
    Sit,
    Rest,
    BellyUp,
    DeepRest,
    Wreck
}

[Serializable, NetSerializable]
public sealed class SwitchStateMessage(BorgStateType type) : BoundUserInterfaceMessage
{
    public BorgStateType Type = type;
}

[Serializable, NetSerializable]
public enum SwitchStateUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public enum SwitchStateVisuals : byte
{
    Key,
}

public sealed partial class SwitchStateActionEvent : InstantActionEvent;

[Serializable, NetSerializable]
public sealed partial class SwitchStateDoAfterEvent : DoAfterEvent
{
    [DataField(required: true)] public BorgStateType Type;

    public SwitchStateDoAfterEvent(BorgStateType type)
    {
        Type = type;
    }

    public override DoAfterEvent Clone() => this;
}
