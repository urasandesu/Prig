
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PArray : PArrayBase
    {
        public static class CreateInstance
        {
            public static IndirectionFunc<System.Type, System.Int32[], System.Int32[], System.Array> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfCreateInstance_Type_intArray_intArray;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Type, System.Int32[], System.Int32[], System.Array>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class Exists<T>
        {
            public static IndirectionFunc<T[], System.Predicate<T>, System.Boolean> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfExists_T_TArray_Predicate_T;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<T[], System.Predicate<T>, System.Boolean>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class BinarySearch
        {
            public static IndirectionFunc<System.Array, System.Int32, System.Int32, System.Object, System.Collections.IComparer, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfBinarySearch_Array_int_int_object_IComparer;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Array, System.Int32, System.Int32, System.Object, System.Collections.IComparer, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
