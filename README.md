# DataStructures

Implementation of advanced data structures. Project contains K-d tree, AVL tree, HashSet and ExtendibleHashing structures. It has been created as a part of course [Algorithms and Data structures 2 (5II215)](https://vzdelavanie.uniza.sk/vzdelavanie/planinfo.php?kod=275016&lng=en) taught at Faculty of Management Science and Informatics, University of Žilina. 

## Structures

Class library project containing implementation of aforementioned data structures. Individual structures can be obtained from this library through StructureFactory class loctated in the [Structures](./Structures) namespace.

    using Structures;
    
    class StructureExample
    {
        static void Main(string[] args)
        {
            // Obtaining AVL tree from StructureFactory.
            StructureFactory.Instance.GetAvlTree<int>();
        }
    }
    
### [AvlTree](./Structures/Tree/AvlTree.cs)
Implementation of [AVL tree](https://en.wikipedia.org/wiki/AVL_tree). Objects stored at this structure must implement [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable?view=net-5.0) interface. Structure does not support duplicate keys and is ordered by the keys in ascending order. Time complexity of **Search, Insert and Delete** operations are **O(log<sub>2</sub>n)**. Finding minimum and maximum through Min and Max properties has the same time complexity as well. **Range search** time complexity is **O(log<sub>2</sub>n + m)** where m is number of found elements.

### [KdTree](./Structures/Tree/KdTree.cs)

Implementation of [K-d tree](https://en.wikipedia.org/wiki/K-d_tree). This structure contains objects with multiple secondary (non-unique) keys. Objects stored at this structure must implement [IKdComparable](./Structures/Interface/IKdComparable.cs) interface. Structure does not necessarily stores objects in ascending order due to multidimensionality. Time complexity of **Search, Insert and Delete** operations are **O(log<sub>2</sub>n)**. **Range search** time complexity is **[O(k * n<sup>1 - 1/k</sup>)](https://link.springer.com/article/10.1007/BF00263763)** where k is number of K-d tree dimensions.

### [HashSet](./Structures/Hashing/HashSet.cs)

Implementation of [Hash table](https://en.wikipedia.org/wiki/Hash_table). Objects stored at the structure should override Equals and GetHashCode method if necessary. **Search, Insert and Delete** time complexities are **O(1)**.

### [ExtendibleHashing](./Structures/Hashing/ExtendibleHashing.cs)

Implementation of [Extendible hashing](https://en.wikipedia.org/wiki/Extendible_hashing). This structures is located at the disk instead of computer memory. Objects stored in this structure must implement [ISerializable](./Structures/Interface/ISerializable.cs) interface. Purpose of this structure is to minimize number of file accesses during Search, Insert and Delete operations. File accesses during all these operations are constant.
    
## StructureTests

XUnit test project used to test functionality of each of the structures.

## SurveyApp

WPF application used to demonstrate structures' functionality. It is a sample project consuming Structures class library.
