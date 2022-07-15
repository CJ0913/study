//最大堆
public class AddHeapSort
{
    //上浮操作
    public static void UpAdjust(int[] array)
    {
        //从最后一个节点开始
        int childIndex = array.Length - 1;
        int parentIndex = (childIndex - 1) / 2;
        int temp = array[childIndex];
        while (childIndex > 0 && temp > array[parentIndex])
        {
            array[childIndex] = array[parentIndex];
            childIndex = parentIndex;
            parentIndex = (childIndex - 1) / 2;
        }
        array[childIndex] = temp;
    }
    /// <summary>
    /// 下沉操作
    /// </summary>
    /// <param name="array">要进行下沉操作的二叉堆</param>
    /// <param name="index">要下沉的索引</param>
    public static void DownAdjust(int[] array, int index)
    {
        //没有子节点不能进行下沉操作
        if (index > (array.Length - 2) / 2)
        {
            return;
        }
        int parentIndex = index;
        int childIndex = index * 2 + 1;//左子节点的下标
        int temp = array[index];
        while (childIndex < array.Length)
        {
            //有右子节点并且右子节点比左子节点大
            if (childIndex + 1 < array.Length && array[childIndex + 1] > array[childIndex])
            {
                childIndex += 1;
            }
            if (temp > array[childIndex])
            {
                break;
            }
            array[parentIndex] = array[childIndex];
            parentIndex = childIndex;
            childIndex = parentIndex * 2 + 1;
        }
        array[parentIndex] = temp;
    }

    //构建二叉堆
    public static void BuildBinaryHeap(int[] array)
    {
        //从最后一个非叶子节点开始到个根节点
        for (int index = (array.Length / 2) - 1; index >= 0; index--)
        {
            //做下沉操作
            DownAdjust(array, index);
        }
    }

    public static int[] DeleteElement(int[] array)
    {
        //删除根节点
        int[] newArray = new int[array.Length - 1];
        for (int i = 1; i < array.Length - 1; i++)
        {
            newArray[i] = array[i];
        }
        //把最后一个节点放到根节点位置
        newArray[0] = array[array.Length - 1];
        //对根节点做下沉操作
        DownAdjust(newArray, 0);
        return newArray;
    }

    public static int[] AddElement(int[] array, int value)
    {
        int[] newArray = new int[array.Length + 1];
        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }
        newArray[array.Length] = value;
        UpAdjust(newArray);
        return newArray;
    }

    public static void Main(string[] args)
    {
        // 构建二叉堆
        int[] arr01 = { 5, 3, 11, 9, 8, 6, 7, 2, 4, 6, 15 };
        BuildBinaryHeap(arr01);
        for (int i = 0; i < arr01.Length; i++)
        {
            Console.Write(arr01[i] + "  ");
        }
        Console.WriteLine();

        arr01 = AddElement(arr01, 13);
        for (int i = 0; i < arr01.Length; i++)
        {
            Console.Write(arr01[i] + "  ");
        }
        Console.WriteLine();

        arr01 = DeleteElement(arr01);
        for (int i = 0; i < arr01.Length; i++)
        {
            Console.Write(arr01[i] + "  ");
        }
        Console.WriteLine();
    }
}