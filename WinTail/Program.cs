using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Akka.Actor;

namespace WinTail
{
    class Program
    {
        static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            MyActorSystem = ActorSystem.Create("myActorSystem");

            //PrintInstructions();

            // actors
            var writerActorProps = Props.Create<ConsoleWriterActor>();
            var writerActor = MyActorSystem.ActorOf(writerActorProps, "WriterActor");

            var tailCoordinaterProps = Props.Create(()=> new TailCoordinatorActor());
            var tailCoordinaterActor = MyActorSystem.ActorOf(tailCoordinaterProps, "tailcoordinator");

            var fileValidActorProps = Props.Create(() => new FileValidatorActor(writerActor, tailCoordinaterActor));
            var validActor = MyActorSystem.ActorOf(fileValidActorProps, "ValidationActor");

            var readerActorProps = Props.Create(() => new ConsoleReaderActor(validActor));
            var readerActor = MyActorSystem.ActorOf(readerActorProps, "ReaderActor");

            // start working
            readerActor.Tell(ConsoleReaderActor.StartCommand);

            // block
            MyActorSystem.WhenTerminated.Wait();
        }

        private static void PrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.Write("Some lines will appear as");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" red ");
            Console.ResetColor();
            Console.Write(" and others will appear as");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" green! ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            //Console.WriteLine("Type 'exit' to quit this application at any time.");
        }
    }
}
