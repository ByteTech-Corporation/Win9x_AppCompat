using System;

// Define missing classes or structs
public abstract class DebuggerBase {}
public abstract class LoggerBase {}
public abstract class LibraryBase {}
public enum ComponentType { RuntimeDebugger, RuntimeLogger, AppDebugger, AppLogger, AppLibrary, RuntimeLibrary }
public delegate bool GetRegisteredDelegate(out DebuggerBase dbg);
public delegate bool GetRegisteredDelegate2<T>(out T component) where T : LibraryBase;

public struct RegisteredComponent
{
    public ComponentType Type;
    public IntPtr Address;
};

// Your original class
public class CompWCM
{
    private readonly DLLClass _dll;

    public CompWCM(DLLClass dll) => _dll = dll;

    // Use properties for better encapsulation
    public uint CompiledAddress
    {
        get => _dll.CompiledAddress;
        set => _dll.CompiledAddress = value;
    }

    public void comp_WCM_pass()
    {
        var dwordValue = ReadDWordAt(_dll.CompiledAddress);

        switch (dwordValue & 0x00004)
        {
            case 0x00004:
                CallGetRegisteredComponents((uint)_dll.Debug, out var debugger);
                Console.WriteLine($"Debugger: {debugger}");
                break;
            default:
                throw new ArgumentException("Unexpected dwordValue.");
        }
    }

    private unsafe uint ReadDWordAt(uint addr)
    {
        byte* ptr = (byte*)Unsafe.AsPointer(ref addr);
        return (uint)(ptr[0] | ptr[1] << 8 | ptr[2] << 16 | ptr[3] << 24);
    }

    private unsafe void CallGetRegisteredComponents(uint id, out DebuggerBase component)
    {
        var methods = typeof(CompWCM)
                      .GetMethods(System.Reflection.BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(out DebuggerBase))
            {
                var del = (GetRegisteredDelegate)method.CreateDelegate(typeof(GetRegisteredDelegate));
                if (del(id, out component))
                    return;
            }
        }

        throw new InvalidOperationException("Couldn't find matching method.");
    }
}
