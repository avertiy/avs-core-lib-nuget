namespace AVS.CoreLib.Extensions;

public static class ArrayExtensions
{
    public static bool AreSame(this decimal[] arr, decimal[] arrToCompare, decimal tolerance)
    {
        if (arr.Length != arrToCompare.Length)
            return false;

        for (var i = 0; i < arr.Length; i++)
        {
            if(!arr[i].IsEqual(arrToCompare[i], tolerance))
                return false;
        }

        return true;
    }
}