using System.ComponentModel;
using System.Data;
using System.Numerics;
using System.Runtime.InteropServices;
using CS16Cheat.LWOperations;
using CS16Cheat.Overlay;
using CS16Cheat.utils;

namespace CS16Cheat.core;

public record EntityListEntry(int Offset, Modules Module);

internal static class GameData
{
    internal static Entity[] Entities { get; private set; }
    private static EntityListEntry EntityListData { get; set; } = null!;
    internal static Entity LocalPlayer;
    private const int _maxPlayers = 64;
    private static nint _entityListAddress;

    static GameData()
    {
        LocalPlayer = new Entity();
        Entities = new Entity[_maxPlayers];

        IntializeUnchangedData();
    }

    private static void IntializeUnchangedData()
    {
        EntityListData = Offsets.EntityList.TryGetValue(Info.CurrentVersion, out var value)
            ? value
            : throw new VersionNotFoundException();

        _entityListAddress =
            Memory.ReadPointer(
                ModuleManager.GetBaseAddress(EntityListData.Module) + EntityListData.Offset
            )
            ?? throw new Win32Exception(
                $"Couldn't read entity list pointer. Make sure you start the game | ERROR_CODE: {Marshal.GetLastWin32Error()}"
            );
    }

    internal static void UpdateGameData()
    {
        if (!FillEntityFields(ref LocalPlayer, Offsets.localPlayer, true))
        {
            return;
        }

        for (int i = 0; i < _maxPlayers; i++)
        {
            var entityAddrOffset = Offsets.step * (i + 1) + Offsets.localPlayer;
            if (!FillEntityFields(ref Entities[i], entityAddrOffset, false))
            {
                break;
            }
        }
    }

    private static bool FillEntityFields(
        ref Entity entity,
        int offsetToInitialAddress,
        bool isLocalPlayer
    )
    {
        var baseAddr = Memory.ReadPointer(_entityListAddress + offsetToInitialAddress);
        if (baseAddr == null)
        {
            if (isLocalPlayer)
            {
                Renderer.SetError("Couldn't read local player base address.");
                return false;
            }
            Renderer.SetWarning("Couldn't read entity base address.");
            return false;
        }
        entity.Address = baseAddr.Value;

        var objectDataAddress = Memory.ReadPointer(baseAddr.Value + Offsets.objectData);
        if (objectDataAddress == null)
        {
            return false;
        }

        var team = Memory.ReadInt32(baseAddr.Value + Offsets.team);
        var health = Memory.ReadFloat(objectDataAddress.Value + Offsets.health);
        var position = Memory.ReadVec3(objectDataAddress.Value + Offsets.position);

        if (team == null)
        {
            Renderer.SetWarning("Couldn't read team address");
            return false;
        }
        if (health == null)
        {
            Renderer.SetWarning("Couldn't read health address");
            return false;
        }
        if (position == null)
        {
            Renderer.SetWarning("Couldn't read position address");
            return false;
        }

        if (!isLocalPlayer)
            entity.Distance = Vector3.Distance(LocalPlayer.Position, position.Value);

        entity.Health = health.Value;
        entity.Team = team.Value;
        entity.Position = position.Value;

        return true;
    }
}
