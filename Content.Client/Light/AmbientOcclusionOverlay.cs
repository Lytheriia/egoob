using System.Numerics;
using Content.Client.Graphics;
using Content.Client.Light.EntitySystems;
using Content.Shared.CCVar;
using Content.Shared.Light.Components;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Physics;
using Robust.Shared.Utility;

namespace Content.Client.Light;

/// <summary>
/// Applies ambient-occlusion to the viewport.
/// </summary>
public sealed class AmbientOcclusionOverlay : Overlay
{
    private static readonly ProtoId<ShaderPrototype> UnshadedShader = "unshaded";
    private static readonly ProtoId<ShaderPrototype> StencilMaskShader = "StencilMask";
    private static readonly ProtoId<ShaderPrototype> StencilEqualDrawShader = "StencilEqualDraw";

    [Dependency] private readonly IClyde _clyde = default!;
    [Dependency] private readonly IConfigurationManager _cfgManager = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowEntities;

    private readonly OverlayResourceCache<CachedResources> _resources = new ();

    // erida edit start
    private readonly OccluderSystem _occluders;
    private readonly SharedTransformSystem _xformSystem;
    private readonly GridStencilSystem _gridStencil;

    private Color _color;
    // erida edit end

    public AmbientOcclusionOverlay()
    {
        IoCManager.InjectDependencies(this);
        ZIndex = AfterLightTargetOverlay.ContentZIndex + 1;

        // erida edit start
        _occluders = _entManager.System<OccluderSystem>();
        _xformSystem = _entManager.System<SharedTransformSystem>();
        _gridStencil = _entManager.System<GridStencilSystem>();

        _cfgManager.OnValueChanged(CCVars.AmbientOcclusionColor, OnColorChanged, true);
        // erida edit end
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        /*
         * tl;dr
         * - we draw a black square on each "ambient occlusion" entity.
         * - we blur this.
         * - We apply it to the viewport.
         *
         * We do this while ignoring lighting because it will wash out the actual effect.
         * In 3D ambient occlusion is more complicated due top having to calculate normals but in 2D
         * we don't have a concept of depth / corners necessarily.
         */

        var viewport = args.Viewport;
        var mapId = args.MapId;
        var worldBounds = args.WorldBounds;
        var worldHandle = args.WorldHandle;
        var distance = _cfgManager.GetCVar(CCVars.AmbientOcclusionDistance);
        // erida edit start
        var resolutionScale = Math.Clamp(_cfgManager.GetCVar(CCVars.AmbientOcclusionResolutionScale), 0.1f, 1f);
        var target = viewport.RenderTarget;
        var aoSize = new Vector2i(
            Math.Max(1, (int) MathF.Ceiling(target.Size.X * resolutionScale)),
            Math.Max(1, (int) MathF.Ceiling(target.Size.Y * resolutionScale)));
        var lightScale = aoSize / (Vector2) viewport.Size;
        var scale = viewport.RenderScale / (Vector2.One / lightScale);
        var blurRadius = 14f * resolutionScale;
        var expandedBounds = worldBounds.Enlarged(GetBlurMargin(viewport, distance, blurRadius));
        // erida edit end

        var res = _resources.GetForViewport(args.Viewport, static _ => new CachedResources());

        if (res.AOTarget?.Texture.Size != aoSize)
        {
            res.AOTarget?.Dispose();
            res.AOTarget = _clyde.CreateRenderTarget(aoSize, new RenderTargetFormatParameters(RenderTargetColorFormat.Rgba8Srgb), name: "ambient-occlusion-target");
        }

        if (res.AOBlurBuffer?.Texture.Size != aoSize)
        {
            res.AOBlurBuffer?.Dispose();
            res.AOBlurBuffer = _clyde.CreateRenderTarget(aoSize, new RenderTargetFormatParameters(RenderTargetColorFormat.Rgba8Srgb), name: "ambient-occlusion-blur-target");
        }

        // Draw the texture data to the texture.
        // erida edit start: skip blur/stencil/composite when nothing occludes (open space views)
        var occluderState = new OccluderDrawState
        {
            Handle = worldHandle,
            Xform = _xformSystem,
            InvMatrix = res.AOTarget.GetWorldToLocalMatrix(viewport.Eye!, scale),
            Distance = distance,
        };
        args.WorldHandle.RenderInRenderTarget(res.AOTarget,
            () =>
            {
                worldHandle.UseShader(_proto.Index(UnshadedShader).Instance());
                worldHandle.SetTransform(Matrix3x2.Identity);
                _occluders.QueryAabb(ref occluderState, DrawOccluder, mapId, expandedBounds);
            }, Color.Transparent);

        if (!occluderState.Drew)
        {
            worldHandle.SetTransform(Matrix3x2.Identity);
            worldHandle.UseShader(null);
            return;
        }
        // erida edit end

        _clyde.BlurRenderTarget(viewport, res.AOTarget, res.AOBlurBuffer, viewport.Eye!, blurRadius); // erida edit

        // erida edit start
        var stencil = _gridStencil.GetNonSpaceStencil(args);

        // Draw the stencil texture to depth buffer.
        worldHandle.UseShader(_proto.Index(StencilMaskShader).Instance());
        worldHandle.DrawTextureRect(stencil.Texture, worldBounds);

        // Draw the Blurred AO texture finally.
        var color = _entManager.TryGetComponent(args.MapUid, out MapAmbientColorComponent? mapAmbient)
            ? mapAmbient.Color
            : _color;

        worldHandle.UseShader(_proto.Index(StencilEqualDrawShader).Instance());
        worldHandle.DrawTextureRect(res.AOTarget!.Texture, worldBounds, color);
        // erida edit end

        args.WorldHandle.SetTransform(Matrix3x2.Identity);
        args.WorldHandle.UseShader(null);
    }

