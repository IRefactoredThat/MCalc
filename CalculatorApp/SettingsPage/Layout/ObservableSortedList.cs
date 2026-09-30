using System.Collections;
using System.Collections.Specialized;

namespace CalculatorApp.SettingsPage.Layout;

public sealed class ObservableSortedList<T> :
    IReadOnlyList<T>,
    INotifyCollectionChanged where T : notnull
{
    private sealed class Node
    {
        public required T Item;
        public required int Key;

        public uint Priority;
        public int Size = 1;

        public Node? Left;
        public Node? Right;
    }

    private readonly Dictionary<T, int> _keys;
    private readonly List<T> _initialItems;

    private Node? _root;

    public ObservableSortedList(IEnumerable<T> collection)
    {
        var nextKey = 0;
        _keys = new Dictionary<T, int>();
        _initialItems = [];

        foreach (var item in collection)
        {
            if (_keys.ContainsKey(item))
            {
                continue;
            }

            var key = nextKey++;

            _keys.Add(item, key);
            _initialItems.Add(item);

            _root = Insert(
                _root,
                new Node
                {
                    Item = item,
                    Key = key,
                    Priority = RandomPriority()
                });
        }
    }

    public int Count => GetSize(_root);

    public T this[int index]
    {
        get => index >= Count ?
                throw new ArgumentOutOfRangeException(nameof(index)) :
                GetAt(_root!, index).Item;
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public bool Add(T item)
    {
        if (!_keys.TryGetValue(item, out var index))
        {
            return false;
        }

        if (ContainsKey(_root, index))
        {
            return false;
        }

        var visibleIndex = RankForInsertion(_root, index);

        _root = Insert(
            _root,
            new Node
            {
                Item = item,
                Key = index,
                Priority = RandomPriority()
            });

        CollectionChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                item,
                visibleIndex));

        return true;
    }

    public bool Remove(T item)
    {
        if (!_keys.TryGetValue(item, out var key))
        {
            return false;
        }

        if (!ContainsKey(_root, key))
        {
            return false;
        }

        var index = Rank(_root, key);

        _root = Remove(_root, key);

        CollectionChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                item,
                index));

        return true;
    }

    private static bool ContainsKey(Node? node, int key)
    {
        while (node is not null)
        {
            if (key == node.Key)
            {
                return true;
            }

            node = key < node.Key
                ? node.Left
                : node.Right;
        }

        return false;
    }

    private static int RankForInsertion(Node? node, int key)
    {
        var rank = 0;

        while (node is not null)
        {
            if (key < node.Key)
            {
                node = node.Left;
            }
            else
            {
                rank += GetSize(node.Left) + 1;
                node = node.Right;
            }
        }

        return rank;
    }

    public void RestoreToInitialState()
    {
        _root = null;

        foreach (var item in _initialItems)
        {
            var key = _keys[item];

            _root = Insert(
                _root,
                new Node
                {
                    Item = item,
                    Key = key,
                    Priority = RandomPriority()
                });
        }

        CollectionChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
    }

    private static int GetSize(Node? node)
        => node?.Size ?? 0;

    private static void Update(Node node)
        => node.Size = 1 + GetSize(node.Left) + GetSize(node.Right);

    private static uint RandomPriority()
    {
        return (uint)Random.Shared.NextInt64(1, uint.MaxValue);
    }

    private static Node Insert(Node? root, Node node)
    {
        if (root is null)
            return node;

        if (node.Key < root.Key)
        {
            root.Left = Insert(root.Left, node);

            if (root.Left!.Priority > root.Priority)
            {
                root = RotateRight(root);
            }
        }
        else
        {
            root.Right = Insert(root.Right, node);

            if (root.Right!.Priority > root.Priority)
            {
                root = RotateLeft(root);
            }
        }

        Update(root);
        return root;
    }

    private static Node? Remove(Node? root, int key)
    {
        if (root is null)
        {
            return null;
        }

        if (key < root.Key)
        {
            root.Left = Remove(root.Left, key);
        }
        else if (key > root.Key)
        {
            root.Right = Remove(root.Right, key);
        }
        else
        {
            return Merge(root.Left, root.Right);
        }

        Update(root);
        return root;
    }

    private static Node? Merge(Node? left, Node? right)
    {
        if (left is null)
        {
            return right;
        }

        if (right is null)
        {
            return left;
        }

        if (left.Priority > right.Priority)
        {
            left.Right = Merge(left.Right, right);
            Update(left);
            return left;
        }

        right.Left = Merge(left, right.Left);
        Update(right);
        return right;
    }

    private static Node RotateRight(Node root)
    {
        var newRoot = root.Left!;

        root.Left = newRoot.Right;
        newRoot.Right = root;

        Update(root);
        Update(newRoot);

        return newRoot;
    }

    private static Node RotateLeft(Node root)
    {
        var newRoot = root.Right!;

        root.Right = newRoot.Left;
        newRoot.Left = root;

        Update(root);
        Update(newRoot);

        return newRoot;
    }

    private static Node GetAt(Node root, int index)
    {
        var node = root;

        while (true)
        {
            var leftSubtreeSize = GetSize(node.Left);

            if (index < leftSubtreeSize)
            {
                node = node.Left!;
            }
            else if (index == leftSubtreeSize)
            {
                return node;
            }
            else
            {
                index -= leftSubtreeSize + 1;
                node = node.Right!;
            }
        }
    }

    private static int Rank(Node? root, int key)
    {
        var rank = 0;
        var node = root;

        while (node is not null)
        {
            if (key < node.Key)
            {
                node = node.Left;
            }
            else if (key > node.Key)
            {
                rank += GetSize(node.Left) + 1;
                node = node.Right;
            }
            else
            {
                rank += GetSize(node.Left);
                return rank;
            }
        }

        throw new InvalidOperationException(
            "The specified key does not exist in the tree.");
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Enumerate(_root).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static IEnumerable<T> Enumerate(Node? node)
    {
        if (node is null)
            yield break;

        foreach (var item in Enumerate(node.Left))
        {
            yield return item;
        }

        yield return node.Item;

        foreach (var item in Enumerate(node.Right))
        {
            yield return item;
        }
    }
}
