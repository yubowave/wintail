using Akka.Actor;

namespace TailCoordinatorActor
{
    internal class StartTail
    {
        private string msg;
        private IActorRef _consoleWriterActor;

        public StartTail(string msg, IActorRef _consoleWriterActor)
        {
            this.msg = msg;
            this._consoleWriterActor = _consoleWriterActor;
        }
    }
}