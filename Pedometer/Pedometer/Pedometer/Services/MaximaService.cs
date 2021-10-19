using System;
using System.Collections.Generic;
using System.Text;

namespace Pedometer.Services
{
    public class MaximaService
    {
        public static List<double> Maxima(double[] dataset)
        {
			List<double> result = new List<double>();
			var lastval = dataset[0];
			var lastindex = 0;
			bool dirpos = true;
			for (int i = 0; i < dataset.Length; i++)
			{
				var currentval = dataset[i];
				if (dirpos && currentval < lastval)
				{
					Console.WriteLine(string.Format("i: {0} value: {1}", lastindex, dataset[lastindex]));
					result.Add(dataset[lastindex]);
					dirpos = false;
				}

				if (currentval >= lastval)
					dirpos = true;

				lastval = currentval;
				lastindex = i;
			}

			return result;
		}
    }
}
