using System.Text;

namespace NKafka.MessageGenerator;

public interface IMessageGenerator
{
    StringBuilder Generate();

    // /// <summary>
    // /// Generate *.cs class base from message api description 
    // /// </summary>
    // StringBuilder GenerateMessage(string className, ApiDescriptor apiDescriptor);
    //
    // /// <summary>
    // /// Generate *Tests.cs class base from message api description 
    // /// </summary>
    // StringBuilder GenerateMessageTests(string className, ApiDescriptor apiDescriptor);
    string ClassName { get; }
}