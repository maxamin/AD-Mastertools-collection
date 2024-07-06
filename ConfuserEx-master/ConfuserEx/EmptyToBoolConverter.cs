using System;
using System.Globalization;
using System.Windows.Data;

namespace ConfuserEx {
	[ValueConversion(typeof(string), typeof(bool), ParameterType = typeof(bool))]
	[ValueConversion(typeof(string), typeof(bool), ParameterType = typeof(string))]
	public class EmptyToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool stateIfEmpty = true;
			switch (parameter) {
				case bool boolParameter:
					stateIfEmpty = boolParameter;
					break;
				case string strParameter when bool.TryParse(strParameter, out var parsedStrParameter):
					stateIfEmpty = parsedStrParameter;
					break;
			}

			if (value == null) return stateIfEmpty;
			if (!(value is string strValue)) return stateIfEmpty;

			return string.IsNullOrEmpty(strValue) ? stateIfEmpty : !stateIfEmpty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
			throw new NotSupportedException();
	}
}
