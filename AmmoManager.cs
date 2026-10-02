/*
 * R e a d m e
 * -----------
 * 
 * ## Setup
 * 
 * 1. Add your the value of lcd tag to the name of an lcd or block with lcd screen (e.g. cockpit). By default this is [AmmoMgrLCD] but you can 
 * 	configure this as well
 * 2. If you just want weapons summary, you are now done. Read on to find out how to customise what is displayed further
 * 
 * ## Config options
 * This script can NOT be configured by editing any variables directly. Read on to find out how to configure it properly
 * 
 * ### Per LCD
 * 
 * Configuration is done per surface via the block's custom data. To configure a specific surface 
 * do [AmmoMgr N] where N is the number of the surface. For example, for a standard LCD panel you need to write 
 * 
 * [AmmoMgr 0]
 * // Config goes here 
 * 
 * For a the fighter cockpit you would write 
 * 
 * [AmmoMgr 0]
 * // Center screen config 
 * 
 * [AmmoMgr 1]
 * // Bottom left screen config 
 * 
 * etc
 * 
 * The script will automagically write out the headers for each of your surfaces but you need to fill in the rest (if you want to change the config) 
 * that is, it will work without touching the custom data.
 * 
 * The config options are as follows: 
 * - type: Can be one of 
 * 	- Weapons: Shows a summary of all weapons 
 * 	- Containers: Shows a summary of all containers
 * 	- All: Shows a summary of both containers and weapons
 * 	- Engaged: Shows a summary of weapons currently engaged (i.e. firing or targeting)
 * - group: Full (case sensative) name of a group to filter by, only blocks in that group will be shown 
 * - offset x: Offset for the display in pixels, right is positive, left is negative
 * - offset y: Offset for the display in pixels, down is positive, up is negative
 * - scale: Scale of the display. Can be a decimal
 * - scroll: true or false, whether to scroll the display if all the contents cannot fit in the viewport
 * - oneline: true or false, whether to show the summary in a condensed one line per entry view. This view hides ammo breakdowns and only 
 * 			shows the block name and fill level 
 * - hide empty: true or false, whether to hide ammo entries where there is no ammo of that type in the block (without this, large containers will show
 * 				every ammo in your world)
 * 
 * ### Per Programmable Block 
 * 
 * The CustomData of the programmable block this script is installed on can be used for additional configuration. The section name is
 * AmmoMgr, e.g. 
 * [AmmoMgr]
 * // Config 
 * 
 * 
 * The current options are:
 * 
 * - lcd tag: Tag to use for LCD panels for displaying status. Defaults to [AmmoMgrLCD]
 * 	
 * 
 * ### Compatability with Isy's Inventory management script 
 * 
 * This script will fight the management script by default. You can fix this by adding "Locked" to the name of 
 * each weapon (e.g. with a renamer script). This will prevent Isy's from pulling items out of the block. 
 * 
 * 
 */

internal struct AmmoItemData
{
    public IMyInventory Parent;
    public MyInventoryItem Item;
}
internal enum Priority
{
    Unknown = -1,
    Container = 0,
    InactiveWeapon = 1,
    ActiveWeapon = 2,
};

internal enum StatusType
{
    //TotalAmmoSummary,
    WeaponsSummary,
    ContainerSummary,
    FullSummary,
    EngagedSummary,
    Invalid,
}
internal class StatusLCDData
{
    public StatusType Type;
    public string Group;
    public Vector2 OriginOffset;
    public Vector2 ScrollOffset;
    public float Scale = 1f;
    public bool ScrollingUp = false;
    public bool Scroll;
    public bool ShortMode;
    public bool HideZeroEntries;
}

internal enum ExecutionPoint
{
    Rebalancing,
    Scanning,

}


internal const string AMMO_TYPE_NAME = "MyObjectBuilder_AmmoMagazine";
internal const string VERSION = "0.5.3";

internal const int MAX_REBALANCE_TICKS = 60; // Increase this to slowdown the script and maybe improve perf with _lots_ of inventories
internal const uint INVENTORY_GROUPING_BATCH_SIZE = 100; // Controls how many inventories are grouped per tick during scan, lowering may fix too complex errors
internal const uint INVENTORY_FILTER_BATCH_SIZE = 100; // Controls how many inventories are filtered per tick during scan, lowering may fix too complex errors
internal const int TICKS_PER_COMP_UPDATE = 30;
internal int max_comp_since_ticks_ = 0;

internal HashSet<MyDefinitionId> wc_weapons_ = new HashSet<MyDefinitionId>();
internal Dictionary<IMyInventory, HashSet<MyItemType>> inv_allowlist_cache_ = new Dictionary<IMyInventory, HashSet<MyItemType>>();
internal List<List<IMyInventory>> partitioned_invs_ = new List<List<IMyInventory>>();
internal Dictionary<IMyInventory, Priority> requester_cache_ = new Dictionary<IMyInventory, Priority>();
internal Dictionary<string, List<AmmoItemData>> avaliability_lookup_ = new Dictionary<string, List<AmmoItemData>>();
internal List<string> actions_log_ = new List<string>();
internal Dictionary<StatusLCDData, List<IMyTextSurface>> status_lcds_ = new Dictionary<StatusLCDData, List<IMyTextSurface>>();
internal Dictionary<string, HashSet<IMyTerminalBlock>> block_groups_cache_ = new Dictionary<string, HashSet<IMyTerminalBlock>>();
internal Dictionary<IMyInventory, bool> cache_outdated_lookup_ = new Dictionary<IMyInventory, bool>();
internal List<IMyInventory> outdated_inv_store_ = new List<IMyInventory>();
internal List<MySprite> sprite_cache_ = new List<MySprite>();
internal List<IMyInventory> flat_inv_cache_ = new List<IMyInventory>();
internal StringBuilder lcd_data_cache_ = new StringBuilder();
internal MyIni status_lcd_parser_ = new MyIni();
internal WcPbApi wc_;
internal SpriteBuilder sbuilder_ = new SpriteBuilder();
internal string lcd_tag_;
internal uint ticks_per_inv_refresh_ = 1200; // Every 2 minutes
internal ulong tick_ = 0;
internal int flat_inventories_ = 0;

internal const string INI_SECT_NAME = "AmmoMgr";
internal static MyIniKey LCD_TAG_KEY = new MyIniKey(INI_SECT_NAME, "lcd tag");




