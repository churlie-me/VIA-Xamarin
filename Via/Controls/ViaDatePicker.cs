using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Via.Controls
{
    public class ViaDatePicker : DatePicker
    {
        public static readonly BindableProperty EnterTextProperty = BindableProperty.Create(propertyName: "Placeholder", 
                                                                                            returnType: typeof(string), 
                                                                                            declaringType: typeof(ViaDatePicker), 
                                                                                            defaultValue: default(string));
        public string Placeholder { get; set; }
    }
}
