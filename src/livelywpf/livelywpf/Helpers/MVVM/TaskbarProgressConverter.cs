﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace livelywpf.Helpers.MVVM
{
    class TaskbarProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
               System.Globalization.CultureInfo culture)
        {
            double progressValue = 0f;
            if (targetType == typeof(double))
            {
                progressValue = ((double)value)/100f;
            }
            return progressValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
