using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions.Collections;
using AVS.CoreLib.Collections;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests.Collections
{
    [TestClass]
    public class FixedListTests
    {
        protected IQueue<int> CreateQueue(int capacity)
        {
            return new FixedList<int>(capacity);
        }

        protected IQueue<int> CreateQueue(int[] items)
        {
            return new FixedList<int>(items);
        }

        protected IStack<int> CreateStack(int capacity)
        {
            return new FixedList<int>(capacity);
        }

        protected IStack<int> CreateStack(int[] items)
        {
            return new FixedList<int>(items);
        }

        #region IList<T> tests

        #region Add / TryAdd / Insert
        [TestMethod]
        public void TryAdd_Should_Add_Elements_Until_Reached_Capacity()
        {
            // Arrange
            var elements = new[] { 1, 2, 3 };
            var list = new FixedList<int>(2);
            var is3Added = false;

            // Act
            foreach (var el in elements)
            {
                is3Added = list.TryAdd(el);
                if (is3Added == false)
                    break;
            }

            // Assert
            list.Head.Should().Be(0);
            list.Count.Should().Be(2);
            list[0].Should().Be(1);
            list[1].Should().Be(2);
            is3Added.Should().BeFalse();
        }

        [TestMethod]
        public void Should_Throw_When_Add_Over_Capacity()
        {
            // Arrange
            var list = new FixedList<int>(2);

            // Act
            list.Add(1);
            list.Add(2);
            Assert.ThrowsException<InvalidOperationException>(() => list.Add(3));

            // Assert
            list.Head.Should().Be(0);
            list.Count.Should().Be(2);
        }

        [TestMethod]
        public void Insert_Should_Insert_Element_At_Given_Index() 
        {
            // Arrange
            var list = new FixedList<int>(4) { 1, 3, 4 };

            // Act
            list.Insert(1, 2); // {1, 2, 3, 4} 

            // Assert
            list.Head.Should().Be(0);
            list.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
        }

        [TestMethod]
        public void Should_Throw_When_Insert_Element_Over_Capacity()
        {
            // Arrange
            var list = new FixedList<int>(4) { 1, 2, 3, 4 };

            // Act
            Assert.Throws<InvalidOperationException>(() => list.Insert(1, 10));

            // Assert
            list.Head.Should().Be(0);
            list.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
        }

        [TestMethod]
        public void Insert_Should_Insert_Element_At_Given_Index_When_Head_IsNot_Zero()
        {
            // Arrange
            var list = new FixedList<int>(4) { 1, 2, 3, 4 };
            list.Dequeue(); // {2,3,4} Head = 1
            list.Dequeue(); // {3,4} Head = 2

            // Act & Assert
            list.Insert(0, 10); // {10,3,4} Head=1
            list.Should().BeEquivalentTo(new[] { 10, 3, 4 });

            list.Insert(1, 20); // {10,20,3,4} Head=0
            list.Should().BeEquivalentTo(new[] { 10,20, 3, 4 });
            list.Count.Should().Be(4);
        }

        #endregion

        #region Remove / Clear
        [TestMethod]
        public void Should_Remove_Item_From_The_List()
        {
            // Arrange
            var list = new FixedList<int>(3) { 1, 2, 3 };

            // Act
            list.Remove(2);

            // Assert
            list.Count.Should().Be(2);
            list.Should().BeEquivalentTo(new[] { 1, 3 });
        }

        [TestMethod]
        public void Should_Remove_Item_From_The_List_At_Given_Index()
        {
            // Arrange
            var list = new FixedList<int>(3) { 1, 2, 3 };

            // Act
            list.RemoveAt(2);

            // Assert
            list.Count.Should().Be(2);
            list.Should().BeEquivalentTo(new[] { 1, 2 });
        }

        [TestMethod]
        public void Should_Remove_Item_From_The_List_At_Given_Index_When_Head_Not_Zero()
        {
            // Arrange
            var list = new FixedList<int>(4) { 1, 2, 3, 4 };
            list.Dequeue(); // {2,3,4} Head = 1

            // Act
            list.RemoveAt(1); // => {2,4} Head=1

            // Assert
            list.Count.Should().Be(2);
            list.Should().BeEquivalentTo(new[] { 2, 4 });
        }

        [TestMethod]
        public void Should_Clear_All_Items_From_The_List()
        {
            // Arrange
            var list = new FixedList<int>(4) { 1, 2, 3, 4 };
            list.Dequeue(); // {2,3,4} Head = 1

            // Act
            list.Clear(); // => { } Head=0 Count=0

            // Assert
            list.Count.Should().Be(0);
            list.Head.Should().Be(0);
            list.Should().BeEquivalentTo(Array.Empty<int>());
        }

        #endregion


        [TestMethod]
        public void Should_Return_IndexOf_Element()
        {
            // Arrange
            var list = new FixedList<int>(3) { 1, 2, 3 };

            // Act
            var ind = list.IndexOf(3);

            // Assert
            ind.Should().Be(2);
        }

        [TestMethod]
        public void Contains_Should_Return_True_When_Element_In_The_List()
        {
            // Arrange
            var list = new FixedList<int>(3) { 1, 2, 3 };

            // Act
            var exists3 = list.Contains(3);
            var exists30 = list.Contains(30);

            // Assert
            exists3.Should().BeTrue();
            exists30.Should().BeFalse();
        }

        #endregion

        #region IQueue<T> tests

        [TestMethod]
        public void Should_Enqueue_And_Dequeue_Elements_In_FIFO_Order()
        {
            // Arrange
            var q = CreateQueue(3);
            var items = new [] { 1, 2, 3};

            // Act
            q.Enqueue(items[0]);
            q.Enqueue(items[1]);
            q.Enqueue(items[2]);

            // Assert
            Assert.AreEqual(3, q.Count);

            var item1 = q.Dequeue();
            q.Should().BeEquivalentTo(new[] { 2, 3 });
            var item2 = q.Dequeue();
            q.Should().BeEquivalentTo(new[] { 3 });

            var item3 = q.Dequeue();
            q.Should().BeEmpty();
            Assert.AreEqual(0, q.Count);

            Assert.AreEqual(items[0], item1);
            Assert.AreEqual(items[1], item2);
            Assert.AreEqual(items[2], item3);
        }

        [TestMethod]
        public void Should_Throw_When_EnqueueStrict_Element_Over_Q_Capacity()
        {
            // Arrange
            var q = CreateQueue(2);
            var items = new[] { 1, 2, 3 };

            // Act
            q.Enqueue(items[0]);
            q.Enqueue(items[1]);

            // Assert
            q.Count.Should().Be(2);
            Assert.Throws<InvalidOperationException>(() => q.EnqueueStrict(items[2]));
        }

        [TestMethod]
        public void Q_Should_Be_FixedSize()
        {
            // Arrange
            var elements = new[] { 1, 2, 3 };
            var q = CreateQueue(elements.ToArray());
            var newElements = new[] { 4, 5 ,6};

            var dequeuedElements = new List<int>(3);

            // Act
            foreach (var newElement in newElements)
            {
                var removed = q.Enqueue(newElement);
                dequeuedElements.Add(removed);
            }

            var qElements = q.ToArray();

            // Assert
            Assert.AreEqual(3, q.Count);
            dequeuedElements.Should().BeEquivalentTo(elements);
            qElements.Should().BeEquivalentTo(newElements);
        }

        [TestMethod]
        public void Peek_Should_Return_First_Element_Without_Removing_It_From_Q()
        {
            // Arrange
            var q = CreateQueue(new []{ 10 , 20});

            // Act & Assert
            Assert.AreEqual(10, q.Peek());
            Assert.AreEqual(10, q.Peek());
            Assert.AreEqual(2, q.Count);
        }

        [TestMethod]
        public void Peek_Should_Return_Elements_From_Q_By_Offset()
        {
            // Arrange
            var q = CreateQueue(new[] { 10, 20, 30, 40 }); // { 10, 20, 30, 40 } => queue:[10,20,30,40]

            // Act & Assert
            Assert.AreEqual(40, q.Peek(0));
            Assert.AreEqual(30, q.Peek(1));
            Assert.AreEqual(20, q.Peek(2));
            Assert.AreEqual(10, q.Peek(3));
        }

        [TestMethod]
        public void Indexer_Should_Peek_Elements_From_Q_By_Index()
        {
            // Arrange
            var q = CreateQueue(new[] { 10, 20, 30 });

            // Act & Assert
            Assert.AreEqual(10, q[0]);
            Assert.AreEqual(20, q[1]);
            Assert.AreEqual(30, q[2]);
        }

        [TestMethod]
        public void PeekLast_Should_Return_Latest_Enqueued_Element()
        {
            // Arrange
            var q = (FixedList<int>)CreateQueue(3);
            q.Enqueue(10);
            q.Enqueue(20);
            q.Enqueue(30);// queue:[10,20,30]

            // Act & Assert
            var last = q.PeekLast();
            last.Should().Be(30);
            Assert.AreEqual(last, q.Peek(0)); //last == offset = 0
        }

        #endregion

        #region IStack<T> tests

        [TestMethod]
        public void Should_Push_And_Pop_Elements_In_LIFO_Order()
        {
            var stack = CreateStack(3);

            var items = new[] { 1, 2, 3 };

            // 1. Push elements
            stack.PushStrict(items[2]);
            stack.PushStrict(items[1]);
            stack.PushStrict(items[0]);

            Assert.AreEqual(3, stack.Count);
            stack.Should().BeEquivalentTo(new[] { 1, 2, 3 });

            // 2. Pop elements
            var item1 = stack.Pop();
            stack.Should().BeEquivalentTo(new[] { 2, 3 });
            var item2 = stack.Pop();
            stack.Should().BeEquivalentTo(new[] { 3 });
            var item3 = stack.Pop();
            stack.Should().BeEmpty();

            // Assert
            Assert.AreEqual(0, stack.Count);
            Assert.AreEqual(items[0], item1);
            Assert.AreEqual(items[1], item2);
            Assert.AreEqual(items[2], item3);
        }

        [TestMethod]
        public void Should_Throw_When_Push_Element_Over_Capacity()
        {
            // Arrange
            var q = CreateStack(2);
            var items = new[] { 1, 2, 3 };

            // Act
            q.PushStrict(items[0]);
            q.PushStrict(items[1]);

            Assert.Throws<InvalidOperationException>(() => q.PushStrict(items[2]));
        }

        [TestMethod]
        public void Stack_Should_Be_FixedSize()
        {
            // Arrange
            var stack = CreateStack(3);
            var elements = new[] { 3, 2, 1 };
            var newElements = new[] { 6, 5, 4 };
            var popedElements = new List<int>(3);

            // Act
            //1. push elements
            foreach (var element in elements)
                stack.PushStrict(element);

            stack.Count.Should().Be(3);
            stack.Should().BeEquivalentTo(elements.Reverse());

            //2. push new elements
            foreach (var newElement in newElements)
                popedElements.Add(stack.Push(newElement));

            var sElements = stack.ToArray();

            // Assert
            stack.Count.Should().Be(3);
            popedElements.Should().BeEquivalentTo(new[]{ 1, 6, 5 });
            sElements.Should().BeEquivalentTo(new[] { 3, 2, 4});
        }

        [TestMethod]
        public void Peek_Should_Return_First_Element_Without_Removing_It_From_Stack()
        {
            // Arrange
            var stack = CreateStack(4);
            stack.PushStrict(40);
            stack.PushStrict(30);
            stack.PushStrict(20);
            stack.PushStrict(10);
            stack.Count.Should().Be(4);

            // Act & Assert
            Assert.AreEqual(10, stack.Peek());
            Assert.AreEqual(10, stack.Pop());
            Assert.AreEqual(20, stack.Peek());
            Assert.AreEqual(20, stack.Pop());
            Assert.AreEqual(30, stack.Peek());
            stack.Count.Should().Be(2);
        }

        [TestMethod]
        public void Peek_Should_Return_Elements_From_Stack_By_Offset()
        {
            // Arrange
            var stack = CreateStack(4);
            stack.PushStrict(40);
            stack.PushStrict(30);
            stack.PushStrict(20);
            stack.PushStrict(10); // {40,30,20,10} => stack:[10,20,30,40]

            // Act & Assert
            Assert.AreEqual(40, stack.Peek(0));
            Assert.AreEqual(30, stack.Peek(1));
            Assert.AreEqual(20, stack.Peek(2));
            Assert.AreEqual(10, stack.Peek(3));
            stack.Count.Should().Be(4);
        }

        #endregion

        public void ToArray_Should_Return_Array_In_Correct_Order()
        {
            // Arrange
            var list = new FixedList<int>(3) { 5, 20, 3 };

            // Act
            var arr = list.ToArray();
            //assert
            arr.Should().BeEquivalentTo(new[] { 1, 2, 3 });
            arr[0].Should().Be(1);
            arr[1].Should().Be(2);
            arr[2].Should().Be(3);
        }

        [TestMethod]
        public void Put_Should_Add_Removing_Older_Items()
        {
            //arrange
            var list = new FixedList<int>(3) { 1, 2, 3 };

            // act
            list.Put(4); // { 1 ,2 ,3 } => { 2, 3, 4 }

            //assert
            list.Count.Should().Be(3);
            var arr = list.ToArray();

            arr.Should().BeEquivalentTo(new [] { 2, 3, 4 });

            arr[0].Should().Be(2);
            arr[1].Should().Be(3);
            arr[2].Should().Be(4);
        }

        [TestMethod]
        public void Should_Put_Item_On_Top()
        {
            //arrange
            var list = new FixedList<int>(3) { 1, 2, 3 };

            // act
            list.Put(2);

            //assert
            list.Count.Should().Be(3);
            var arr = list.ToArray();

            arr.Should().BeEquivalentTo(new int[] { 1, 3, 2 });
        }

        [TestMethod]
        public void Should_Add_To_The_End_And_Put_On_Top_Correctly()
        {
            //arrange
            var list = new FixedList<int>(5) { 1, 2, 3, 4, 5 };

            // act
            list.Put(6);
            list.Put(2);

            // assert
            var arr1 = list.ToArray();
            arr1.Should().BeEquivalentTo(new[] { 3, 4, 5, 6, 2 });

            // act
            list.Put(7);
            list.Put(2);

            //assert
            list.Count.Should().Be(5);
            var arr2 = list.ToArray();
            arr2.Should().BeEquivalentTo(new[] { 4, 5, 6, 7, 2 });
        }
    }
}
