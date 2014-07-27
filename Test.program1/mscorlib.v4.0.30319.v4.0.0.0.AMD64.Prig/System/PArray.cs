
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PArray : PArrayBase 
    {
        public static zzCreateInstance CreateInstance() 
        {
            return new zzCreateInstance();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzCreateInstance 
        {
            public IndirectionFunc<System.Type, System.Int32[], System.Int32[], System.Array> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfCreateInstance_Type_intArray_intArray;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Type, System.Int32[], System.Int32[], System.Array>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzExists<T> Exists<T>() 
        {
            return new zzExists<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzExists<T> 
        {
            public IndirectionFunc<T[], System.Predicate<T>, System.Boolean> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfExists_T_TArray_Predicate_T;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<T[], System.Predicate<T>, System.Boolean>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzBinarySearch BinarySearch() 
        {
            return new zzBinarySearch();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzBinarySearch 
        {
            public IndirectionFunc<System.Array, System.Int32, System.Int32, System.Object, System.Collections.IComparer, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfBinarySearch_Array_int_int_object_IComparer;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Array, System.Int32, System.Int32, System.Object, System.Collections.IComparer, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
