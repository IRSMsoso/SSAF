using Content.Server._SSAF.Parasite.Components;
using Content.Server.Popups;
using Content.Shared._SSAF.Parasite;
using Content.Shared.Actions;

namespace Content.Server._SSAF.Parasite;

/// <summary>
/// This handles...
/// </summary>
public sealed class ParasiteSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly PopupSystem _popup = default!;


    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ParasiteComponent, MapInitEvent>(OnInit);
        SubscribeLocalEvent<ParasiteComponent, ParasiteInfectHostActionEvent>(OnSpawnRift);
    }

    private void OnInit(EntityUid uid, ParasiteComponent component, MapInitEvent args)
    {
        _actions.AddAction(uid, ref component.InfectHostActionEntity, component.InfectHostAction);
    }

    private void OnSpawnRift(EntityUid uid, ParasiteComponent component, ParasiteInfectHostActionEvent args)
    {
        _popup.PopupEntity("It Worked!", uid, uid);

        // if (component.Rifts.Count >= RiftsAllowed)
        // {
        //     _popup.PopupEntity(Loc.GetString("carp-rift-max"), uid, uid);
        //     return;
        // }
        //
        // if (component.Rifts.Count > 0 && TryComp<DragonRiftComponent>(component.Rifts[^1], out var rift) && rift.State != DragonRiftState.Finished)
        // {
        //     _popup.PopupEntity(Loc.GetString("carp-rift-duplicate"), uid, uid);
        //     return;
        // }
        //
        // var xform = Transform(uid);
        //
        // // Have to be on a grid fam
        // if (!TryComp<MapGridComponent>(xform.GridUid, out var grid))
        // {
        //     _popup.PopupEntity(Loc.GetString("carp-rift-anchor"), uid, uid);
        //     return;
        // }
        //
        // // cant stack rifts near eachother
        // foreach (var (_, riftXform) in EntityQuery<DragonRiftComponent, TransformComponent>(true))
        // {
        //     if (_transform.InRange(riftXform.Coordinates, xform.Coordinates, RiftRange))
        //     {
        //         _popup.PopupEntity(Loc.GetString("carp-rift-proximity", ("proximity", RiftRange)), uid, uid);
        //         return;
        //     }
        // }
        //
        // // cant put a rift on solars
        // foreach (var tile in _map.GetTilesIntersecting(xform.GridUid.Value, grid, new Circle(_transform.GetWorldPosition(xform), RiftTileRadius), false))
        // {
        //     if (!tile.IsSpace(_tileDef))
        //         continue;
        //
        //     _popup.PopupEntity(Loc.GetString("carp-rift-space-proximity", ("proximity", RiftTileRadius)), uid, uid);
        //     return;
        // }
        //
        // var carpUid = Spawn(component.RiftPrototype, _transform.GetMapCoordinates(uid, xform: xform));
        // component.Rifts.Add(carpUid);
        // Comp<DragonRiftComponent>(carpUid).Dragon = uid;
        // component.HalftimePopupShown = false; // DeltaV - Reset popup flag
    }
}
