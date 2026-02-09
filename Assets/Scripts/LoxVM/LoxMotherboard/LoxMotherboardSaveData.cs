using Assets.Scripts.Objects.Items;
using LoxVMod.Events;
using System.Xml.Serialization;

namespace LoxVMod
{
    [XmlInclude(typeof(LoxMotherboardSaveData))]
    public class LoxMotherboardSaveData : MotherboardSaveData
    {
        [XmlElement]
        public string filename;

        [XmlElement]
        public EventVMRunState.VMRunState runState;
    }
}