Exception fatal_error_ = null;

internal ulong ticks_10 = 0;

internal Console console = new Console
{
    ClearOnPrint = true,
    Header = $"== AmmoMgr v{VERSION} ==",
};

internal static bool IsAmmo(MyItemType type)
{
    return type.TypeId == AMMO_TYPE_NAME;
}

internal static void SortItems(IMyInventory parent, List<MyInventoryItem> items, Dictionary<string, List<AmmoItemData>> readin)
{

    foreach(var item in items)
    {
        if (IsAmmo(item.Type))
        {
            List<AmmoItemData> target_set;
            var key = item.Type.SubtypeId.ToString();

            var found = true;
            if (!readin.TryGetValue(key, out target_set))
            {
                target_set = new List<AmmoItemData>();
                found = false;
            }

            target_set.Add(new AmmoItemData { Item = item, Parent = parent });
            if (!found)
            {
                readin.Add(key, target_set);
            }
        }
    }
}
internal static void ClearLists<T, V>(Dictionary<T, List<V>> dict)
{
    foreach (var val in dict.Values)
    {
        val.Clear();
    }
}

internal bool IsWeapon(IMyTerminalBlock entity)
{
    if (entity is IMyUserControllableGun)
    {
        return true;
    } else
    {
        var id = new MyDefinitionId(entity.BlockDefinition.TypeId, entity.BlockDefinition.SubtypeId);

        return wc_weapons_.Contains(id);
    }
}
internal HashSet<MyItemType> AcceptedItems(IMyInventory inv)
{
    HashSet<MyItemType> allowed;
    if (!inv_allowlist_cache_.TryGetValue(inv, out allowed))
    {
        allowed = new HashSet<MyItemType>();
        inv.GetAcceptedItems(null, t => {
            if (IsAmmo(t))
            {
                allowed.Add(t);
            }
            return false;
        });
        inv_allowlist_cache_.Add(inv, allowed);
    }
    return allowed;

}
internal bool CanContainItem(IMyInventory inv, MyItemType ammo)
{
    return AcceptedItems(inv).Contains(ammo);
}

internal bool CanContainAmmo(IMyInventory inv)
{
    AcceptedItems(inv); // populate cache if not already
    return inv_allowlist_cache_[inv].Count > 0;
}

internal bool IsRequester(IMyInventory inv)
{
    Priority priority;
    if (!requester_cache_.TryGetValue(inv, out priority))
    {
        return false;
    } else
    {
        return priority > 0;
    }
}

internal bool IsSameOrHigherPriority(IMyInventory target, IMyInventory than)
{
    return PriorityFor(target) >= PriorityFor(than);
}

internal Priority PriorityFor(IMyInventory inv)
{
    Priority p;
    if (!requester_cache_.TryGetValue(inv, out p))
    {
        return Priority.Unknown;
    }
    else
    {
        return p;
    }
}



internal static void ScanForWeapons(WcPbApi wc, ICollection<MyDefinitionId> readin)
{
    wc.GetAllCoreStaticLaunchers(readin);
    wc.GetAllCoreTurrets(readin);
    wc.GetAllCoreWeapons(readin);
}
internal void ScanForLCDs(Dictionary<StatusLCDData, List<IMyTextSurface>> readin)
{
    var search_str = lcd_tag_;

    var blocks_tmp = new List<IMyTextSurfaceProvider>();
    GridTerminalSystem.GetBlocksOfType(blocks_tmp);

    foreach (var prov in blocks_tmp)
    {
        var block = prov as IMyTerminalBlock;
        if (block != null)
        {
            var name = block.CustomName;
            if (name.Contains(search_str))
            {
                ParseStatusLCDData(block, prov, readin);
            }
        }
    }
}
internal string BuildStatusLCDSectionName(int index)
{
    return $"AmmoMgr {index}";
}
internal bool TryParseStatus(string input, out StatusType output)
{
    switch(input)
    {
        case "":
        case "Weapons":
            output = StatusType.WeaponsSummary;
            return true;
        case "Full":
            output = StatusType.FullSummary;
            return true;
        case "Engaged":
            output = StatusType.EngagedSummary;
            return true;
        case "Containers":
            output = StatusType.ContainerSummary;
            return true;
        default:
            output = StatusType.Invalid;
            return false;
    }
}
internal void ParseStatusLCDData(IMyTerminalBlock block, IMyTextSurfaceProvider prov, Dictionary<StatusLCDData, List<IMyTextSurface>> readin)
{
    status_lcd_parser_.Clear();
    if (block.CustomData.Length != 0 && !status_lcd_parser_.TryParse(content: block.CustomData))
    {
        console.Persistout.WriteLn($"{block.CustomName} has invalid custom data");
        return;
    }
    for(var i = 0; i != prov.SurfaceCount; ++i)
    {
        var sect = BuildStatusLCDSectionName(i);

        if (status_lcd_parser_.ContainsSection(sect))
        {
            StatusType type;
            TryParseStatus(status_lcd_parser_.Get(sect, "type").ToString(), out type);
            var group = status_lcd_parser_.Get(sect, "group").ToString(null);
            var offset_x = status_lcd_parser_.Get(sect, "offset x").ToInt32(0);
            var offset_y = status_lcd_parser_.Get(sect, "offset y").ToInt32(0);
            var origin_offset = new Vector2(offset_x, offset_y);
            var scale = status_lcd_parser_.Get(sect, "scale").ToDouble(1);
            var scroll = status_lcd_parser_.Get(sect, "scroll").ToBoolean(true);
            var short_m = status_lcd_parser_.Get(sect, "oneline").ToBoolean(false);

            var data = new StatusLCDData {
                Group = group,
                Type = type,
                OriginOffset =
                origin_offset,
                Scale = (float)scale,
                Scroll = scroll,
                ShortMode = short_m,
                HideZeroEntries = status_lcd_parser_.Get(sect, "hide empty").ToBoolean(true),
            };

            List<IMyTextSurface> surfaces;
            if (!readin.TryGetValue(data, out surfaces))
            {
                surfaces = new List<IMyTextSurface>();
                readin.Add(data, surfaces);
            }
            surfaces.Add(prov.GetSurface(i));
        } else
        {
            status_lcd_parser_.AddSection(sect);
        }
    }
    block.CustomData = status_lcd_parser_.ToString();




}

