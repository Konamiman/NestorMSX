using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Konamiman.NestorMSX.Z80Debugger.Console
{
    public class PropertyVariable : Variable
    {
        private readonly PropertyInfo propertyInfo;
        private readonly object hostingObject;
        private readonly TypeConverterWrapper converter;
        
        public PropertyVariable(string fullName, PropertyInfo propertyInfo, object hostingObject) : base(fullName)
        {
            this.propertyInfo = propertyInfo;
            this.hostingObject = hostingObject;

            CanWrite = propertyInfo.GetSetMethod()?.IsPublic == true;
            CanRead = propertyInfo.GetGetMethod()?.IsPublic == true;

            converter = new TypeConverterWrapper(TypeDescriptor.GetConverter(propertyInfo.GetType()), propertyInfo.PropertyType);
        }

        public override object GetValueImpl()
        {
            try {
                return propertyInfo.GetValue(hostingObject, null);
            }
            catch (TargetInvocationException ex) {
                throw ex.InnerException;
            }
        }

        public override void SetValueImpl(object value)
        {
            value = this.converter.ConvertFrom(value);

            try {
                propertyInfo.SetValue(hostingObject, value, null);
            }
            catch (TargetInvocationException ex) {
                throw ex.InnerException;
            }
        }
    }
}
