using System;
using System.Collections.Generic;
using System.Linq;

namespace _01.FrendsOfPesho
{
    class FrendsOfPesho
    {
        static void Main()
        {
            string[] mainInput = Console.ReadLine().Split(' ');
            string[] hospitals = Console.ReadLine().Split(' ');
            int totalNumberOfObjectsInMap = int.Parse(mainInput[0]);
            int connectionCount = int.Parse(mainInput[1]);
            int hospitalCount = int.Parse(mainInput[2]);

            Dictionary<Node, List<Connection>> graph = new Dictionary<Node, List<Connection>>();
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < connectionCount; i++)
            {
                string[] currentLine = Console.ReadLine().Split(' ');

                int startPoint = int.Parse(currentLine[0]);
                int endPoint = int.Parse(currentLine[1]);
                int dist = int.Parse(currentLine[2]);

                bool isStartPointNodeAlreadyIncludedInNodesList = false;
                bool isEndPointNodeAlreadyIncludedInNodesList = false;
                int currentStartNodeIndex = -1;
                int currentEndNodeIndex = -1;

                for (int j = 0; j < nodes.Count; j++)
                {
                    if (nodes[j].ID == startPoint)
                    {
                        isStartPointNodeAlreadyIncludedInNodesList = true;
                        currentStartNodeIndex = j;
                    }

                    if (nodes[j].ID == endPoint)
                    {
                        isEndPointNodeAlreadyIncludedInNodesList = true;
                        currentEndNodeIndex = j;
                    }
                }

                if (!isStartPointNodeAlreadyIncludedInNodesList)
                {
                    nodes.Add(new Node(startPoint));
                    currentStartNodeIndex = nodes.Count - 1; //*last included node
                }

                if (!isEndPointNodeAlreadyIncludedInNodesList)
                {
                    nodes.Add(new Node(endPoint));
                    currentEndNodeIndex = nodes.Count - 1;//*last included node
                }

                Connection currentConnection = new Connection(nodes[currentEndNodeIndex], dist);

                if (graph.ContainsKey(nodes[currentStartNodeIndex]))
                {
                    graph[nodes[currentStartNodeIndex]].Add(currentConnection);
                }
                else
                {
                    List<Connection> newList = new List<Connection>();
                    newList.Add(currentConnection);
                    graph.Add(nodes[currentStartNodeIndex], newList);
                }
            }

            int[] diikstraDIstances = new int[hospitals.Length];

            for (int i = 0; i < hospitals.Length; i++)
            {
                int hospitalID = int.Parse(hospitals[i]);
                int currentHospitalIndexInNodesList = -1;

                for (int j = 0; j < nodes.Count ; j++)
                {
                    if (nodes[j].ID == hospitalID)
                    {
                        currentHospitalIndexInNodesList = j;
                        break;
                    }
                }

                DijkstraAlgorithm(graph, nodes[currentHospitalIndexInNodesList]);
                int sum = 0;
                for (int k = 0; k < nodes.Count; k++)
                {
                    //if()
                    //{
                    sum += nodes[k].DijkstraDistance;
                    //}
                }
                diikstraDIstances[i] = sum;// nodes[currentHospitalIndexInNodesList].DijkstraDistance;
            }

            int minDistance = int.MaxValue;
            for (int i = 0; i < diikstraDIstances.Length; i++)
            {
                if (diikstraDIstances[i] < minDistance)
                {
                    minDistance = diikstraDIstances[i];
                }
            }

            Console.WriteLine(minDistance);
        }

        static void DijkstraAlgorithm(Dictionary<Node, List<Connection>> graph, Node source)
        {
            PriorityQueue<Node> queue = new PriorityQueue<Node>();

            foreach (var node in graph)
            {
                node.Key.DijkstraDistance = int.MaxValue;
                queue.Enqueue(node.Key);
            }

            source.DijkstraDistance = 0;
            queue.Enqueue(source);

            while (queue.Count != 0)
            {
                Node currentNode = queue.Peek();

                if (currentNode.DijkstraDistance == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbour in graph[currentNode])
                {
                    int potDistance = currentNode.DijkstraDistance + neighbour.Distance;

                    if (potDistance < neighbour.Node.DijkstraDistance)
                    {
                        neighbour.Node.DijkstraDistance = potDistance;
                    }
                }

                queue.Dequeue();
            }
        }
    }

    public class Connection
    {
        public Node Node { get; set; }
        public int Distance { get; set; }

        public Connection(Node node, int distance)
        {
            this.Node = node;
            this.Distance = distance;
        }
    }

    public class Node : IComparable
    {
        public int ID { get; private set; }
        public int DijkstraDistance { get; set; }

        public Node(int id)
        {
            this.ID = id;
        }

        public int CompareTo(object obj)
        {
            return this.DijkstraDistance.CompareTo((obj as Node).DijkstraDistance);
        }
    }

    public class PriorityQueue<T> where T : IComparable
    {
        private T[] heap;
        private int index;

        public int Count
        {
            get
            {
                return this.index - 1;
            }
        }

        public PriorityQueue()
        {
            this.heap = new T[16];
            this.index = 1;
        }

        public void Enqueue(T element)
        {
            if (this.index >= this.heap.Length)
            {
                IncreaseArray();
            }

            this.heap[this.index] = element;

            int childIndex = this.index;
            int parentIndex = childIndex / 2;
            this.index++;

            while (parentIndex >= 1 && this.heap[childIndex].CompareTo(this.heap[parentIndex]) < 0)
            {
                T swapValue = this.heap[parentIndex];
                this.heap[parentIndex] = this.heap[childIndex];
                this.heap[childIndex] = swapValue;

                childIndex = parentIndex;
                parentIndex = childIndex / 2;
            }
        }

        public T Dequeue()
        {
            T result = this.heap[1];

            this.heap[1] = this.heap[this.Count];
            this.index--;

            int rootIndex = 1;

            int minChild;

            while (true)
            {
                int leftChildIndex = rootIndex * 2;
                int rightChildIndex = rootIndex * 2 + 1;

                if (leftChildIndex > this.index)
                {
                    break;
                }
                else if (rightChildIndex > this.index)
                {
                    minChild = leftChildIndex;
                }
                else
                {
                    if (this.heap[leftChildIndex].CompareTo(this.heap[rightChildIndex]) < 0)
                    {
                        minChild = leftChildIndex;
                    }
                    else
                    {
                        minChild = rightChildIndex;
                    }
                }

                if (this.heap[minChild].CompareTo(this.heap[rootIndex]) < 0)
                {
                    T swapValue = this.heap[rootIndex];
                    this.heap[rootIndex] = this.heap[minChild];
                    this.heap[minChild] = swapValue;

                    rootIndex = minChild;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public T Peek()
        {
            return this.heap[1];
        }

        private void IncreaseArray()
        {
            T[] copiedHeap = new T[this.heap.Length * 2];

            for (int i = 0; i < this.heap.Length; i++)
            {
                copiedHeap[i] = this.heap[i];
            }

            this.heap = copiedHeap;
        }
    }
}
