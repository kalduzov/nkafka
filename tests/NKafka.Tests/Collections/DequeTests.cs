using NKafka.Collections;

namespace NKafka.Tests.Collections;

public class DequeTests
{
    private record Test(int Value = 0);

    [Fact]
    public void Count_MustBeZero_AfterCreation()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);

        deque.Count.Should().Be(0);
        deque.IsSynchronized.Should().BeFalse();
    }

    [Fact]
    public void RemoveFirst_WhenDequeEmpty_MustBeReturn_DefaultClass()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);
        var val = deque.RemoveFirst();
        val.Should().Be(defaultTest);
    }

    [Fact]
    public void RemoveLast_WhenDequeEmpty_MustBeReturn_DefaultClass()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);
        var val = deque.RemoveLast();
        val.Should().Be(defaultTest);
    }

    [Fact]
    public void PeekFirst_WhenDequeEmpty_MustBeReturn_DefaultClass()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);
        var val = deque.PeekFirst();
        val.Should().Be(defaultTest);
    }

    [Fact]
    public void TryPeekLast_WhenDequeEmpty_MustBeReturn_DefaultClass()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);
        var val = deque.TryPeekLast(out var last);
        val.Should().BeFalse();
        last.Should().Be(defaultTest);
    }

    [Fact]
    public void AfterClear_MustBeReturn_DefaultClass()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);

        deque.AddFirst(new Test(1));
        deque.AddLast(new Test(2));

        deque.Count.Should().Be(2);

        deque.Clear();

        deque.Count.Should().Be(0);

        var result = deque.TryPeekLast(out var val);
        result.Should().BeFalse();
        val.Should().Be(defaultTest);

        val = deque.PeekFirst();
        val.Should().Be(defaultTest);

        val = deque.RemoveLast();
        val.Should().Be(defaultTest);

        val = deque.RemoveFirst();
        val.Should().Be(defaultTest);
    }

    [Fact]
    public void AddFirst_And_RemoveFirst_MustBe_Successful()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);

        deque.AddFirst(new Test(1));
        deque.Count.Should().Be(1);

        deque.AddFirst(new Test(2));
        deque.Count.Should().Be(2);

        deque.RemoveFirst().Value.Should().Be(2);
        deque.RemoveFirst().Value.Should().Be(1);
        deque.RemoveFirst().Should().Be(defaultTest);
    }

    [Fact]
    public void AddFirst_And_RemoveLast_MustBe_Successful()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);

        deque.AddFirst(new Test(1));
        deque.Count.Should().Be(1);

        deque.AddFirst(new Test(2));
        deque.Count.Should().Be(2);

        deque.RemoveLast().Value.Should().Be(1);
        deque.RemoveLast().Value.Should().Be(2);
        deque.RemoveLast().Should().Be(defaultTest);
    }

    [Fact]
    public void AddLast_And_RemoveLast_MustBe_Successful()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);

        deque.AddLast(new Test(1));
        deque.Count.Should().Be(1);

        deque.AddLast(new Test(2));
        deque.Count.Should().Be(2);

        deque.RemoveLast().Value.Should().Be(2);
        deque.RemoveLast().Value.Should().Be(1);
        deque.RemoveLast().Should().Be(defaultTest);
    }

    [Fact]
    public void AddLast_And_RemoveFirst_MustBe_Successful()
    {
        var defaultTest = new Test();
        var deque = new Deque<Test>(defaultTest);

        deque.AddLast(new Test(1));
        deque.Count.Should().Be(1);

        deque.AddLast(new Test(2));
        deque.Count.Should().Be(2);

        deque.RemoveFirst().Value.Should().Be(1);
        deque.RemoveFirst().Value.Should().Be(2);
        deque.RemoveFirst().Should().Be(defaultTest);
    }
}