internal void ScanGroups()
{
    block_groups_cache_.Clear();
    GridTerminalSystem.GetBlockGroups(null, g => {
        var set = new HashSet<IMyTerminalBlock>();
        g.GetBlocks(null, b => { set.Add(b); return false; });
        block_groups_cache_.Add(g.Name, set);


        return false; });
}

internal void ParseConfig()
{
    status_lcd_parser_.Clear();
    if (!status_lcd_parser_.TryParse(Me.CustomData))
    {
        console.Persistout.WriteLn("[WARN]: PB has invalid custom data, fix it then recompile to have config applied");
        return;
    }

    lcd_tag_ = status_lcd_parser_.Get(LCD_TAG_KEY).ToString("AmmoMgrLCD");

    status_lcd_parser_.Clear();
}


internal void WriteStatsToStdout()
{
    var complexity = Runtime.CurrentInstructionCount;

    if (max_comp_since_ticks_ < complexity)
    {
        max_comp_since_ticks_ = complexity;
    }
    console.Stdout.WriteLn($"Status displays: {status_lcds_.Count}");
    console.Stdout.WriteLn($"Inventories: {flat_inventories_}");
    console.Stdout.WriteLn($"Groups: {partitioned_invs_.Count}");
    console.Stdout.WriteLn($"Complexity: {max_comp_since_ticks_} / {Runtime.MaxInstructionCount}");
    foreach(var act in actions_log_)
    {
        console.Stdout.WriteLn(act);
    }
    if (tick_ % TICKS_PER_COMP_UPDATE == 0)
    {
        max_comp_since_ticks_ = 0;
    }

}


internal static string OwnerName(IMyInventory inv)
{
    var owner = inv?.Owner as IMyTerminalBlock;
    return owner?.CustomName ?? "";
}
internal bool IsValidInventory(IMyInventory inv)
{
    var parent = inv.Owner as IMyTerminalBlock;
    return parent != null && Me.IsSameConstructAs(parent) && CanContainAmmo(inv);
}
internal void CullWeaponlessGroups(List<List<IMyInventory>> groups)
{
    groups.RemoveAll(group =>
    {
        return group.All(inv => !IsWeapon((IMyTerminalBlock)inv.Owner));
    });
}

