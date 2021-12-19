using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Microlibs.Kafka.BuildTasks;

public class MessagesGeneratorTask : Task
{
    private readonly IMessageGenerator _messageGenerator;

    [Required]
    public string SolutionDirectory { get; set; }

    public MessagesGeneratorTask()
    {
        _messageGenerator = new MessageGenerator(SolutionDirectory,"");
    }

    public override bool Execute()
    {
        Log.LogMessage(MessageImportance.High, "Start messages generator");

        try
        {
            _messageGenerator.Generate();
        }
        catch (Exception exc)
        {
            Log.LogError(exc.Message);
        }
        finally
        {
            Log.LogMessage(MessageImportance.High, "Finish messages generator");
        }

        return true;
    }
}