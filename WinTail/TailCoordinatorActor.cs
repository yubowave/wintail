using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = message as StartTail;

                Context.ActorOf(Props.Create(() => new TailActor(msg.ReportActor, msg.FilePath)));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30),
                e =>
                {
                    if (e is ArithmeticException) return Directive.Resume;
                    else if (e is NotSupportedException) return Directive.Stop;
                    else return Directive.Restart;
                });
        }


        public class StartTail
        {
            public StartTail(string filePath, IActorRef reportActor)
            {
                FilePath = filePath;
                ReportActor = reportActor;
            }

            public string FilePath { get; private set; }

            public IActorRef ReportActor { get; private set; }
        }

        public class StopTail
        {
            public StopTail(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; private set; }
        }
    }
}
