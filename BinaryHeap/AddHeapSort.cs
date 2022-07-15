//����
public class AddHeapSort
{
    //�ϸ�����
    public static void UpAdjust(int[] array)
    {
        //�����һ���ڵ㿪ʼ
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
    /// �³�����
    /// </summary>
    /// <param name="array">Ҫ�����³������Ķ����</param>
    /// <param name="index">Ҫ�³�������</param>
    public static void DownAdjust(int[] array, int index)
    {
        //û���ӽڵ㲻�ܽ����³�����
        if (index > (array.Length - 2) / 2)
        {
            return;
        }
        int parentIndex = index;
        int childIndex = index * 2 + 1;//���ӽڵ���±�
        int temp = array[index];
        while (childIndex < array.Length)
        {
            //�����ӽڵ㲢�����ӽڵ�����ӽڵ��
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

    //���������
    public static void BuildBinaryHeap(int[] array)
    {
        //�����һ����Ҷ�ӽڵ㿪ʼ�������ڵ�
        for (int index = (array.Length / 2) - 1; index >= 0; index--)
        {
            //���³�����
            DownAdjust(array, index);
        }
    }

    public static int[] DeleteElement(int[] array)
    {
        //ɾ�����ڵ�
        int[] newArray = new int[array.Length - 1];
        for (int i = 1; i < array.Length - 1; i++)
        {
            newArray[i] = array[i];
        }
        //�����һ���ڵ�ŵ����ڵ�λ��
        newArray[0] = array[array.Length - 1];
        //�Ը��ڵ����³�����
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
        // ���������
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