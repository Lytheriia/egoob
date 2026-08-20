// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage.Events;

[Serializable, NetSerializable]
public sealed class StorageLimitChangedMessage : EntityEventArgs
{
    public int Limit;
}