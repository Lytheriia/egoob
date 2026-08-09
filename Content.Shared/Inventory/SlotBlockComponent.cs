using Robust.Shared.GameStates;

namespace Content.Shared.Inventory;

/// <summary>
/// Used to prevent items from being unequipped and equipped from slots that are listed in <see cref="Slots"/>.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(SlotBlockSystem))]
public sealed partial class SlotBlockComponent : Component
{
    /// <summary>
    /// Slots that this entity should block.
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<SlotFlags> BlockList = new(); // Erida edit
    /// <summary>
    /// Slots that this entity should only hide.
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<SlotFlags> HideList = new(); // Erida edit
}