internal IEnumerator<bool> RefreshInventories(List<List<IMyInventory>> readin)
{
    RemoveOutdatedInvs();
    flat_inv_cache_.Clear();
    var cache = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(cache);
    flat_inventories_ = 0;
    foreach (var b in Schedular.DistributeForEach(cache, INVENTORY_FILTER_BATCH_SIZE, b =>
    {
        if (b.HasInventory && IsValidInventory(b.GetInventory()))
        {
            flat_inv_cache_.Add(b.GetInventory());
            ++flat_inventories_; // so it displays live updates in the stats
        }
    }))
    {
        yield return false;
    }
    cache.Clear();
    yield return false;


    flat_inventories_ = flat_inv_cache_.Count;
    while (flat_inv_cache_.Count > 0)
    {
        for (var n = 0; n != INVENTORY_GROUPING_BATCH_SIZE; ++n)
        {
            if (flat_inv_cache_.Count == 0)
            {
                break;
            }
            var inv = flat_inv_cache_.Pop();
            var partition = new List<IMyInventory>(); // we are _always_ first
            readin.Add(partition);
            partition.Add(inv);
            cache_outdated_lookup_[inv] = false;
            for(var i = 0; i != flat_inv_cache_.Count; i++)
            {
                var peer = flat_inv_cache_[i];
                if (inv.IsConnectedTo(peer))
                {
                    partition.Add(peer);
                    cache_outdated_lookup_[inv] = false;
                    flat_inv_cache_.RemoveAtFast(i);
                    --i;
                }
            }
        }
        yield return false;
    }
    CullWeaponlessGroups(readin);

    yield break;

}
internal void AllotItems(double per_inv, List<AmmoItemData> avaliable, List<IMyInventory> requesters)
{

    if (avaliable.Count == 0)
    {
        return;
    }

    var ammo_t = avaliable[0].Item.Type;

    var aval_head = 0;
    foreach (var inv in requesters)
    {
        var to_block = inv.Owner as IMyTerminalBlock;
        var needed = per_inv;
        if (
           (to_block != null && to_block.IsWorking)
        && ((double)inv.GetItemAmount(ammo_t) < per_inv)
        && CanContainItem(inv, ammo_t)
        )
        {
            for (var i = aval_head; i < avaliable.Count; ++i)
            {

                var target_item = avaliable[i];
                if (ammo_t != target_item.Item.Type)
                {
                    throw new Exception("All avaliable must have same ammo type");
                }

                var from_inv = target_item.Parent;
                if (
                    IsSameOrHigherPriority(inv, from_inv)
                    && (PriorityFor(from_inv) < PriorityFor(inv) || (double)from_inv.GetItemAmount(target_item.Item.Type) > per_inv)
                    && from_inv.IsConnectedTo(inv)
                    )
                {
                    var aval = Math.Min(needed, (double)target_item.Item.Amount);

                    inv.TransferItemFrom(target_item.Parent, target_item.Item, (MyFixedPoint)aval);

                    needed -= aval;
                    if (needed > 0)
                    {
                        ++aval_head;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}

internal IEnumerable<IMyInventory> FilterByCanContain(List<IMyInventory> src, MyItemType type)
{
    return src.Where(i => CanContainItem(i, type));
}

private double CalcPerInv(MyItemType ammo_t, List<IMyInventory> inv_system)
{
    var nb_req = 0;
    var total_qty = 0.0;
    foreach (var inv in inv_system)
    {
        if (CanContainItem(inv, ammo_t))
        {
            if (IsRequester(inv))
            {
                ++nb_req;
            }
            total_qty += (double)inv.GetItemAmount(ammo_t);
        }
    }

    var per_inv = Math.Floor(Math.Round(total_qty / nb_req, 1));

    return per_inv;

}

internal IEnumerator<bool> RebalanceInventories(List<List<IMyInventory>> requesters, Dictionary<string, List<AmmoItemData>> avaliable)
{
    var per_yield = MAX_REBALANCE_TICKS / (double)requesters.Count;
    var since_yield = 0.0;
    foreach (var ammo in avaliable)
    {
        if (ammo.Value.Count != 0)
        {

            foreach (var inv_system in requesters)
            {
                if (since_yield >= 1.0)
                {
                    since_yield = 0;
                    yield return false;
                }
                since_yield += per_yield;
                var per_inv = CalcPerInv(ammo.Value[0].Item.Type, inv_system);
                AllotItems(per_inv, ammo.Value, inv_system);
            }
        }

    }


}

readonly List<MyInventoryItem> items_tmp_ = new List<MyInventoryItem>();
internal void ScanInventories(List<List<IMyInventory>> inventories, Dictionary<string, List<AmmoItemData>> readin)
{
    foreach(var part in inventories)
    {
        foreach (var inv in part)
        {
            cache_outdated_lookup_[inv] = true;
            items_tmp_.Clear();
            inv.GetItems(items_tmp_);

            SortItems(inv, items_tmp_, readin);
        }

    }
}

internal void RefreshTargetingStatus()
{


    foreach(var parition in partitioned_invs_)
    {
        foreach(var inv in parition)
        {
            var inv_parent = inv.Owner as IMyTerminalBlock;
            if (inv_parent != null && IsWeapon(inv_parent))
            {
                var priority = Priority.InactiveWeapon;
                if (wc_ != null && wc_weapons_.Contains(inv_parent.BlockDefinition))
                {
                    var curr_target = wc_.GetWeaponTarget(inv_parent);


                    if (!(curr_target == null || curr_target.Value.EntityId == 0 || !wc_.CanShootTarget(inv_parent, ((MyDetectedEntityInfo)curr_target).EntityId, 0)))
                    {
                        priority = Priority.ActiveWeapon;
                    }
                } else if ((inv_parent as IMyUserControllableGun)?.IsShooting ?? false)
                {
                    priority = Priority.ActiveWeapon;
                }

                requester_cache_[inv] = priority;
            }

        }

    }

}




internal void RemoveOutdatedInvs()
{
    outdated_inv_store_.Clear();
    foreach(var kh in cache_outdated_lookup_)
    {
        if (kh.Value)
        {
            outdated_inv_store_.Add(kh.Key);
        }
    }
    foreach(var outdated in outdated_inv_store_)
    {
        cache_outdated_lookup_.Remove(outdated);
        partitioned_invs_.FirstOrDefault(p => p.Contains(outdated))?.Remove(outdated);
    }
    outdated_inv_store_.Clear();
}


public Program()
{
    ParseConfig();
    var wc = new WcPbApi();
    bool has_wc = false;
    try
    {
        wc.Activate(Me);
        has_wc = true;
    } catch(Exception)
    {
        console.Persistout.WriteLn($"Failed to initalise WeaponCore, falling back to vanilla only");
    }
    if (has_wc)
    {
        ScanForWeapons(wc, wc_weapons_);
        wc_ = wc;
        console.Persistout.WriteLn($"Using WeaponCore weapons as well as vanilla");
    }
    ScanForLCDs(status_lcds_);
    ScanGroups();

    sm_instructions_.Enqueue(RefreshInventories(partitioned_invs_));

    Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Once;

}

public void Save()
{
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means.
    //
    // This method is optional and can be removed if not
    // needed.
}
readonly Queue<IEnumerator<bool>> sm_instructions_ = new Queue<IEnumerator<bool>>();

private IEnumerator<bool> Tickover()
{
    actions_log_.Clear();
    ClearLists(avaliability_lookup_);
    ScanInventories(partitioned_invs_, avaliability_lookup_);

    return RebalanceInventories(partitioned_invs_, avaliability_lookup_);

}

public void Main(string argument, UpdateType updateSource)
{
    if (fatal_error_ != null)
    {
        console.Stderr.WriteLn($"== Fatal Error ==\nReason: {fatal_error_.Message}\n\n-- Stack Trace --\n{fatal_error_.StackTrace}");
    }
    else
    {

        try
        {
            ++tick_;
            if ((updateSource & UpdateType.Update10) == UpdateType.Update10)
            {

                DrawStatus();
                RefreshTargetingStatus();
            }

            var is_oneshot = (updateSource & UpdateType.Once) == UpdateType.Once;
            if (is_oneshot && sm_instructions_.Count != 0)
            {

                var curr = sm_instructions_.Peek();
                if (curr.MoveNext())
                {
                } else
                {
                    sm_instructions_.Dequeue().Dispose();
                }
                Runtime.UpdateFrequency |= UpdateFrequency.Once;

            }
            if (sm_instructions_.Count == 0)
            {
                sm_instructions_.Enqueue(Tickover());
                Runtime.UpdateFrequency |= UpdateFrequency.Once;
            }

            if (tick_ % ticks_per_inv_refresh_ == 0 && sm_instructions_.Count <= 1)
            {
                sm_instructions_.Enqueue(RefreshInventories(partitioned_invs_));
                Runtime.UpdateFrequency |= UpdateFrequency.Once;
            }

            WriteStatsToStdout();
        }
        catch (Exception e)
        {
            fatal_error_ = e;
            Runtime.UpdateFrequency = UpdateFrequency.Once | UpdateFrequency.Update100;
        }
    }

    console.PrintOutput(this);

}


internal void DrawStatus()
{
    foreach (var kh in status_lcds_)
    {
        DrawStatusFor(kh.Key, kh.Value);
    }
}

internal void DrawStatusFor(StatusLCDData data, List<IMyTextSurface> surfaces)
{
    lcd_data_cache_.Clear();

    foreach (var surface in surfaces)
    {
        surface.ContentType = ContentType.SCRIPT;
        surface.Script = string.Empty;
        var viewport = new RectangleF(new Vector2(0, (surface.TextureSize.Y - surface.SurfaceSize.Y) / 2f) + data.OriginOffset, surface.SurfaceSize);
        var pos = viewport.Position + data.ScrollOffset;

        var frame = surface.DrawFrame();
        sbuilder_.CurrPos = pos;
        sbuilder_.Scale = data.Scale;
        sbuilder_.Viewport = viewport;
        sbuilder_.Surface = surface;

        var end_pos = AppendTxtFor(data, ref frame);

        if (data.Scroll && !viewport.Contains(end_pos) || data.ScrollOffset.LengthSquared() > 0)
        {
            if (!data.ScrollingUp && viewport.Position.Y + viewport.Size.Y < end_pos.Y)
            {
                data.ScrollOffset -= new Vector2(0, 10);
            }
            else if (data.ScrollingUp && viewport.Position.Y + 5 < pos.Y)
            {
                data.ScrollingUp = false;
            }
            else
            {
                data.ScrollOffset += new Vector2(0, 50);
                data.ScrollingUp = true;
            }
        }


        frame.Dispose();

    }

    lcd_data_cache_.Clear();
}
internal static Color ColourForProg(int prog)
{
    return prog > 80 ? Color.Green : prog > 50 ? Color.Orange : prog > 30 ? Color.OrangeRed : prog > 10 ? Color.Red : Color.DarkRed;
}

internal Vector2 AppendForWepSummary(ref MySpriteDrawFrame frame, StatusLCDData data, Func<IMyTerminalBlock, bool> accept_filt)
{
    HashSet<IMyTerminalBlock> filter_group = null;
    var filter_group_name = data.Group;
    if (filter_group_name != null)
    {
        block_groups_cache_.TryGetValue(filter_group_name, out filter_group);
    }

    sprite_cache_.Clear();
    var to = sprite_cache_;
    foreach (var wep_group in partitioned_invs_)
    {
        var box_border = new SpriteBuilder.BoxedProxy(sbuilder_);
        foreach (var wep in wep_group)
        {
            var owner_block = wep.Owner as IMyTerminalBlock;
            if (accept_filt(owner_block) && (filter_group == null || filter_group.Contains(owner_block)))
            {
                var title_txt = $"[ {owner_block.CustomName} ]";
                to.Add(sbuilder_.MakeText(title_txt));


                SpriteBuilder.IndentProxy? maybe_indent = null;
                if (data.ShortMode)
                {
                    maybe_indent = sbuilder_.WithIndent((int)(sbuilder_.TextSizePx(title_txt).X + sbuilder_.NewlineHeight));
                }
                else
                {
                    sbuilder_.AddNewline();
                }
                var aval = wep.MaxVolume;

                sbuilder_.MakeProgressBar(
                    to: to,
                    size:  new Vector2(sbuilder_.Viewport.Size.X / 6, 2 * (SpriteBuilder.NEWLINE_HEIGHT_BASE / 3)),
                    bg: Color.White,
                    fg: ColourForProg((int)((double)wep.CurrentVolume / (double)aval * 100)),
                    curr: (double)wep.CurrentVolume, total: (double)aval
                    );
                maybe_indent?.Dispose();


                sbuilder_.AddNewline();
                if (!data.ShortMode)
                {
                    using (var idn1 = sbuilder_.WithIndent(20))
                    {
                        var accepted = AcceptedItems(wep);
                        foreach (var accept in accepted)
                        {
                            var qty = wep.GetItemAmount(accept);
                            if (!data.HideZeroEntries || qty > 0)
                            {
                                to.Add(sbuilder_.MakeBulletPt());
                                to.Add(sbuilder_.MakeText($"{accept.SubtypeId}: {(double)qty:00}", offset: new Vector2(sbuilder_.NewlineHeight / 2, 0)));
                                sbuilder_.AddNewline();
                            }

                        }
                        sbuilder_.AddNewline();
                    }
                }
            }
        }
        if (sprite_cache_.Count != 0)
        {
            box_border.Make(ref frame, (int)sbuilder_.Viewport.Size.X, 10);

            frame.AddRange(sprite_cache_);

            sprite_cache_.Clear();

            sbuilder_.AddNewline();
            sbuilder_.AddNewline();
        }
    }
    return sbuilder_.CurrPos;
}
internal Vector2 AppendTxtFor(StatusLCDData data, ref MySpriteDrawFrame to)
{
    Func<IMyTerminalBlock, bool> filter_act = null;
    var status = data.Type;
    switch (status)
    {
        case StatusType.WeaponsSummary:
            filter_act = b => b != null && IsWeapon(b);
            break;
        case StatusType.ContainerSummary:
            filter_act = b => b != null && !IsWeapon(b);
            break;
        case StatusType.FullSummary:
            filter_act = b => b != null;
            break;
        case StatusType.EngagedSummary:
            filter_act = b => b != null && PriorityFor(b.GetInventory()) >= Priority.ActiveWeapon;
            break;

    }

    if (filter_act != null)
    {
        return AppendForWepSummary(ref to, data, filter_act);
    }
    else
    {
        to.Add(sbuilder_.MakeText("Invalid status type in custom data", alignment: TextAlignment.CENTER, color: Color.Red));
        return Vector2.Zero;
    }
}

public class Console
{

    public void PrintOutput(MyGridProgram prog)
    {
        prog.Echo(Header);
        Persistout.PrintTo(prog);
        Stderr.PrintTo(prog);
        Stdout.PrintTo(prog);

        if (ClearOnPrint)
        {
            Stderr.Clear();
            Stdout.Clear();
        }
    }

    public bool ClearOnPrint = false;
    public string Header = string.Empty;
    public IOStream Stdout = new IOStream();
    public IOStream Stderr = new IOStream() { Prefix = "[Error]: " };
    public IOStream Persistout = new IOStream();

}

public class IOStream
{

    public void Write(string data)
    {
        buffer.Append(Prefix + data);
    }
    public void WriteLn(string data)
    {
        Write(data + "\n");
    }

    public void PrintTo(MyGridProgram prog)
    {
        prog.Echo(buffer.ToString());
    }

    public void Clear()
    {
        buffer.Clear();
    }

    public double Length
    {
        get { return buffer.Length; }
    }

    public string Prefix { get; set; }
    private readonly StringBuilder buffer = new StringBuilder();

}

public class SpriteBuilder
{
    public struct IndentProxy: IDisposable {
        internal SpriteBuilder parent_;
        internal int by_;
        internal IndentProxy(SpriteBuilder parent, int by)
        {
            parent_ = parent;
            by_ = by;
            parent.CurrPos.X += by;
        }

        public void Dispose()
        {
            parent_.CurrPos.X -= by_;
        }
    }
    public struct BoxedProxy {
        internal SpriteBuilder parent_;
        internal Vector2 origin_;
        public BoxedProxy(SpriteBuilder parent)
        {
            parent_ = parent;
            origin_ = parent.CurrPos;
        }

        public void Make(ref MySpriteDrawFrame to, int width, int line_width)
        {
            var size = new RectangleF(origin_, new Vector2( width, parent_.CurrPos.Y - origin_.Y));

            to.Add(parent_.MakeRect( // Right
                start: new Vector2(origin_.X + width - line_width, origin_.Y),
                end: new Vector2(origin_.X + width, parent_.CurrPos.Y)
                ));
        }
    }


    public float Scale;
    public RectangleF Viewport;
    public Vector2 CurrPos;
    public IMyTextSurface Surface;


    internal Vector2 NewlineCenterOffset => new Vector2(0, NewlineHeight / 4);
    internal StringBuilder str_cache_;

    public float NewlineHeight => NEWLINE_HEIGHT_BASE * Scale;

    public const float NEWLINE_HEIGHT_BASE = 37f;

    public Vector2 TextSizePx(string txt, IMyTextSurface reference = null,  string font_id = "White")
    {
        reference = reference ?? Surface;
        if (str_cache_ == null)
        {
            str_cache_ = new StringBuilder();
        } else
        {
            str_cache_.Clear();
        }

        str_cache_.Append(txt);
        var size = reference.MeasureStringInPixels(str_cache_, font_id, Scale);
        str_cache_.Clear();
        return size;

    }

    public IndentProxy WithIndent(int by)
    {
        return new IndentProxy(this, by);
    }

    public void AddIndent(int by)
    {
        CurrPos.X += by;
    }
    public void AddNewline()
    {
        CurrPos.Y += NewlineHeight;
    }
    public MySprite MakeRect(Vector2 start, Vector2 end, Color? fg = null)
    {
        var rect = new RectangleF(start, end - start);
        var colour = fg ?? Color.White;
        return new MySprite
        {
            Type = SpriteType.TEXTURE,
            Data = "SquareSimple",
            Color = colour,
            Size = rect.Size,
            Position = rect.Center,
            Alignment = TextAlignment.CENTER,

        };
    }
    public MySprite MakeBulletPt(Color? fg = null)
    {
        var color = fg ?? Color.White;
        var bounds = new RectangleF(CurrPos, new Vector2(NewlineHeight / 3, NewlineHeight / 3));
        return new MySprite
        {
            Data = "Circle",
            Type = SpriteType.TEXTURE,
            Color = color,
            Alignment = TextAlignment.CENTER,
            Position = bounds.Center + NewlineCenterOffset,
            Size = bounds.Size,
        };
    }

    public void MakeProgressBar(ICollection<MySprite> to, Vector2 size, Color bg, Color fg, double curr, double total)
    {
        var padding = new Vector2(2, 2);
        size *= Scale;

        var bg_rect = new RectangleF(CurrPos, size);

        var sprite = new MySprite
        {
            Type = SpriteType.TEXTURE,
            Data = "SquareTapered",
            Position = bg_rect.Center + NewlineCenterOffset,
            Color = bg,
            Alignment = TextAlignment.CENTER,
            Size = bg_rect.Size,
        };
        to.Add(sprite);

        var fg_rect = new RectangleF(CurrPos + padding / 2, new Vector2((float)(curr * (size.X / total)), size.Y) - padding);

        sprite = new MySprite
        {
            Type = SpriteType.TEXTURE,
            Data = "SquareSimple",
            Position = fg_rect.Center + NewlineCenterOffset,
            Color = fg,
            Alignment = TextAlignment.CENTER,
            Size = fg_rect.Size,
        };
        to.Add(sprite);
        var txt_rect = new RectangleF(bg_rect.Position + new Vector2(bg_rect.Size.X + 5, 0), new Vector2(90, 0));
        to.Add(MakeText(
            txt: $"{curr / total * 100:00}%",
            offset: new Vector2(bg_rect.Size.X + NewlineHeight, bg_rect.Size.Y / 4),
            alignment: TextAlignment.CENTER
            )
        );

    }

    public MySprite MakeText(string txt, TextAlignment alignment = TextAlignment.LEFT, Color? color = null, string font_id = "White", Vector2? offset = null)
    {
        var roffset = offset ?? Vector2.Zero;
        return new MySprite
        {
            Type = SpriteType.TEXT,
            Data = txt,
            Alignment = alignment,
            RotationOrScale = Scale,
            FontId = font_id,
            Color = color ?? Color.White,
            Position = CurrPos + roffset,
        };
    }

}

}
internal class Schedular
{
    public static IEnumerable<bool> DistributeForEach<T>(ICollection<T> collection, uint partSize, Action<T> func)
    {
        var iter = collection.GetEnumerator();
        iter.MoveNext();
        var parts = Math.Ceiling((double)collection.Count / (double)partSize);
        for (var n = 0; n < parts; n++)
        {
            var max = Math.Min((n + 1) * partSize, collection.Count);
            for (var i = n * partSize; i < max; i++)
            {
                var b = iter.Current;
                func(b);
                iter.MoveNext();
            }
            yield return false;
        }
    }
}

/// <summary>
    /// https://github.com/sstixrud/CoreSystems/blob/master/BaseData/Scripts/CoreSystems/Api/CoreSystemsPbApi.cs
    /// </summary>
public class WcPbApi
{
    private Action<ICollection<MyDefinitionId>> _getCoreWeapons;
    private Action<ICollection<MyDefinitionId>> _getCoreStaticLaunchers;
    private Action<ICollection<MyDefinitionId>> _getCoreTurrets;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, IDictionary<string, int>, bool> _getBlockWeaponMap;
    private Func<long, MyTuple<bool, int, int>> _getProjectilesLockedOn;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, IDictionary<MyDetectedEntityInfo, float>> _getSortedThreats;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, ICollection<Sandbox.ModAPI.Ingame.MyDetectedEntityInfo>> _getObstructions;
    private Func<long, int, MyDetectedEntityInfo> _getAiFocus;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, bool> _setAiFocus;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, MyDetectedEntityInfo> _getWeaponTarget;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int> _setWeaponTarget;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, bool, int> _fireWeaponOnce;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, bool, bool, int> _toggleWeaponFire;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, bool, bool, bool> _isWeaponReadyToFire;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, float> _getMaxWeaponRange;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, ICollection<string>, int, bool> _getTurretTargetTypes;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, ICollection<string>, int> _setTurretTargetTypes;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, float> _setBlockTrackingRange;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, bool> _isTargetAligned;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, MyTuple<bool, Vector3D?>> _isTargetAlignedExtended;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, bool> _canShootTarget;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, Vector3D?> _getPredictedTargetPos;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, float> _getHeatLevel;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, float> _currentPowerConsumption;
    private Func<MyDefinitionId, float> _getMaxPower;
    private Func<long, bool> _hasGridAi;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, bool> _hasCoreWeapon;
    private Func<long, float> _getOptimalDps;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, string> _getActiveAmmo;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, string> _setActiveAmmo;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Action<long, int, ulong, long, Vector3D, bool>> _monitorProjectile;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Action<long, int, ulong, long, Vector3D, bool>> _unMonitorProjectile;
    private Func<ulong, MyTuple<Vector3D, Vector3D, float, float, long, string>> _getProjectileState;
    private Func<long, float> _getConstructEffectiveDps;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long> _getPlayerController;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Matrix> _getWeaponAzimuthMatrix;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Matrix> _getWeaponElevationMatrix;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, bool, bool, bool> _isTargetValid;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, MyTuple<Vector3D, Vector3D>> _getWeaponScope;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, MyTuple<bool, bool>> _isInRange;

    public bool Activate(Sandbox.ModAPI.Ingame.IMyTerminalBlock pbBlock)
    {
        var dict = pbBlock.GetProperty("WcPbAPI")?.As<IReadOnlyDictionary<string, Delegate>>().GetValue(pbBlock);
        if (dict == null) throw new Exception("WcPbAPI failed to activate");
        return ApiAssign(dict);
    }

    public bool ApiAssign(IReadOnlyDictionary<string, Delegate> delegates)
    {
        if (delegates == null)
            return false;

        AssignMethod(delegates, "GetCoreWeapons", ref _getCoreWeapons);
        AssignMethod(delegates, "GetCoreStaticLaunchers", ref _getCoreStaticLaunchers);
        AssignMethod(delegates, "GetCoreTurrets", ref _getCoreTurrets);
        AssignMethod(delegates, "GetBlockWeaponMap", ref _getBlockWeaponMap);
        AssignMethod(delegates, "GetProjectilesLockedOn", ref _getProjectilesLockedOn);
        AssignMethod(delegates, "GetSortedThreats", ref _getSortedThreats);
        AssignMethod(delegates, "GetObstructions", ref _getObstructions);
        AssignMethod(delegates, "GetAiFocus", ref _getAiFocus);
        AssignMethod(delegates, "SetAiFocus", ref _setAiFocus);
        AssignMethod(delegates, "GetWeaponTarget", ref _getWeaponTarget);
        AssignMethod(delegates, "SetWeaponTarget", ref _setWeaponTarget);
        AssignMethod(delegates, "FireWeaponOnce", ref _fireWeaponOnce);
        AssignMethod(delegates, "ToggleWeaponFire", ref _toggleWeaponFire);
        AssignMethod(delegates, "IsWeaponReadyToFire", ref _isWeaponReadyToFire);
        AssignMethod(delegates, "GetMaxWeaponRange", ref _getMaxWeaponRange);
        AssignMethod(delegates, "GetTurretTargetTypes", ref _getTurretTargetTypes);
        AssignMethod(delegates, "SetTurretTargetTypes", ref _setTurretTargetTypes);
        AssignMethod(delegates, "SetBlockTrackingRange", ref _setBlockTrackingRange);
        AssignMethod(delegates, "IsTargetAligned", ref _isTargetAligned);
        AssignMethod(delegates, "IsTargetAlignedExtended", ref _isTargetAlignedExtended);
        AssignMethod(delegates, "CanShootTarget", ref _canShootTarget);
        AssignMethod(delegates, "GetPredictedTargetPosition", ref _getPredictedTargetPos);
        AssignMethod(delegates, "GetHeatLevel", ref _getHeatLevel);
        AssignMethod(delegates, "GetCurrentPower", ref _currentPowerConsumption);
        AssignMethod(delegates, "GetMaxPower", ref _getMaxPower);
        AssignMethod(delegates, "HasGridAi", ref _hasGridAi);
        AssignMethod(delegates, "HasCoreWeapon", ref _hasCoreWeapon);
        AssignMethod(delegates, "GetOptimalDps", ref _getOptimalDps);
        AssignMethod(delegates, "GetActiveAmmo", ref _getActiveAmmo);
        AssignMethod(delegates, "SetActiveAmmo", ref _setActiveAmmo);
        AssignMethod(delegates, "MonitorProjectile", ref _monitorProjectile);
        AssignMethod(delegates, "UnMonitorProjectile", ref _unMonitorProjectile);
        AssignMethod(delegates, "GetProjectileState", ref _getProjectileState);
        AssignMethod(delegates, "GetConstructEffectiveDps", ref _getConstructEffectiveDps);
        AssignMethod(delegates, "GetPlayerController", ref _getPlayerController);
        AssignMethod(delegates, "GetWeaponAzimuthMatrix", ref _getWeaponAzimuthMatrix);
        AssignMethod(delegates, "GetWeaponElevationMatrix", ref _getWeaponElevationMatrix);
        AssignMethod(delegates, "IsTargetValid", ref _isTargetValid);
        AssignMethod(delegates, "GetWeaponScope", ref _getWeaponScope);
        AssignMethod(delegates, "IsInRange", ref _isInRange);
        return true;
    }

    private void AssignMethod<T>(IReadOnlyDictionary<string, Delegate> delegates, string name, ref T field) where T : class
    {
        if (delegates == null) {
            field = null;
            return;
        }

        Delegate del;
        if (!delegates.TryGetValue(name, out del))
            throw new Exception($"{GetType().Name} :: Couldn't find {name} delegate of type {typeof(T)}");

        field = del as T;
        if (field == null)
            throw new Exception(
                $"{GetType().Name} :: Delegate {name} is not type {typeof(T)}, instead it's: {del.GetType()}");
    }

    public void GetAllCoreWeapons(ICollection<MyDefinitionId> collection) => _getCoreWeapons?.Invoke(collection);

    public void GetAllCoreStaticLaunchers(ICollection<MyDefinitionId> collection) =>
        _getCoreStaticLaunchers?.Invoke(collection);

    public void GetAllCoreTurrets(ICollection<MyDefinitionId> collection) => _getCoreTurrets?.Invoke(collection);

    public bool GetBlockWeaponMap(Sandbox.ModAPI.Ingame.IMyTerminalBlock weaponBlock, IDictionary<string, int> collection) =>
        _getBlockWeaponMap?.Invoke(weaponBlock, collection) ?? false;

    public MyTuple<bool, int, int> GetProjectilesLockedOn(long victim) =>
        _getProjectilesLockedOn?.Invoke(victim) ?? new MyTuple<bool, int, int>();

    public void GetSortedThreats(Sandbox.ModAPI.Ingame.IMyTerminalBlock pBlock, IDictionary<MyDetectedEntityInfo, float> collection) =>
        _getSortedThreats?.Invoke(pBlock, collection);
    public void GetObstructions(Sandbox.ModAPI.Ingame.IMyTerminalBlock pBlock, ICollection<Sandbox.ModAPI.Ingame.MyDetectedEntityInfo> collection) =>
        _getObstructions?.Invoke(pBlock, collection);
    public MyDetectedEntityInfo? GetAiFocus(long shooter, int priority = 0) => _getAiFocus?.Invoke(shooter, priority);

    public bool SetAiFocus(Sandbox.ModAPI.Ingame.IMyTerminalBlock pBlock, long target, int priority = 0) =>
        _setAiFocus?.Invoke(pBlock, target, priority) ?? false;

    public MyDetectedEntityInfo? GetWeaponTarget(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId = 0) =>
        _getWeaponTarget?.Invoke(weapon, weaponId);

    public void SetWeaponTarget(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long target, int weaponId = 0) =>
        _setWeaponTarget?.Invoke(weapon, target, weaponId);

    public void FireWeaponOnce(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, bool allWeapons = true, int weaponId = 0) =>
        _fireWeaponOnce?.Invoke(weapon, allWeapons, weaponId);

    public void ToggleWeaponFire(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, bool on, bool allWeapons, int weaponId = 0) =>
        _toggleWeaponFire?.Invoke(weapon, on, allWeapons, weaponId);

    public bool IsWeaponReadyToFire(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId = 0, bool anyWeaponReady = true,
        bool shootReady = false) =>
        _isWeaponReadyToFire?.Invoke(weapon, weaponId, anyWeaponReady, shootReady) ?? false;

    public float GetMaxWeaponRange(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
        _getMaxWeaponRange?.Invoke(weapon, weaponId) ?? 0f;

    public bool GetTurretTargetTypes(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, IList<string> collection, int weaponId = 0) =>
        _getTurretTargetTypes?.Invoke(weapon, collection, weaponId) ?? false;

    public void SetTurretTargetTypes(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, IList<string> collection, int weaponId = 0) =>
        _setTurretTargetTypes?.Invoke(weapon, collection, weaponId);

    public void SetBlockTrackingRange(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, float range) =>
        _setBlockTrackingRange?.Invoke(weapon, range);

    public bool IsTargetAligned(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetEnt, int weaponId) =>
        _isTargetAligned?.Invoke(weapon, targetEnt, weaponId) ?? false;

    public MyTuple<bool, Vector3D?> IsTargetAlignedExtended(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetEnt, int weaponId) =>
        _isTargetAlignedExtended?.Invoke(weapon, targetEnt, weaponId) ?? new MyTuple<bool, Vector3D?>();

    public bool CanShootTarget(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetEnt, int weaponId) =>
        _canShootTarget?.Invoke(weapon, targetEnt, weaponId) ?? false;

    public Vector3D? GetPredictedTargetPosition(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetEnt, int weaponId) =>
        _getPredictedTargetPos?.Invoke(weapon, targetEnt, weaponId) ?? null;

    public float GetHeatLevel(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon) => _getHeatLevel?.Invoke(weapon) ?? 0f;
    public float GetCurrentPower(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon) => _currentPowerConsumption?.Invoke(weapon) ?? 0f;
    public float GetMaxPower(MyDefinitionId weaponDef) => _getMaxPower?.Invoke(weaponDef) ?? 0f;
    public bool HasGridAi(long entity) => _hasGridAi?.Invoke(entity) ?? false;
    public bool HasCoreWeapon(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon) => _hasCoreWeapon?.Invoke(weapon) ?? false;
    public float GetOptimalDps(long entity) => _getOptimalDps?.Invoke(entity) ?? 0f;

    public string GetActiveAmmo(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
        _getActiveAmmo?.Invoke(weapon, weaponId) ?? null;

    public void SetActiveAmmo(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId, string ammoType) =>
        _setActiveAmmo?.Invoke(weapon, weaponId, ammoType);

    public void MonitorProjectileCallback(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId, Action<long, int, ulong, long, Vector3D, bool> action) =>
        _monitorProjectile?.Invoke(weapon, weaponId, action);

    public void UnMonitorProjectileCallback(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId, Action<long, int, ulong, long, Vector3D, bool> action) =>
        _unMonitorProjectile?.Invoke(weapon, weaponId, action);

    public MyTuple<Vector3D, Vector3D, float, float, long, string> GetProjectileState(ulong projectileId) =>
        _getProjectileState?.Invoke(projectileId) ?? new MyTuple<Vector3D, Vector3D, float, float, long, string>();

    public float GetConstructEffectiveDps(long entity) => _getConstructEffectiveDps?.Invoke(entity) ?? 0f;

    public long GetPlayerController(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon) => _getPlayerController?.Invoke(weapon) ?? -1;

    public Matrix GetWeaponAzimuthMatrix(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
        _getWeaponAzimuthMatrix?.Invoke(weapon, weaponId) ?? Matrix.Zero;

    public Matrix GetWeaponElevationMatrix(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
        _getWeaponElevationMatrix?.Invoke(weapon, weaponId) ?? Matrix.Zero;

    public bool IsTargetValid(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetId, bool onlyThreats, bool checkRelations) =>
        _isTargetValid?.Invoke(weapon, targetId, onlyThreats, checkRelations) ?? false;

    public MyTuple<Vector3D, Vector3D> GetWeaponScope(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
        _getWeaponScope?.Invoke(weapon, weaponId) ?? new MyTuple<Vector3D, Vector3D>();
    // terminalBlock, Threat, Other, Something
    public MyTuple<bool, bool> IsInRange(Sandbox.ModAPI.Ingame.IMyTerminalBlock block) =>
        _isInRange?.Invoke(block) ?? new MyTuple<bool, bool>();