using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicRecognition
{
    static class Parser
    {
        /// <summary>
        /// Parses double[] array to string - python array type
        /// </summary>
        public static string ToString(double[] array)
        {
            if (array == null)
            {
                MessageBox.Show("Mediana zapisuje się do bazy jako null");
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i=0; i<array.Length; i++)
            {
                string temp = array[i].ToString().Replace(',', '.');
                sb.Append(temp);

                if (i != array.Length-1)
                    sb.Append(" ");
            }
            sb.Append("]");
            return sb.ToString();
        }
        /// <summary>
        /// Parses double[,] array to string - python array type
        /// </summary>
        public static string ToString(double[,] array)
        {
            if (array == null)
            {
                MessageBox.Show("Macierz kowariancji zapisuje się do bazy jako null");
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < array.GetLength(0); i++)
            {
                sb.Append("[");
                for (int j=0; j<array.GetLength(1); j++)
                {
                    string temp = array[i, j].ToString().Replace(',', '.');
                    sb.Append(temp);

                    if (j != array.GetLength(1) - 1)
                        sb.Append(" ");
                }
                sb.Append("]");
                if (i != array.GetLength(0) - 1)
                    sb.Append(" ");
            }
            sb.Append("]");
            return sb.ToString();
        }
        /// <summary>
        /// Parses python vector to double[] array
        /// </summary>
        public static double[] ToDoubleArray(string array)
        {
            List<double> arr = new List<double>();

            array = array.Replace('.', ',');
            string[] arrays = array.Split('[', ' ', ']');
            for (int i = 0; i < arrays.Length; i++)
                if (!string.IsNullOrWhiteSpace(arrays[i]))
                    arr.Add(double.Parse(arrays[i]));

            return arr.ToArray();
        }

        /// <summary>
        /// Parses python vector to double[,] array
        /// </summary>
        public static double[,] To2dDoubleArray(string array)
        {
            array = array.Replace('.', ',');
            var arr = new List<List<double>>();
            int start = 0;
            int len = 0;

            for (int i=0; i<array.Length; i++)
            {
                if (array[i] != '[')
                    continue;

                start = ++i;
                len = 0;
                while (array[i] != ']')
                {
                    if (array[i] == '[')
                        break;

                    i++;
                    len++;
                }
                if (array[i] == '[')
                {
                    i--;
                    continue;
                }

                string vector = array.Substring(start, len);
                var tempList = new List<double>();
                string[] nums = vector.Split(' ');
                for (int j = 0; j < nums.Length; j++)
                    if (!string.IsNullOrWhiteSpace(nums[j]))
                        tempList.Add(double.Parse(nums[j]));
                arr.Add(tempList);
            }

            // prepare answer
            int dim1 = arr.Count;
            int dim2 = arr[0].Count;
            double[,] ans = new double[dim1, dim2];
            for (int i = 0; i < dim1; i++)
                for (int j = 0; j < dim2; j++)
                    ans[i, j] = arr[i][j];
            return ans;
        }
    }
}
