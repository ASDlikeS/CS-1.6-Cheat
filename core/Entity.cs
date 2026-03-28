using System.Numerics;
using CS16Cheat.core;

namespace CS16Cheat.core;

public class Entity()
{
    public nint Address { get; set; }
    public Vector3 Position { get; set; }
    public float Health { get; set; }
    public int Team { get; set; }
    public float Distance { get; set; }
}
