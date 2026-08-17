using Content.Shared._DV.Traits.Effects;
using Content.Shared.Humanoid;

namespace Content.Shared._Erida.Traits.Effects;

public sealed partial class AddHiddenLayersEffect : BaseTraitEffect
{
    [DataField(required: true)]
    public List<HumanoidVisualLayers> Layers = [];

    public override void Apply(TraitEffectContext ctx)
    {
        if (!ctx.EntMan.TryGetComponent<HumanoidAppearanceComponent>(ctx.Player, out var comp))
            return;

        foreach (var layer in Layers)
            if (!comp.HideLayersOnEquip.Contains(layer))
                comp.HideLayersOnEquip.Add(layer);

        ctx.EntMan.Dirty(ctx.Player, comp);
    }
}
