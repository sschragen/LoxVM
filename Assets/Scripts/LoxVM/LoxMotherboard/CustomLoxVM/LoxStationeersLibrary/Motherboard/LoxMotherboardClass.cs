using Assets.Scripts.Objects;
using System.Net.Sockets;
using System.Xml.Linq;
using ULox;
using UnityEngine;
using static RootMotion.FinalIK.VRIK;

namespace LoxVMod
{
    public class LoxMotherboardClass : UserTypeInternal
    {
        public static readonly Value SharedLoxAllDevicesClassValue = Value.New(new LoxMotherboardClass(connectedDevices, console));
        public override InstanceInternal MakeInstance() => CreateInstance();
        public static LoxMotherboardInstance CreateInstance() => new(SharedLoxAllDevicesClassValue.val.asClass);

        private static ConnectedDevices connectedDevices;
        private static ConsoleData console;

        public LoxMotherboardClass(ConnectedDevices _connectedDevices, ConsoleData _console) : base(new HashedString("Motherboard"), UserType.Native)
        {
            connectedDevices = _connectedDevices;
            console = _console;

            this.AddMethodsToClass(
                (nameof(Count), Value.New(Count, 1, 0)),
                (nameof(GetDevice), Value.New(GetDevice, 1, 1)),
                (nameof(Print), Value.New(Print, 0, 0)),
                (nameof(GetDevicesByName), Value.New(GetDevicesByName, 1, 1)),
                (nameof(GetDevicesByType), Value.New(GetDevicesByType, 1, 1)),
                (nameof(GetDevicesByTypeName), Value.New(GetDevicesByTypeName, 1, 2)),
                (nameof(GetAllDevices), Value.New(GetAllDevices, 1, 0)),
                (nameof(Hash),Value.New(Hash,1,1)),
                (nameof(SetInstructionsPerFrame),Value.New(SetInstructionsPerFrame,0,1))
            );
        }
        private NativeCallResult Print (Vm vm)
        {
            foreach (var item in connectedDevices.deviceList)
            {
                console.Log($"RefID : {item.ReferenceId} Name : {item.DisplayName}");
            }
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult Count(Vm vm)
        {
            //var instance = vm.GetArg(0);            
            //return Value.New(connectedDevices.deviceDict.Count);
            vm.SetNativeReturn(0, Value.New(connectedDevices.deviceDict.Count));
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult GetDevice(Vm vm)
        {
            //var instance = vm.GetArg(0);
            if (vm.GetArg(1).type == ValueType.Double)
            {
                var index = vm.GetArg(1);            
                var referenceId = connectedDevices.deviceList[(int)index.val.asDouble].ReferenceId;

                var returnClass = LoxDeviceClass.CreateInstance();
                returnClass.Set(Value.New(referenceId));

                vm.SetNativeReturn(0, Value.New(returnClass));
                return NativeCallResult.SuccessfulExpression;
            }
            else
            {
                vm.ThrowRuntimeException("Type of Argument must be <number>.");
                return NativeCallResult.Failure;
            }
        }
        private NativeCallResult GetDevicesByName(Vm vm)
        {
            //var instance = vm.GetArg(0);
            if (vm.GetArg(1).type == ValueType.String)
            {
                var name = vm.GetArg(1).val.asString.String;

                var returnClass = LoxDeviceListClass.CreateInstance();
                foreach (var device in connectedDevices.GetDevicesByName(name))
                {
                    var deviceInstance = LoxDeviceClass.CreateInstance();
                    deviceInstance.Set(Value.New(device));
                    returnClass.List.Add(deviceInstance);
                }

                vm.SetNativeReturn(0, Value.New(returnClass));
                return NativeCallResult.SuccessfulExpression;
            }
            else
            {
                vm.ThrowRuntimeException("Type of Argument must be <string>.");
                return NativeCallResult.Failure;
            }
        }
        private NativeCallResult GetDevicesByType(Vm vm)
        {
            //var instance = vm.GetArg(0);
            if (vm.GetArg(1).type == ValueType.String)
            {
                var type = vm.GetArg(1).val.asString.String;

                var returnClass = LoxDeviceListClass.CreateInstance();
                foreach (var device in connectedDevices.GetDevicesByType(type))
                {
                    var deviceInstance = LoxDeviceClass.CreateInstance();
                    deviceInstance.Set(Value.New(device));
                    returnClass.List.Add(deviceInstance);
                }
            
                vm.SetNativeReturn(0, Value.New(returnClass));
                return NativeCallResult.SuccessfulExpression;
            }
            else
            {
                vm.ThrowRuntimeException("Type of Argument must be <string>.");
                return NativeCallResult.Failure;
            }
        }
        private NativeCallResult GetDevicesByTypeName(Vm vm)
        {
            //var instance = vm.GetArg(0);
            if ((vm.GetArg(1).type == ValueType.String) &&  (vm.GetArg(2).type == ValueType.String))
            {
                var type = vm.GetArg(1).val.asString.String;
                var name = vm.GetArg(2).val.asString.String;

                var returnClass = LoxDeviceListClass.CreateInstance();
                foreach (var device in connectedDevices.GetDevivesByTypeName(type, name))
                {
                    var deviceInstance = LoxDeviceClass.CreateInstance();
                    deviceInstance.Set(Value.New(device));
                    returnClass.List.Add(deviceInstance);
                }
                vm.SetNativeReturn(0, Value.New(returnClass));
                return NativeCallResult.SuccessfulExpression;
            }
            else
            {
                vm.ThrowRuntimeException("Type of Argument must be <string>,<string>.");
                return NativeCallResult.Failure;
            }
        }
        private NativeCallResult GetAllDevices(Vm vm)
        {
            //var instance = vm.GetArg(0);

            var returnClass = LoxDeviceListClass.CreateInstance();
            foreach (var device in connectedDevices.GetAllDevices())
            {
                var deviceInstance = LoxDeviceClass.CreateInstance();
                deviceInstance.Set(Value.New(device));
                returnClass.List.Add(deviceInstance);
            }
            vm.SetNativeReturn(0, Value.New(returnClass));
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult Hash(Vm vm)
        { 
            var s = vm.GetArg(1);
            if (s.type == ValueType.String)
            {
                long hash = Animator.StringToHash(s.val.asString.String);
                vm.SetNativeReturn(0, Value.New(hash));
                return NativeCallResult.SuccessfulExpression; 
            }
            else
            {
                vm.ThrowRuntimeException("Type of Argument must be <string>.");
                return NativeCallResult.Failure;
            }
        }
        private NativeCallResult SetInstructionsPerFrame(Vm vm)
        {
            var s = vm.GetArg(1);
            if (s.type == ValueType.Double)
            {
                vm.platform.SetTimeSlice((float)s.val.asDouble);
                return NativeCallResult.SuccessfulExpression;
            }
            else
            {
                vm.ThrowRuntimeException("Type of Argument must be <number>.");
                return NativeCallResult.Failure;
            }
        }


    }
}
