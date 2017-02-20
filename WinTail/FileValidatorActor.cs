using System;
using System.IO;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console.
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>. 
    /// </summary>
    class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public FileValidatorActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;

            if (string.IsNullOrEmpty(msg))
            {
                _consoleWriterActor.Tell(new NullInputError("Input was blank. Please try again."));

                Sender.Tell(new ContinueProcessing());
            }
            else
            {
                var valid = IsFileUrl(msg);
                if (valid)
                {
                    _consoleWriterActor.Tell(new InputSucess($"Starting processing for {msg}"));

                    var selection = Context.ActorSelection("akka://myActorSystem/user/tailcoordinator");
                    selection.Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                }
                else
                {
                    _consoleWriterActor.Tell(new InputError("{msg} is not an existing URI on disk."));

                    Sender.Tell(new ContinueProcessing());
                }
            }            
        }

        private bool IsFileUrl(string msg)
        {
            return File.Exists(msg);
        }
    }
}