    // erida edit start
    private void OnColorChanged(string value)
    {
        _color = Color.FromHex(value);
    }

    private static float GetBlurMargin(IClydeViewport viewport, float distance, float blurRadius)
    {
        if (viewport.Eye == null)
            return distance / EyeManager.PixelsPerMeter;

        var cameraSize = viewport.Eye.Zoom.Y * viewport.Size.Y * (1 / viewport.RenderScale.Y) / EyeManager.PixelsPerMeter;

        // Matches Clyde's BlurRenderTarget radius calculation closely enough to include off-screen AO contributors.
        return distance / EyeManager.PixelsPerMeter + blurRadius / cameraSize;
    }

    // erida edit start: zero-alloc occluder pass (HashSet-per-frame QueryAabb overload replaced)
    private struct OccluderDrawState
    {
        public DrawingHandleWorld Handle;
        public SharedTransformSystem Xform;
        public Matrix3x2 InvMatrix;
        public float Distance;
        public bool Drew;
    }

    private static bool DrawOccluder(ref OccluderDrawState state, in ComponentTreeEntry<OccluderComponent> entry)
    {
        state.Drew = true;
        DebugTools.Assert(entry.Component.Enabled);
        var matrix = state.Xform.GetWorldMatrix(entry.Transform);
        var localMatrix = Matrix3x2.Multiply(matrix, state.InvMatrix);

        state.Handle.SetTransform(localMatrix);
        // 4 pixels
        state.Handle.DrawRect(Box2.UnitCentered.Enlarged(state.Distance / EyeManager.PixelsPerMeter), Color.White);
        return true;
    }
    // erida edit end

    protected override void DisposeBehavior()
    {
        _cfgManager.UnsubValueChanged(CCVars.AmbientOcclusionColor, OnColorChanged);
        _resources.Dispose();

        base.DisposeBehavior();
    }
    // erida edit end

    private sealed class CachedResources : IDisposable
    {
        public IRenderTexture? AOTarget;
        public IRenderTexture? AOBlurBuffer;

        public void Dispose()
        {
            AOTarget?.Dispose();
            AOBlurBuffer?.Dispose();
        }
    }
}
