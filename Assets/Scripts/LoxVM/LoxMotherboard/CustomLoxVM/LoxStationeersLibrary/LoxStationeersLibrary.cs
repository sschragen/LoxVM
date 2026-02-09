using ULox;

namespace LoxVMod
{
    public class LoxStationeersLibrary : IULoxLibrary
    {
        private ConnectedDevices connectedDevices;
        private ConsoleData console;
        public LoxStationeersLibrary (ConnectedDevices _connectedDevices, ConsoleData _console)
        {
            connectedDevices = _connectedDevices;
            console = _console;
        }

        public Table GetBindings()  
        {
            return ULoxLibraryExt.GenerateBindingTable(this,                
                ("Motherboard", Value.New(new LoxMotherboardClass(connectedDevices,console))),
                ("DeviceList", Value.New(new LoxDeviceListClass(connectedDevices, console))),
                ("Device", Value.New(new LoxDeviceClass(connectedDevices, console))),
                ("Console", Value.New(new LoxConsoleClass(console)))
            );
        }
    }
}
