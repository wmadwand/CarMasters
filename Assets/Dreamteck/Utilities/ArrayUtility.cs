namespace Dreamteck
{
    using System;

    public static class ArrayUtility
    {
        public static void Add<T>(ref T[] array, T item)
        {
            T[] newArray = new T[array.Length + 1];
            array.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = item;
            array = newArray;
        }
        public static bool Contains<T>(T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item)) return true;
            }
            return false;
        }
        public static int IndexOf<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(value)) return i;
            }
            return 0;
        }
        public static void Insert<T>(ref T[] array, int index, T item)
        {
            T[] newArray = new T[array.Length + 1];
            for (int i = 0; i < newArray.Length; i++)
            {
                if (i < index) newArray[i] = array[i];
                else if (i > index) newArray[i] = array[i - 1];
                else newArray[i] = item;
            }
            array = newArray;
        }


        public static void RemoveAt<T>(ref T[] array, int index)
        {
            if (array.Length == 0) return;
            T[] newArray = new T[array.Length - 1];
            for (int i = 0; i < array.Length; i++)
            {
                if (i < index) newArray[i] = array[i];
                else if (i > index) newArray[i-1] = array[i];
            }
            array = newArray;
        }

        public static void ForEach<T>(this T[] source, Action<T> onLoop)
        {
            foreach (var item in source)
            {
                onLoop(item);
            }
        }

        public static void SetLength<T>(ref T[] source, int newCount)
        {
            T[] newArray = new T[newCount];
            for (int i = 0; i < UnityEngine.Mathf.Min(newCount, source.Length); i++)
            {
                newArray[i] = source[i];
            }
            source = newArray;
        }
    }
}
