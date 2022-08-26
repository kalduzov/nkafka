using System.Runtime.CompilerServices;
using System.Text;

using NKafka.MessageGenerator.Specifications;

[assembly: InternalsVisibleTo("NKafka.MessageGenerator.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace NKafka.MessageGenerator;

public interface IMessageGenerator
{
    StringBuilder Generate(MessageSpecification message);

    // /// <summary>
    // /// Generate *.cs class base from message api description 
    // /// </summary>
    // StringBuilder GenerateMessage(string className, MessageSpecification apiDescriptor);
    //
    // /// <summary>
    // /// Generate *Tests.cs class base from message api description 
    // /// </summary>
    // StringBuilder GenerateMessageTests(string className, MessageSpecification apiDescriptor);
    string ClassName(MessageSpecification messageSpecification)
        => messageSpecification.ClassName;
}