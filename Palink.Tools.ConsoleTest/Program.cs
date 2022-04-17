using Palink.Tools.Extensions.PLArray;

var array1= new[,]
{
    {1,2,3},
    {4,5,6},
    {7,8,9}
};

var array2= new[,,]
{
    {
        {1,2,3},
        {4,5,6},
        {7,8,9}
    },
    {
        {10,11,12},
        {13,14,15},
        {16,17,18}
    },
    {
        {19,20,21},
        {22,23,24},
        {25,26,27}
    }
};

array1.ForEach((arr, indices) =>
{
    var value = arr.GetValue(indices[0], indices[1]);
    Console.WriteLine($"[{indices[0]},{indices[1]}] = {value}");
});

array2.ForEach((arr, indices) =>
{
    var value = arr.GetValue(indices[0], indices[1], indices[2]);
    Console.WriteLine($"[{indices[0]},{indices[1]},{indices[2]}] = {value}");
});