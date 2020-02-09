using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace LilyWhite.Lib.Util
{
    public class ObjAccessor
    {
        public object Object { get; private set; }
        public System.Type Type { get; private set; }
        private ObjAccessor() { }
        public ObjAccessor(object obj)
        {
            this.Object = obj;
            this.Type = obj.GetType();

        }
        public T GetProperty<T>(string property)
        {
            try
            {
                if (this.Object is ExpandoObject)
                {
                    return (T)((ExpandoObject)this.Object).Single(e => e.Key == property).Value;
                }
                return (T)this.Type.GetProperty(property).GetValue(this.Object);
            }
            catch (InvalidOperationException)
            {
                return default(T);
            }
        }
        public void SetProperty<T>(string property, T value)
        {
            this.Type.GetProperty(property).SetValue(this.Object, value);
        }
    }
}
