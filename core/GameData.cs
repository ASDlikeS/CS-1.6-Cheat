using System.ComponentModel;
using System.Data;
using System.Numerics;
using System.Runtime.InteropServices;
using CS16Cheat.Graphic;
using CS16Cheat.LWOperations;
using CS16Cheat.utils;

namespace CS16Cheat.core;

public record EntityListEntry(int Offset, Modules Module);

internal static class GameData
{
    internal static Entity LocalPlayer { get; private set; }
    internal static Entity[] Entities { get; private set; }
    private const int MaxPlayers = 64;
    private static nint EntityListAddress;
    private static EntityListEntry _entityListData = null!;

    static GameData()
    {
        LocalPlayer = new Entity();
        Entities = new Entity[MaxPlayers];
        for (int i = 0; i < MaxPlayers; i++)
            Entities[i] = new Entity();

        IntializeUnchangedData();
    }

    private static void IntializeUnchangedData()
    {
        _entityListData = Offsets.EntityList.TryGetValue(Info.CurrentVersion, out var value)
            ? value
            : throw new VersionNotFoundException();

        var entityListAddress =
            Memory.ReadPointer(
                ModuleManager.GetBaseAddress(_entityListData.Module) + _entityListData.Offset
            )
            ?? throw new Win32Exception(
                $"Couldn't read entity list pointer. Make sure you start the game | ERROR_CODE: {Marshal.GetLastWin32Error()}"
            );

        EntityListAddress = entityListAddress;
    }

    internal static void UpdateGameData()
    {
        Renderer.HasError = false;
        Renderer._hasWarn = false;

        if (!FillEntityFields(LocalPlayer, Offsets.localPlayer, true))
        {
            return;
        }

        for (int i = 0; i < MaxPlayers; i++)
        {
            var entityAddrOffset = Offsets.step * (i + 1) + Offsets.localPlayer;
            if (!FillEntityFields(Entities[i], entityAddrOffset, false))
                break;
        }
    }

    private static bool FillEntityFields(
        Entity entity,
        int offsetToInitialAddress,
        bool isLocalPlayer
    )
    {
        var baseAddr = Memory.ReadPointer(EntityListAddress + offsetToInitialAddress);
        if (baseAddr == null)
        {
            if (isLocalPlayer)
            {
                SetError("Couldn't read local player base address.");
                return false;
            }
            SetWarning("Couldn't read entity base address.");
            return false;
        }
        entity.Address = baseAddr.Value;

        var objectDataAddress = Memory.ReadPointer(baseAddr.Value + Offsets.objectData);
        if (objectDataAddress == null)
        {
            SetError("Couldn't read entity data pointer address.");
            return false;
        }

        var team = Memory.ReadInt32(baseAddr.Value + Offsets.team);
        var health = Memory.ReadFloat(objectDataAddress.Value + Offsets.health);
        var position = Memory.ReadVec3(objectDataAddress.Value + Offsets.position);

        if (team == null)
        {
            SetWarning("Couldn't read team address");
            return false;
        }
        if (health == null)
        {
            SetWarning("Couldn't read health address");
            return false;
        }
        if (position == null)
        {
            SetWarning("Couldn't read position address");
            return false;
        }

        if (!isLocalPlayer)
            entity.Distance = Vector3.Distance(LocalPlayer.Position, position.Value);

        entity.Health = health.Value;
        entity.Team = team.Value;
        entity.Position = position.Value;
        return true;
    }

    private static void SetError(string errMessage)
    {
        Renderer.HasError = true;
        Renderer.LastErrorMessage = $"ERROR: {errMessage} | Win32: {Marshal.GetLastWin32Error()}";
    }

    private static void SetWarning(string warnMessage)
    {
        Renderer._hasWarn = true;
        Renderer.LastWarnMessage = $"WARNING: {warnMessage}.";
    }
}
